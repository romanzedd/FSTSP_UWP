using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSTSP_UWP
{
    public class ViewModel
    {
        public Map FormMap { get; set; }
        public Graph[] nodes;
        public Graph[,,] map;
        public static SquareGrid grid;
        public static SquareGrid groundGrid;

        public List<Location> ThePath;
        public static Location Depot;
        public static int DroneTime = 28800;
        public static int TruckTime = 28800;

        public int AreaSize = 0;

        public string runTSP(string areaSizeInput, string numberOfCustomers)
        {
            var areaSize = Int16.Parse(areaSizeInput) * 1000 / BaseConstants.PolygonSize; //sets size in nodes
            Depot = new Location(areaSize / 2, areaSize / 2, 0);
            generateSpace(areaSize, 1);
            groundGrid = grid;
            var orders = generateOrders(areaSize, numberOfCustomers);
            var result = doTruck(orders);
            return result;
        }

        public string runFSTSP(string areaSizeInput, string numberOfCustomers)
        {
            var areaSize = Int16.Parse(areaSizeInput) * 1000 / BaseConstants.PolygonSize; //sets size in nodes
            Depot = new Location(areaSize / 2, areaSize / 2, 0);

            var result = generateSpace(areaSize);

            var orders = generateOrders(areaSize, numberOfCustomers);
            Order.sortOrders(ref orders, Depot);
            //var truckOrders = orders.Where(x => x.isDroneFriendly == false);
            //var droneOrders = orders.Where(x => x.isDroneFriendly == true);

            var truck = generateTruck("truck1", 3, areaSize);
            FSTSPRouting.buildUnitRoute(grid, orders, truck);

            return "\nOK";
            //doDrone(droneOrders.ToList());
            //doTruck(truckOrders.ToList());
        }

        public string generateSpace(int areaSize)
        {
            grid = new SquareGrid(areaSize, areaSize, BaseConstants.areaHeight);
            GridGeneration.fillGrid(grid, areaSize, BaseConstants.areaHeight);

            groundGrid = new SquareGrid(areaSize, areaSize, 1);
            groundGrid.walls = grid.walls.Where(location => location.z == 0).ToHashSet();

            return $"Space of {areaSize * BaseConstants.PolygonSize / 1000} km2 ({areaSize * areaSize * BaseConstants.areaHeight} polygons) generated successfully\n";
        }

        public string generateSpace(int areaSize, int areaHeight)
        {
            grid = new SquareGrid(areaSize, areaSize, areaHeight);
            GridGeneration.fillGrid(grid, areaSize, areaHeight);
            return $"Space of {areaSize * BaseConstants.PolygonSize / 1000} km2 ({areaSize * areaSize} polygons) generated successfully\n";
        }

        public static List<Order> generateOrders(int areaSize, string numberOfCustomers)
        {
            List<Order> orders = new List<Order>();

            short ordersCount;
            if (!Int16.TryParse(numberOfCustomers, out ordersCount))
                ordersCount = 15;

            orders = Order.generateOrders(grid, Depot, ordersCount, areaSize);
            //outputTextBox.Text += $"{ordersCount} orders generated successfully\n";

            return orders;
        }

        public static Truck generateTruck(string truckID, int numberOfDrones, int areaSize)
        {
            var areaLength = areaSize * 1000;

            var truck = new Truck(truckID, Depot, new List<Drone>());
            for (int i = 0; i < numberOfDrones; i++)
            {
                var drone = new Drone(truckID + "_drone" + (i + 1).ToString(),
                                      Convert.ToInt32(areaLength),
                                      2500,
                                      Depot);
                truck.drones.Add(drone);
            }

            return truck;
        }

        private string doDrone(List<Order> droneOrders)
        {
            List<List<Location>> dronePaths = new List<List<Location>>();
            foreach (var order in droneOrders)
            {
                List<Location> path = new List<Location>();
                AStarSearch astar = new AStarSearch(grid, Depot, new Location(order.x, order.y, 0));
                path = astar.ReconstructPath(Depot, new Location(order.x, order.y, 0), astar.cameFrom);
                var returnPath = path;
                returnPath.Reverse();
                path.AddRange(returnPath);
                dronePaths.Add(path);
            }

            return droneStatusUpdate(dronePaths);
        }
        private string  doTruck(List<Order> truckOrders)
        {
            List<List<Location>> truckPaths = new List<List<Location>>();
            List<Location> path;

            AStarSearch astar = new AStarSearch(groundGrid, Depot, new Location(truckOrders.First().x, truckOrders.First().y, 0));
            path = astar.ReconstructPath(Depot, new Location(truckOrders.First().x, truckOrders.First().y, 0), astar.cameFrom);
            truckPaths.Add(path);

            for (int i = 0; i < truckOrders.Count - 1; i++)
            {

                astar = new AStarSearch(groundGrid,
                                        new Location(truckOrders[i].x, truckOrders[i].y, 0),
                                        new Location(truckOrders[i + 1].x, truckOrders[i + 1].y, 0));
                path = astar.ReconstructPath(new Location(truckOrders[i].x, truckOrders[i].y, 0),
                                             new Location(truckOrders[i + 1].x, truckOrders[i + 1].y, 0),
                                             astar.cameFrom);
                truckPaths.Add(path);
            }

            astar = new AStarSearch(groundGrid, new Location(truckOrders.Last().x, truckOrders.Last().y, 0), Depot);
            path = astar.ReconstructPath(new Location(truckOrders.Last().x, truckOrders.Last().y, 0), Depot, astar.cameFrom);
            truckPaths.Add(path);

            return truckStatusUpdate(truckPaths);
        }

        private string droneStatusUpdate(List<List<Location>> dronePaths)
        {
            var output = string.Empty;
            foreach (var path in dronePaths)
            {
                var currentTime = TimeSpan.FromSeconds(DroneTime);
                output += $"[{currentTime.ToString(@"hh\:mm\:ss\:fff")}] Drone picked parcel and left the depot\n";

                DroneTime += (path.Count / 2) * BaseConstants.PolygonSize / BaseConstants.DroneSpeed;
                currentTime = TimeSpan.FromSeconds(DroneTime);
                output += $"[{currentTime.ToString(@"hh\:mm\:ss\:fff")}] Drone arrived to a client\n";

                DroneTime += BaseConstants.DropDeliveryTime;
                currentTime = TimeSpan.FromSeconds(DroneTime);
                output += $"[{currentTime.ToString(@"hh\:mm\:ss\:fff")}] Drone dropped parcel and is returning to the depot\n";

                DroneTime += (path.Count / 2) * BaseConstants.PolygonSize / BaseConstants.DroneSpeed;
                currentTime = TimeSpan.FromSeconds(DroneTime);
                output += $"[{currentTime.ToString(@"hh\:mm\:ss\:fff")}] Drone arrived to the depot\n";

                DroneTime += BaseConstants.DropDeliveryTime;
            }

            return output;
        }

        private string truckStatusUpdate(List<List<Location>> truckPaths)
        {
            var output = string.Empty;
            var currentTime = TimeSpan.FromSeconds(TruckTime);
            output += $"[{currentTime.ToString(@"hh\:mm\:ss\:fff")}] Truck picked parcels and left the depot\n";

            foreach (var path in truckPaths)
            {
                if (path == truckPaths.Last())
                {
                    TruckTime += path.Count * BaseConstants.PolygonSize / BaseConstants.TruckSpeed;
                    currentTime = TimeSpan.FromSeconds(TruckTime);
                    output += $"[{currentTime.ToString(@"hh\:mm\:ss\:fff")}] Truck arrived to the depot\n";
                    continue;
                }

                TruckTime += path.Count * BaseConstants.PolygonSize / BaseConstants.TruckSpeed;
                currentTime = TimeSpan.FromSeconds(TruckTime);
                output += $"[{currentTime.ToString(@"hh\:mm\:ss\:fff")}] Truck arrived to a client\n";

                TruckTime += BaseConstants.DropDeliveryTime;
                currentTime = TimeSpan.FromSeconds(TruckTime);
                output += $"[{currentTime.ToString(@"hh\:mm\:ss\:fff")}] Truck dropped parcel and is heading to the next client\n";
            }
            return output;
        }

    }

}
