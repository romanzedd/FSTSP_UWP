using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSTSP_UWP
{
    public class BaseConstants
    {
        public static int DroneRange = 7; // kilometers
        public static int DroneSpeed = 20; //meters per second
        public static int DropDeliveryTime = 120; // seconds required to drop delivery at customer

        public static int TruckSpeed = 5;//meters per second

        public static int PolygonSize = 5; //in paper it says 5, but generation for 5 is taking way too long
        public static int areaHeight = 10;

        public static int DroneRetrieveTime = 300;
        public static int DroneLoadTime = 300;
    }
}