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
        public int time;
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

        public string Print()
        {
            var result = string.Empty;
            result += $"\nVehicle type: {this.vehicleType}";
            result += $"\n\tLocation: x = {this.location.x.ToString()}, y = {this.location.y.ToString()}, z = {this.location.z.ToString()} - ";
            result += $"{locationName}";
            var currentTime = TimeSpan.FromSeconds(this.time);
            result += $"\n\tTime: {currentTime.ToString(@"hh\:mm\:ss\:fff")}";
            result += $"\n\tVehicle Status: {this.vehicleStatus.ToString()}";
            result += $"\n\tOperation result: {this.operationResult}";
            return result;
        }

        public static void saveToXML(string filename)
        {

        }
    }

}
