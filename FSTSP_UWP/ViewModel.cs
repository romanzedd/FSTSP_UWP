using System;
using System.Collections.Generic;
using System.Linq;
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
        public static List<Order> orders;

        public List<Location> ThePath;
        public static Location Depot;
        public static int DroneTime = 28800;
        public static int TruckTime = 28800;

        public int AreaSize = 0;

        public async Task<string> runTSP(int areaSizeInput, int numberOfCustomers)
        {
            var areaSize = areaSizeInput * 1000 / BaseConstants.PolygonSize; //sets size in nodes
            Depot = new Location(areaSize / 2, areaSize / 2, 0);
            await generateSpace(areaSize, 1);
            groundGrid = grid;
            generateOrders(areaSize, numberOfCustomers);
            var result = doTruck(orders);
            return result;
        }

        public string runFSTSP(int areaSizeInput, int numberOfCustomers)
        {
            var areaSize = areaSizeInput * 1000 / BaseConstants.PolygonSize; //sets size in nodes
            Depot = new Location(areaSize / 2, areaSize / 2, 0);

            generateOrders(areaSize, numberOfCustomers);
            var truck = generateTruck("truck1", 3, areaSize);

            if (!Settings.DeliveryInterval)
            {
                Order.sortOrders(ref orders, Depot);
                FSTSPRouting.buildUnitRoute(grid, orders, truck);
            }
            else
            {
                var timeClusteredOrders = new List<List<Order>>();
                var intervals = new DeliveryIntervals();
                foreach (var interval in intervals.Intervals)
                {
                    var ordersInInterval = orders.Where(x => x.dueTime.Equals(interval.Key)).ToList();
                    timeClusteredOrders.Add(ordersInInterval);
                }

                for (int i = 0; i < timeClusteredOrders.Count(); i++)
                {
                    if (truck.time < intervals.Intervals.ToArray()[i].Value.start)
                    {
                        truck.status = Status.Idle;
                        truck.log.Add(new Log(truck.id,
                                              truck.currentPosition,
                                              orders.Where(x => (x.x == truck.currentPosition.x && x.y == truck.currentPosition.y)).First().address,
                                              truck.time,
                                              truck.status,
                                              "success"));
                        truck.time = intervals.Intervals.ToArray()[i].Value.start;
                        truck.status = Status.Ready;

                        foreach (var drone in truck.drones)
                        {
                            drone.status = Status.Idle;
                            drone.log.Add(new Log(drone.id,
                                                  drone.currentPosition,
                                                  orders.Where(x => (x.x == truck.currentPosition.x && x.y == truck.currentPosition.y)).First().address,
                                                  drone.time,
                                                  drone.status,
                                                  "success"));
                            drone.time = intervals.Intervals.ToArray()[i].Value.start;
                            drone.status = Status.Available;
                        }

                    }

                    var timedOrders = timeClusteredOrders.ElementAt(i);
                    Order.sortOrders(ref timedOrders, truck.currentPosition);
                    FSTSPRouting.buildUnitRoute(grid, timedOrders, truck);
                }
            }

            var output = ComposeResult(truck);

            return output;
        }

        public async Task<string> generateSpace(int areaSizeInput)
        {
            var areaSize = areaSizeInput * 1000 / BaseConstants.PolygonSize;
            grid = new SquareGrid(areaSize, areaSize, BaseConstants.areaHeight);

            await Task.Run(() =>
            {
                GridGeneration.fillGrid(grid, areaSize, BaseConstants.areaHeight);
            });


            groundGrid = new SquareGrid(areaSize, areaSize, 1);
            groundGrid.walls = grid.walls.Where(location => location.z == 0).ToHashSet();

            return $"Space of {areaSizeInput} km2 ({areaSize * areaSize * BaseConstants.areaHeight} polygons) generated successfully\n";
        }

        public async Task<string> generateSpace(int areaSizeInput, int areaHeight)
        {
            var areaSize = areaSizeInput * 1000 / BaseConstants.PolygonSize;
            grid = new SquareGrid(areaSize, areaSize, areaHeight);
            await Task.Run(() =>
            {
                GridGeneration.fillGrid(grid, areaSize, areaHeight);
            });
            return $"Space of {areaSizeInput} km2 ({areaSize * areaSize * areaHeight} polygons) generated successfully\n";
        }

        public static string generateOrders(int areaSize, int ordersCount)
        {
            if (ordersCount < 1)
                ordersCount = 15;

            if (Settings.DeliveryInterval)
            {
                orders = Order.generateOrders(grid, Depot, ordersCount, areaSize, true);
            }
            else
            {
                orders = Order.generateOrders(grid, Depot, ordersCount, areaSize);
            }

            return $"{ordersCount} orders generated successfully\n";
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
        private string doTruck(List<Order> truckOrders)
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

        private string ComposeResult(Truck truck)
        {
            var result = string.Empty;
            var log = new List<Log>();

            log.AddRange(truck.log);
            foreach (var drone in truck.drones)
            {
                log.AddRange(drone.log);
            }

            var sortedLog = log.OrderBy(x => x.time).ToList();
            foreach (var entry in sortedLog)
            {
                result += "\n" + entry.Print();
            }

            return result;
        }

    }
}
