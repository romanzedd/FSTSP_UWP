using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSTSP_UWP
{
    public class DeliveryInterval
    {
        public int start;
        public int end;

        public DeliveryInterval(int start, int end)
        {
            this.start = start;
            this.end = end;
        }
    }

    public class DeliveryIntervals
    {
        public Dictionary<string, DeliveryInterval> Intervals = new Dictionary<string, DeliveryInterval>();
        
        public DeliveryIntervals()
        {
            Intervals.Add("8:00 - 11:00", new DeliveryInterval(28800, 39600));
            Intervals.Add("11:00 - 14:00", new DeliveryInterval(39600, 50400));
            Intervals.Add("14:00 - 17:00", new DeliveryInterval(50400, 61200));
            Intervals.Add("17:00 - 20:00", new DeliveryInterval(61200, 72000));
        }
    }

    enum StreetNames
    {
        Krasnyi_Prospekt,
        Lenina,
        Kuibysheva,
        Gogolya,
        Gorkogo,
        Oktyabrskaya,
        Sovetskaya,
        Ippodromskaya,
        Nemirovicha_Danchenko,
        Novogodnyaya,
        Geodezicheskaya,
        Blyukhera,
        Marksa
    }

    public class Order
    {
        public int x;
        public int y;
        public int weight;
        public string address;
        public string dueTime;

        

        public Order(int X, int Y, int Weight, int dueTime = 0)
        {
            x = X;
            y = Y;
            weight = Weight;
            if (dueTime != 0)
            {
                var intervals = new DeliveryIntervals();
                this.dueTime = intervals.Intervals.ToArray()[dueTime - 1].Key;
            }

            Random rnd = new Random();
            var streets = Enum.GetNames(typeof(StreetNames));
            var street = streets[rnd.Next(streets.Length)];
            var building = rnd.Next(150);
            address = street + " - " + building.ToString(); 
        }

        public static List<Order> generateOrders(SquareGrid grid, Location Depot, int ordersCount, int areaSize, bool intervals = false)
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

                ordersList.Add(new Order(x, y, rnd.Next(100, 6000), rnd.Next(1, 4)));
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
