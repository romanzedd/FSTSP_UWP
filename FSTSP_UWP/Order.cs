using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSTSP_UWP
{
    public class Order
    {
        public int x;
        public int y;
        public int weight;
        public bool isDroneFriendly;
        public string address;
        public int dueTime;

        private string[] streetNames = {"������� ��������",
                                        "��������",
                                        "1905 ����",
                                        "9 �����",
                                        "���������",
                                        "���������� ����������",
                                        "������",
                                        "������",
                                        "������ ���������",
                                        "���� ���������",
                                        "��������",
                                        "������",
                                        "���������� ��������",
                                        "�������������",
                                        "����������",
                                        "������"};

        public Order(int X, int Y, bool droneFriendly, int Weight, int dueTime = 0)
        {
            x = X;
            y = Y;
            isDroneFriendly = droneFriendly;
            weight = Weight;
            if (dueTime != 0) this.dueTime = dueTime;

            Random rnd = new Random();
            var street = streetNames[rnd.Next(streetNames.Length)];
            var building = rnd.Next(150);
            address = street + " - " + building.ToString(); 
        }

        public static List<Order> generateOrders(SquareGrid grid, Location Depot, int ordersCount, int areaSize)
        {
            Random rnd = new Random();
            List<Order> ordersList = new List<Order>();
            while (ordersCount > 0)
            {
                var x = rnd.Next(areaSize);
                var y = rnd.Next(areaSize);
                var isWall = true;
                while (isWall)
                {
                    if (grid.walls.Contains(new Location(x, y, 0)))
                    {
                        x = rnd.Next(areaSize);
                        y = rnd.Next(areaSize);
                    }
                    else
                        isWall = false;
                }

                var distanceFromDepot = Math.Sqrt(Math.Pow((x - Depot.x), 2.0) + Math.Pow((y - Depot.y), 2.0)) * BaseConstants.PolygonSize / 1000;

                bool isDroneFriendly = false;
                if (rnd.Next(2) == 0)
                {

                    if (distanceFromDepot < BaseConstants.DroneRange)
                        isDroneFriendly = true;
                }

                ordersList.Add(new Order(x, y, 
                                         isDroneFriendly,
                                         rnd.Next(100, 6000)));
                ordersCount--;
            }
            return ordersList;
        }

        public static void sortOrders(ref List<Order> orders, Location depot)
        {
            List<Order> sorted = new List<Order>();

            Order tempOrder = findClosestOrder(depot, orders);
            if (tempOrder is null) return;
            sorted.Add(tempOrder);
            orders.Remove(tempOrder);

            while (orders.Count() != 0)
            {
                tempOrder = findClosestOrder(new Location(sorted.Last().x, sorted.Last().y, 0), orders);
                sorted.Add(tempOrder);
                orders.Remove(tempOrder);
            }
            orders = sorted;
        }

        private static Order findClosestOrder(Location current, List<Order> orders)
        {
            var closestOrder = orders.FirstOrDefault();
            if (closestOrder is null) return null;
            var dist = distance(current.x, current.y, closestOrder.x, closestOrder.y);

            foreach(var order in orders)
            {
                var tempDist = distance(current.x, current.y, order.x, order.y);
                if (tempDist < dist)
                {
                    dist = tempDist;
                    closestOrder = order;
                }
            }

            return closestOrder;
        }

        private static double distance(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }
    }
}
