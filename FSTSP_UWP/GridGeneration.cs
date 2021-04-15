using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace FSTSP_UWP
{
    class GridGeneration
    {
        //public static void fillGrid(SquareGrid grid, int areaSize, int areaHeight)
        //{
        //    bool[,,] obstacles = new bool[areaSize, areaSize, areaHeight];
        //    Random rnd = new Random();
        //    int threshold = 65;

        //    for (int x = 0; x < areaSize; x++)
        //    {
        //        for (int y = 0; y < areaSize; y++)
        //        {
        //            obstacles[x, y, 0] = true;
        //        }
        //    }

        //    for (int x = 0; x < areaSize; x++)
        //    {
        //        if (rnd.Next(100) > threshold)
        //        {
        //            for (int y = 0; y < areaSize; y++)
        //            {
        //                obstacles[x, y, 0] = false;
        //            }
        //        }
        //    }

        //    for (int y = 0; y < areaSize; y++)
        //    {
        //        if (rnd.Next(100) > threshold)
        //        {
        //            for (int x = 0; x < areaSize; x++)
        //            {
        //                obstacles[x, y, 0] = false;
        //            }
        //        }
        //    }

        //    for (int x = 0; x < areaSize; x++)
        //    {
        //        for (int y = 0; y < areaSize; y++)
        //        {
        //            if (obstacles[x, y, 0])
        //                grid.walls.Add(new Location(x, y, 0));
        //        }
        //    }


        //    for (int z = 1; z < areaHeight; z++)
        //    {
        //        for (int x = 0; x < areaSize; x++)
        //        {
        //            for (int y = 0; y < areaSize; y++)
        //            {
        //                if ((rnd.Next(100) > threshold || rnd.Next(100) > 20) && obstacles[x, y, z - 1])
        //                {
        //                    obstacles[x, y, z] = true;
        //                    grid.walls.Add(new Location(x, y, z));
        //                }
        //                else
        //                {
        //                    obstacles[x, y, z] = false;
        //                }
        //            }
        //        }
        //        threshold -= 5;
        //    }
        //}

        public static void fillGrid(SquareGrid grid, int areaSize, int areaHeight)
        {
            //var threshold = 65;
            Random rnd = new Random();

            List<int> xStreets = new List<int>();
            List<int> yStreets = new List<int>();
            defineStreets(xStreets, yStreets, areaSize);

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            for (int x = 0; x < areaSize; x++)
            {
                for (int y = 0; y < areaSize; y++)
                {
                    if (!xStreets.Contains(x) && !yStreets.Contains(y))
                        grid.walls.Add(new Location(x, y, 0));
                }
            }
            


            var increment = 1.0;
            for (int z = 1; z < areaHeight; z++)
            {
                var previousLevelWalls = grid.walls.Where(location => location.z == z - 1).ToList();
                //var newLevelWalls = new List<Location>();
                for(int i = 0; i < previousLevelWalls.Count(); i += (int) Math.Round(increment, MidpointRounding.ToEven))
                {
                    grid.walls.Add(new Location(previousLevelWalls[i].x, previousLevelWalls[i].y, z));
                }
                increment += 0.3;
            }

            watch.Stop();
            var seconds = watch.ElapsedMilliseconds / 1000;

            //saveToXML(grid, "c:\\FSTSP_Files\\grid.xml");
            //loadFromXML(grid, "c:\\FSTSP_Files\\grid.xml");
        }

        static void defineStreets(List<int> xStreets, List<int> yStreets, int size)
        {
            var threshold = 70;
            Random rnd = new Random();
            for (int i = 0; i < size; i++)
            {
                if (rnd.Next(100) > threshold && !xStreets.Contains(i-1))
                {
                    xStreets.Add(i);
                }
                if (rnd.Next(100) > threshold && !yStreets.Contains(i - 1))
                {
                    yStreets.Add(i);
                }
            }
        }

        public static void saveToXML(SquareGrid grid, string filename)
        {
            TextWriter writer = new StreamWriter(filename);
            XDocument document = new XDocument();

            var Grid = new XElement("Grid", new XAttribute("xsd", "http://www.w3.org/2001/XMLSchema"),
                                            new XAttribute("xsi", "http://www.w3.org/2001/XMLSchema-instance"));

            document.Add(Grid);
            Grid.Add(new XElement("Measurements", new XElement("length", grid.length), new XElement("width", grid.width), new XElement("height", grid.height)));
            var walls = new XElement("Walls");
            Grid.Add(walls);

            foreach(var wall in grid.walls)
            {
                walls.Add(new XElement("wall", new XElement("x", wall.x), new XElement("y", wall.y), new XElement("z", wall.z)));
            }

            document.Save(writer);
            writer.Close();
        }

        public static void loadFromXML(SquareGrid grid, string filename)
        {

            var xdocument = new XmlDocument();
            xdocument.Load(filename);
            var doc = xdocument.DocumentElement;

            int length = 0, width = 0, height = 0;
            var measurements = doc.GetElementsByTagName("Measurements")[0];
            foreach(XmlNode xnode in measurements)
            {
                switch (xnode.Name)
                {
                    case "length":
                        length = Int16.Parse(xnode.InnerText);
                        break;
                    case "width":
                        width = Int16.Parse(xnode.InnerText);
                        break;
                    case "height":
                        height = Int16.Parse(xnode.InnerText);
                        break;
                }
            }
            var newGrid = new SquareGrid(length, width, height);

            var walls = doc.GetElementsByTagName("wall");

            foreach(XmlNode xnode in walls)
            {
                int x = 0, y = 0, z = 0;
                foreach(XmlNode childnode in xnode.ChildNodes)
                {
                    switch (childnode.Name)
                    {
                        case "x":
                            x = Int16.Parse(childnode.InnerText);
                            break;
                        case "y":
                            y = Int16.Parse(childnode.InnerText);
                            break;
                        case "z":
                            z = Int16.Parse(childnode.InnerText);
                            break;
                    }
                }
                newGrid.walls.Add(new Location(x, y, z));
            }
        }
    }
}
