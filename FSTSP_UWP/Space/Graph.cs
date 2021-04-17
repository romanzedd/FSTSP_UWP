using System;
using System.Collections.Generic;

namespace FSTSP_UWP
{
    public class Map
    {
        public List<Graph> Nodes { get; set; } = new List<Graph>();

        public Graph StartNode { get; set; }

        public Graph EndNode { get; set; }

        public List<Graph> ShortestPath { get; set; } = new List<Graph>();
    }
    public class Graph
    {
        public int index;
        public string street;
        public string streetEng;
        public string house;
        public double latitude;
        public double longitude;
        public double altitude;
        public int level;
        public bool passable;
        //public Graph[] neighbours; //если что-то сломалось, попробуй раскоментить
        public List<Graph> neighbourhood;
        public List<Rib> connections { get; set; } = new List<Rib>();
        public Guid Id;
        public string Name;
        public Point Point;
        public double? MinCostToStart { get; set; }
        public Graph NearestToStart { get; set; }
        public bool Visited { get; set; }
        public double StraightLineDistanceToEnd { get; set; }

        public double StraightLineDistanceTo(Graph end)
        {
            routing route = new routing();
            return route.Haversine(Point.X, end.Point.X, Point.Y, end.Point.Y, Point.Z, end.Point.Z);
        }

        //constructor
        public Graph() { }
        public Graph(int _index, string _street, string _streetEng, string _house, double _latitude, double _longitude, double _altitude, int _level)
        {
            index = _index;
            street = _street;
            streetEng = _streetEng;
            house = _house;
            latitude = _latitude;
            longitude = _longitude;
            altitude = _altitude;
            level = _level;
            neighbourhood = new List<Graph>();
            Point = new Point(latitude, longitude, altitude);
            Name = _street + " " + _house;

            byte[] bytes = new byte[16];
            BitConverter.GetBytes(index).CopyTo(bytes, 0);
            Id = new Guid(bytes);

        }
        public Graph findNode(string _address, Graph[] node)
        {
            int i;
            string[] linkText = _address.Split(',');
            string[] address = linkText[0].Split(null);
            for (i = 1; i < address.Length - 1; i++)
            {
                address[0] += " " + address[i];
            }
            for (i = 0; i < node.Length; i++)
            {
                if ((node[i].street == address[0] && node[i].house == address[address.Length - 1]) || (node[i].streetEng == address[0] && node[i].house == address[address.Length - 1]))
                    return node[i];
            }
            return node[0];
        }
        public Location findNode(string _address, Graph[,,] map, int dimX, int dimY, int dimZ)
        {
            string[] linkText = _address.Split(',');
            string[] address = linkText[0].Split(null);
            for (int i = 1; i < address.Length - 1; i++)
            {
                address[0] += " " + address[i];
            }
            for (int x = 0; x < dimX; x++)
            {
                for (int y = 0; y < dimY; y++)
                {
                    for (int z = 0; z < dimZ; z++)
                    {
                        if ((map[x, y, z].street == address[0] && map[x, y, z].house == address[address.Length - 1]) || (map[x, y, z].streetEng == address[0] && map[x, y, z].house == address[address.Length - 1]))
                            return new Location(x, y, z);
                    }
                }
            }

            return new Location(-1, -1, -1);
        }

    }
    public class Rib
    {
        public double Length { get; set; }
        public double Cost { get; set; }
        public Graph ConnectedNode { get; set; }

    }
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Point(double _x, double _y, double _z)
        {
            X = _x;
            Y = _y;
            Z = _z;
        }
    }
}
