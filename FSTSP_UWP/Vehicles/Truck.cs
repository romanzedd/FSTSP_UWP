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
            //log = string.Empty;
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
                                  "success"));
        }
    }
}
