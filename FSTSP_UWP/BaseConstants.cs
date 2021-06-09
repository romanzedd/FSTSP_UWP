using System.Collections.Generic;

namespace FSTSP_UWP
{
    public class BaseConstants
    {
        //to control experiment parameters
        public static double LightweightOrdersPercentile = 0.3;
        enum OrdersLocations
        {
            center,
            corner,
            peripheral,
            random
        };

        public static string OrdersLocation = OrdersLocations.random.ToString();
        //________________________________

        public static int DroneRange = 7000; // kilometers
        //public static int DroneSpeed = 20; //meters per second
        public static int DroneSpeed = 800;
        public static int DropDeliveryTime = 120; // seconds required to drop delivery at customer
        public static int TruckDropDeliveryTime = 400;

        //public const  int TruckSpeedConst = 10;
        //public static int TruckSpeed = 10;//meters per second
        public const int TruckSpeedConst = 5;
        public static int TruckSpeed = 5;//meters per second

        public static int PolygonSize = 5; //in paper it says 5, but generation for 5 is taking way too long
        public static int areaHeight = 10;

        //public static int DroneRetrieveTime = 300;
        //public static int DroneLoadTime = 300;
        public static int DroneRetrieveTime = 10;
        public static int DroneLoadTime = 10;

        public static Dictionary<int, int> trafficByTime = new Dictionary<int, int>()
            {
                { 8, 6 },
                { 9, 5 },
                { 10, 4 },
                { 11, 4 },
                { 12, 4 },
                { 13, 4 },
                { 14, 4 },
                { 15, 4 },
                { 16, 4 },
                { 17, 5 },
                { 18, 6 },
                { 19, 7 },
                { 20, 4 }
            };
    }
}