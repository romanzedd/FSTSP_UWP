using System;
using System.Collections.Generic;
using System.Linq;

namespace FSTSP_UWP
{
    public class Truck : Vehicle
    {
        public readonly List<Drone> drones;

        public Truck(string Id, Location Depot, List<Drone> Drones)
        {
            id = Id;
            drones = Drones;
            currentPosition = Depot; // construction is always in the Depot
            status = Status.Ready;
            destination = new Location(-1, -1, -1);
            time = 28800;
            fulfilledOrders = new List<Order>();
        }

        public static Truck generateTruck(string truckID, int numberOfDrones, int areaSize, Location Depot)
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

        public static void doTruckDelivery(SquareGrid grid, Truck truck, Location start, Location deliveryLocation)
        {
            truck.status = Status.OnMission;
            truck.log.Add(new Log(truck.id,
                                  truck.currentPosition,
                                  ViewModel.orders.Where(x => (x.x == truck.currentPosition.x && x.y == truck.currentPosition.y)).FirstOrDefault()?.address,
                                  truck.time,
                                  truck.status,
                                  "success"));

            var path = simpleRoute(grid, start, deliveryLocation);
            var pathLength = path.Count * BaseConstants.PolygonSize;
            var deliveryTime = pathLength / BaseConstants.TruckSpeed + BaseConstants.DropDeliveryTime;

            truck.time += deliveryTime;
            truck.status = Status.Ready;
            truck.currentPosition = deliveryLocation;
            var availableDrones = truck.drones.Where(x => x.status.Equals(Status.Available)).ToList();
            foreach (var drone in availableDrones)
            {
                drone.currentPosition = truck.currentPosition;
            }
            truck.log.Add(new Log(truck.id,
                                  truck.currentPosition,
                                  ViewModel.orders.Where(x => (x.x == truck.currentPosition.x && x.y == truck.currentPosition.y)).FirstOrDefault()?.address,
                                  truck.time,
                                  truck.status,
                                  "Delivery finished"));
        }

        public static void adjustTruckSpeed()
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

        public static string truckStatusUpdate(List<List<Location>> truckPaths, int TruckTime)
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
