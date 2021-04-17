using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;

namespace FSTSP_UWP
{
    class routing
    {
        public double Haversine(double lat1, double lat2, double lon1, double lon2, double alt1, double alt2)
        {
            if (lat1 == lat2 && lon1 == lon2)
            {
                return Math.Abs(alt1 - alt2);
            }
            var rlat1 = Math.PI * lat1 / 180;
            var rlat2 = Math.PI * lat2 / 180;
            var theta = lon1 - lon2;
            var rtheta = Math.PI * theta / 180;

            var distance = Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) * Math.Cos(rlat2) * Math.Cos(rtheta);
            distance = Math.Acos(distance);
            distance = distance * 180 / Math.PI;
            distance = (distance * 60 * 1.1515) * 1.609344 * 1000;

            return distance;
        }
        public void MapMaker(ref Graph[,,] map, Graph[] nodes, int dimX, int dimY, int dimZ, double x0, double y0)
        {
            Graph node = new Graph();
            Random rand = new Random();
            double latitude = x0;
            double longitude = y0;
            int index = 0;

            for (int x = 0; x < dimX; x++)
            {
                for (int y = 0; y < dimY; y++)
                {
                    int altitude = 20;
                    for (int z = 0; z < dimZ; z++)
                    {
                        map[x, y, z] = new Graph();
                        map[x, y, z].neighbourhood = new List<Graph>();
                        map[x, y, z].index = index;
                        map[x, y, z].latitude = latitude;
                        map[x, y, z].longitude = longitude;
                        map[x, y, z].altitude = altitude;
                        map[x, y, z].level = z;
                        map[x, y, z].passable = true;
                        if (0 == z)
                        {
                            if (rand.Next(0, 10) < 3)
                                map[x, y, z].passable = false;

                            node = FindClosest(nodes, latitude, longitude);
                            map[x, y, z].street = node.street;
                            map[x, y, z].streetEng = node.streetEng;
                            map[x, y, z].house = node.house;
                        }
                        else
                        {
                            if (map[x, y, (z - 1)].passable == false)
                            {
                                if (rand.Next(0, 10) < 3)
                                    map[x, y, z].passable = false;
                            }
                        }
                        if (0 == z)
                            altitude += 30;
                        else
                            altitude += 50;

                        index++;
                    }
                    latitude += 0.000178;
                    longitude += 0.000046;
                }
                latitude = x0;
                longitude = y0;
                latitude += (0.000025 * (x + 1));
                longitude += (0.000311 * (x + 1));
            }
            Connect(ref map, dimX, dimY, dimZ);
        }
        private void Connect(ref Graph[,,] map, int dimX, int dimY, int dimZ)
        {
            // string mapp = null;
            for (int x = 0; x < dimX; x++)
            {
                for (int y = 0; y < dimY; y++)
                {
                    for (int z = 0; z < dimZ; z++)
                    {
                        if (x != 0)
                        {
                            if (map[x - 1, y, z].passable)
                                map[x, y, z].neighbourhood.Add(map[x - 1, y, z]);
                        }
                        if (x != dimX - 1)
                        {
                            if (map[x + 1, y, z].passable)
                                map[x, y, z].neighbourhood.Add(map[x + 1, y, z]);
                        }
                        if (y != 0)
                        {
                            if (map[x, y - 1, z].passable)
                                map[x, y, z].neighbourhood.Add(map[x, y - 1, z]);
                        }
                        if (y != dimY - 1)
                        {
                            if (map[x, y + 1, z].passable)
                                map[x, y, z].neighbourhood.Add(map[x, y + 1, z]);
                        }
                        if (z != 0)
                        {
                            if (map[x, y, z - 1].passable)
                                map[x, y, z].neighbourhood.Add(map[x, y, z - 1]);
                        }
                        if (z != dimZ - 1)
                        {
                            if (map[x, y, z + 1].passable)
                                map[x, y, z].neighbourhood.Add(map[x, y, z + 1]);
                        }
                    }
                    //if (map[x, y, 3].passable)
                    //    mapp += "_";
                    //else
                    //    mapp += "B";
                }
                // mapp += "\n";
            }
            //MessageBox.Show(mapp);
        }
        private Graph FindClosest(Graph[] nodes, double lat, double lon)
        {
            Graph closest = new Graph();
            double distance = 999999;
            double tempdist = 0;
            for (int i = 0; i < nodes.Length / 4; i++)
            {
                tempdist = Haversine(nodes[i].latitude, lat, nodes[i].longitude, lon, nodes[i].altitude, 20);
                if (tempdist != 0 && tempdist < distance)
                {
                    distance = tempdist;
                    closest = nodes[i];
                }
            }
            return closest;
        }
        public Location FindClosestOnMap(Graph[,,] map, double lat, double lon)
        {
            Graph closest = new Graph();
            Location close = new Location(-1, -1, -1);
            double distance = 999999;
            double tempdist = 0;
            for (int x = 0; x < 41; x++)
            {
                for (int y = 0; y < 41; y++)
                {
                    tempdist = Haversine(map[x, y, 0].latitude, lat, map[x, y, 0].longitude, lon, map[x, y, 0].altitude, 20);
                    if (/*tempdist != 0 && */tempdist < distance)
                    {
                        distance = tempdist;
                        closest = map[x, y, 0];
                        close = new Location(x, y, 0);
                    }
                }
            }
            return close;
        }
    }
}
