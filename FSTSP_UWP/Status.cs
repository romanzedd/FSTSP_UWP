namespace FSTSP_UWP
{
    public enum Status
    {
        Offline,
        Idle,
        Ready,
        OnMission,
        Awaitng, //at the meeting point
        Available, //drone only - for a drone that is on the truck and available for a mission
        Preparing //truck only - when a truck driver prepares a drone for a mission
    }
}
