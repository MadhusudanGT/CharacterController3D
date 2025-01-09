using UnityEngine;

public static class GlobalStatic
{
    public static string GetDeviceIdForAndriod()
    {
        return SystemInfo.deviceUniqueIdentifier;        
    }
}
