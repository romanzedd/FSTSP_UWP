using System.Collections.Generic;

namespace FSTSP_UWP
{
    public class BaseConstants
    {
        public static int DroneRange = 7; // kilometers
        public static int DroneSpeed = 20; //meters per second
        public static int DropDeliveryTime = 120; // seconds required to drop delivery at customer

        public const  int TruckSpeedConst = 14;
        public static int TruckSpeed = 14;//meters per second

        public static int PolygonSize = 5; //in paper it says 5, but generation for 5 is taking way too long
        public static int areaHeight = 10;

        public static int DroneRetrieveTime = 300;
        public static int DroneLoadTime = 300;

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