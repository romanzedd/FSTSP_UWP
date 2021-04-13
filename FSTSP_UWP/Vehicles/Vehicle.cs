using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSTSP_UWP
{
    public class Vehicle
    {
        public string id;
        public Status status = Status.Available;
        public Location currentPosition;
        public Location destination = new Location(-1, -1, -1);
        public int time = 28800;
        public List<Order> fulfilledOrders = new List<Order>();
        public string log = string.Empty;

        
        public static void compareAndUpdateTime(List<Drone> drones, Truck truck)
        {
            var maxTime = 0;
            List<Vehicle> vehicles = new List<Vehicle>();
            vehicles.Add(truck);
            foreach(var drone in drones)
            {
                vehicles.Add(drone);
                if (drone.time > maxTime) 
                    maxTime = drone.time;
            }
            if (truck.time > maxTime) maxTime = truck.time;

            foreach(var vehicle in vehicles.Where(x => !(x.time == maxTime)))
            {
                updateTime(vehicle, maxTime);
            }
        }
        public static void updateTime(Vehicle targetVehicle, int actualTime)
        {
            targetVehicle.log += $"\n-Waited for {(actualTime - targetVehicle.time).ToString(@"hh\:mm\:ss\")}";
            targetVehicle.time = actualTime;
        }

        public static List<Location> simpleRoute(SquareGrid grid, Location start, Location finish)
        {
            List<Location> path = new List<Location>();
            AStarSearch astar = new AStarSearch(grid, start, finish);
            path = astar.ReconstructPath(start, finish, astar.cameFrom);

            return path;
        }
    }
}
