using System.Collections.Generic;
using System.Linq;

namespace FSTSP_UWP
{
    public class FSTSPRouting
    {
        public static void buildUnitRoute(SquareGrid grid, List<Order> GlobalOrders, Truck truck)
        {
            var orders = new List<Order>();
            orders.AddRange(GlobalOrders);
            while (orders.Count() > 0)
            {
                if(Settings.TrafficScore == 0)
                {
                    ViewModel.adjustTruckSpeed(truck.time);
                }

                if (truck.status.Equals(Status.Ready))
                {
                    var availableDrones = truck.drones.Where(drone => drone.status.Equals(Status.Available)).ToList();
                    var routeSheets = selectDronesOrders(availableDrones, orders);

                    List<Location> truckRoute = new List<Location>();
                    truckRoute.Add(truck.currentPosition);
                    foreach (var sheet in routeSheets)
                    {
                        sheet.drone.status = Status.Preparing;
                        truckRoute.Add(sheet.meetingPoint);
                    }

                    if (routeSheets.Count() == 0)
                    {
                        truckRoute.Add(new Location(orders.First().x, orders.First().y, 0));
                    }

                    for (int i = 0; i < truckRoute.Count() - 1; i++)
                    {
                        var awaitingDrones = truck.drones.Where(drone => drone.status.Equals(Status.Awaitng) && drone.currentPosition.Equals(truck.currentPosition)).ToList();
                        Vehicle.compareAndUpdateTime(awaitingDrones, truck);
                        Drone.retrieveDrones(truck, awaitingDrones);

                        Truck.doTruckDelivery(grid, truck, truckRoute[i], truckRoute[i + 1]);
                        var deliveredOrder = orders.Find(order => order.x == truckRoute[i + 1].x && order.y == truckRoute[i + 1].y);
                        orders.Remove(deliveredOrder);

                        foreach (var sheet in routeSheets)
                        {
                            Drone.loadDrones(truck, sheet.drone);
                            Drone.doDroneDelivery(sheet, grid);
                            deliveredOrder = orders.Find(order => order.x == sheet.deliveryPoint.x && order.y == sheet.deliveryPoint.y);
                            orders.Remove(deliveredOrder);
                        }
                        routeSheets.Clear();
                    }
                    Order.sortOrders(ref orders, truck.currentPosition);
                }
            }

        }

        private static List<droneRouteSheet> selectDronesOrders(List<Drone> availableDrones, List<Order> orders)
        {
            List<droneRouteSheet> routeSheets = new List<droneRouteSheet>();
            foreach (var drone in availableDrones)
            {
                var candidateOrders = orders.Where(order => order.weight < drone.maxWeight);
                if (candidateOrders.Count() == 0) continue;

                Location orderToDeliver;
                foreach (var order in candidateOrders)
                {
                    if (routeSheets.Where(x => x.deliveryPoint.Equals(new Location(order.x, order.y, 0))
                                            || x.meetingPoint.Equals(new Location(order.x, order.y, 0))).Count() == 0)
                    {
                        orderToDeliver = new Location(order.x, order.y, 0);

                        var orderToDeliverIndex = orders.IndexOf(order);
                        var meetingPoint = orderToDeliverIndex + 1 < orders.Count() ?
                                                new Location(orders[orderToDeliverIndex + 1].x, orders[orderToDeliverIndex + 1].y, 0) : ViewModel.Depot;
                        var distance = Location.surfaceDistance(drone.currentPosition, orderToDeliver);
                        distance += Location.surfaceDistance(orderToDeliver, meetingPoint);
                        distance *= BaseConstants.PolygonSize * 1.3;

                        if (distance < drone.range)
                        {
                            var newRouteSheet = new droneRouteSheet(drone,
                                                                    drone.currentPosition,
                                                                    orderToDeliver,
                                                                    meetingPoint);
                            routeSheets.Add(newRouteSheet);
                            break;
                        }
                    }
                    else continue;
                }
            }
            return routeSheets;
        }

        public class droneRouteSheet
        {
            public Drone drone;
            public Location start;
            public Location deliveryPoint;
            public Location meetingPoint;

            public droneRouteSheet(Drone Drone, Location Start, Location DeliveryPoint, Location MeetingPoint)
            {
                drone = Drone;
                start = Start;
                deliveryPoint = DeliveryPoint;
                meetingPoint = MeetingPoint;
            }
        }
    }

}
