using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSTSP_UWP
{
    public class Drone : Vehicle
    {
        public readonly int range; //metres
        public readonly int maxWeight;

        public Drone(string Id, int Range, int MaxWeight, Location Depot)
        {
            id = Id;
            range = Range;
            maxWeight = MaxWeight;
            currentPosition = Depot; // construction is always in the Depot
            status = Status.Available;
            destination = new Location(-1, -1, -1);
            time = 28800;
            fulfilledOrders = new List<Order>();
            //log = string.Empty;
        }

        public static void loadDrones(Truck truck, Drone drone)
        {
            drone.time += BaseConstants.DroneLoadTime;
            truck.time += BaseConstants.DroneLoadTime;
        }
        public static void retrieveDrones(Truck truck, List<Drone> drones)
        {
            foreach (var drone in drones)
            {
                drone.time += BaseConstants.DroneLoadTime;
                drone.status = Status.Available;
                truck.time += BaseConstants.DroneLoadTime;
            }
        }

        public static void doDroneDelivery(FSTSPRouting.droneRouteSheet routeSheet, SquareGrid grid)
        {
            List<Location> path = new List<Location>();

            //routeSheet.drone.time += BaseConstants.DroneLoadTime;
            routeSheet.drone.status = Status.OnMission;

            path.AddRange(simpleRoute(grid, routeSheet.start, routeSheet.deliveryPoint));
            var pathLength = path.Count * BaseConstants.PolygonSize;
            var deliveryTime = pathLength / BaseConstants.DroneSpeed + BaseConstants.DropDeliveryTime;
            routeSheet.drone.currentPosition = routeSheet.deliveryPoint;
            routeSheet.drone.time += deliveryTime;
            routeSheet.drone.log.Add(new Log(routeSheet.drone.id,
                                             routeSheet.drone.currentPosition,
                                             ViewModel.orders.Where(x => (x.x == routeSheet.drone.currentPosition.x && x.y == routeSheet.drone.currentPosition.y)).FirstOrDefault()?.address,
                                             routeSheet.drone.time,
                                             routeSheet.drone.status,
                                             "success"));

            path.Clear();
            path.AddRange(simpleRoute(grid, routeSheet.deliveryPoint, routeSheet.meetingPoint));
            pathLength = path.Count * BaseConstants.PolygonSize;
            deliveryTime = pathLength / BaseConstants.DroneSpeed;
            routeSheet.drone.currentPosition = routeSheet.meetingPoint;
            routeSheet.drone.time += deliveryTime;
            routeSheet.drone.status = Status.Awaitng;
            routeSheet.drone.log.Add(new Log(routeSheet.drone.id,
                                             routeSheet.drone.currentPosition,
                                             ViewModel.orders.Where(x => (x.x == routeSheet.drone.currentPosition.x && x.y == routeSheet.drone.currentPosition.y)).FirstOrDefault()?.address,
                                             routeSheet.drone.time,
                                             routeSheet.drone.status,
                                             "success"));



        }
    }
}
