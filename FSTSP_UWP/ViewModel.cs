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

        /// <summary>
        /// Starts sequence to execute Travelling Salesman Problem algorithm with A* and a set of conditions
        /// </summary>
        /// <param name="areaSizeInput"></param>
        /// <param name="numberOfCustomers"></param>
        /// <returns></returns>
        public string runTSP(int areaSizeInput, int numberOfCustomers)
        {
            var areaSize = areaSizeInput * 1000 / BaseConstants.PolygonSize; //sets size in nodes
            Depot = new Location(areaSize / 2, areaSize / 2, 0);
            //await generateSpace(areaSize, 1);
            groundGrid = grid;

            generateOrders(areaSize, numberOfCustomers);

            var truck = generateTruck("singleTruck1", 0, areaSize);

            var result = string.Empty;
            try
            {
                if (!Settings.DeliveryInterval)
                {
                    Order.sortOrders(ref orders, Depot);
                    doTruck(Depot, orders, truck);
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
                            truck.time = intervals.Intervals.ToArray()[i].Value.start;

                        var timedOrders = timeClusteredOrders.ElementAt(i);
                        Order.sortOrders(ref timedOrders, truck.currentPosition);
                        doTruck(truck.currentPosition, timedOrders, truck);
                    }
                }

            }
            catch(Exception ex)
            {
                return "Unhandled exception during path reconstruction\nPlease try again";
            }

            result = ComposeResult(truck);

            return result;
        }

        /// <summary>
        /// Starts sequence to execute Flying Sidekick Travelling Salesman Problem algorithm
        /// </summary>
        /// <param name="areaSizeInput"></param>
        /// <param name="numberOfCustomers"></param>
        /// <returns></returns>
        public string runFSTSP(int areaSizeInput, int numberOfCustomers)
        {
            var areaSize = areaSizeInput * 1000 / BaseConstants.PolygonSize; //sets size in nodes
            Depot = new Location(areaSize / 2, areaSize / 2, 0);

            var weatherConditions = checkWeatherConditions();
            if (weatherConditions == string.Empty)
            {
                weatherConditions = "\nWeather conditions OK";
            }
            else
            {
                return weatherConditions;
            }

            generateOrders(areaSize, numberOfCustomers);
            var truck = generateTruck("truck1", 3, areaSize);
            try
            {
                performDelivery(truck);
            }
            catch (Exception ex) 
            {
                return "Unhandled exception during path reconstruction\nPlease try again";
            };

            var output = ComposeResult(truck);

            return string.Concat(weatherConditions, output);
        }

        /// <summary>
        /// Performs delivery depending on wether deliveries are scheduled or not, based on settings value
        /// </summary>
        /// <param name="truck"></param>
        private void performDelivery(Truck truck)
        {
            if (Settings.TrafficScore != 0)
                adjustTruckSpeed();
            

            if (!Settings.DeliveryInterval)
            {
                Order.sortOrders(ref orders, Depot);

                var ordersString = string.Empty;
                foreach (var o in orders)
                    ordersString += $"{o.address}\t--\t{o.x}.{o.y}\n";

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
        }

        private void adjustTruckSpeed()
        {
            double doubleSpeed = BaseConstants.TruckSpeedConst * (1 - ((double)Settings.TrafficScore / 20));
            BaseConstants.TruckSpeed = (int)Math.Ceiling(doubleSpeed);
        }

        public static void adjustTruckSpeed(int currentTime)
        {
            var hour = TimeSpan.FromSeconds(currentTime).Hours;
            var trafficScore = BaseConstants.trafficByTime.GetValueOrDefault(hour);

            double doubleSpeed = BaseConstants.TruckSpeedConst * (1 - ((double)trafficScore / 20));
            BaseConstants.TruckSpeed = (int)Math.Ceiling(doubleSpeed);
        }

        private string checkWeatherConditions()
        {
            var result = string.Empty;

            if (Settings.Temperature <= -10 || Settings.Temperature >= 50)
                result += "\n\tTemperature is out of operable range";
            if (Settings.PrecipitationType.Equals("Showers") || Settings.PrecipitationType.Equals("Snowfall"))
            {
                if ((Settings.Temperature >= -5 && Settings.PrecipitationVolume >= 40) ||
                    (Settings.Temperature <= -5 && Settings.PrecipitationVolume >= 60))
                    result += "\n\tHeavy precipation probability";
            }
            else if (Settings.PrecipitationType.Equals("Hail"))
                result += "\n\tHail";
            if (Settings.Wind > 10)
                result += "\n\tStrong wind";
            if (Settings.GAIndex > 3)
                result += "\n\tGeomagnetic activity index is high";

            if (result != string.Empty)
                result = string.Concat("\nDrone delivery is not possible due to next reasons:", result);

            return result;
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
        private string doTruck(Location depot, List<Order> truckOrders, Truck truck)
        {
            if (truckOrders is null) return string.Empty;
            if (Settings.TrafficScore != 0)
                adjustTruckSpeed();

            List<List<Location>> truckPaths = new List<List<Location>>();
            List<Location> path;

            AStarSearch astar = new AStarSearch(groundGrid, depot, new Location(truckOrders.First().x, truckOrders.First().y, 0));
            path = astar.ReconstructPath(depot, new Location(truckOrders.First().x, truckOrders.First().y, 0), astar.cameFrom);
            truckPaths.Add(path);

            truck.currentPosition = new Location(truckOrders.First().x, truckOrders.First().y, 0);
            var pathLength = path.Count * BaseConstants.PolygonSize;
            var deliveryTime = pathLength / BaseConstants.TruckSpeed + BaseConstants.DropDeliveryTime;
            truck.time += deliveryTime;
            truck.status = Status.OnMission;

            truck.log.Add(new Log(truck.id,
                  truck.currentPosition,
                  orders.Where(x => (x.x == truck.currentPosition.x && x.y == truck.currentPosition.y)).First().address,
                  truck.time,
                  truck.status,
                  "Delivery finished"));

            for (int i = 0; i < truckOrders.Count - 1; i++)
            {
                if (Settings.TrafficScore != 0)
                    adjustTruckSpeed(truck.time);

                var start = new Location(truckOrders[i].x, truckOrders[i].y, 0);
                var deliveryLocation = new Location(truckOrders[i + 1].x, truckOrders[i + 1].y, 0);

                astar = new AStarSearch(groundGrid, start, deliveryLocation);
                path = astar.ReconstructPath(start, deliveryLocation, astar.cameFrom);
                truckPaths.Add(path);

                pathLength = path.Count * BaseConstants.PolygonSize;
                deliveryTime = pathLength / BaseConstants.TruckSpeed + BaseConstants.DropDeliveryTime;

                truck.currentPosition = deliveryLocation;
                truck.time += deliveryTime;
                truck.status = Status.OnMission;

                truck.log.Add(new Log(truck.id,
                      truck.currentPosition,
                      orders.Where(x => (x.x == truck.currentPosition.x && x.y == truck.currentPosition.y)).First().address,
                      truck.time,
                      truck.status,
                      "Delivery finished"));
            }

            //astar = new AStarSearch(groundGrid, new Location(truckOrders.Last().x, truckOrders.Last().y, 0), Depot);
            //path = astar.ReconstructPath(new Location(truckOrders.Last().x, truckOrders.Last().y, 0), Depot, astar.cameFrom);
            //truckPaths.Add(path);

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

            log.AddRange(truck.log.Where(x => x.operationResult.Equals("Delivery finished")));
            foreach (var drone in truck.drones)
            {
                log.AddRange(drone.log.Where(x => x.operationResult.Equals("Delivery finished")));
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
