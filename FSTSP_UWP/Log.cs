using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSTSP_UWP
{
    public class Log
    {
        string vehicleType;
        Location location;
        string locationName;
        int time;
        Status vehicleStatus;
        string operationResult;
    
        public Log(string VehicleType, Location Location, string LocationName, int Time, Status VehicleStatus, string OperationResult)
        {
            vehicleType = VehicleType;
            location = Location;
            locationName = LocationName;
            time = Time;
            vehicleStatus = VehicleStatus;
            operationResult = OperationResult;
        }

        public static void saveToXML(string filename)
        {

        }
    }

}
