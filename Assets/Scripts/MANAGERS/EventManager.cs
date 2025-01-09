using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static Action<string> updateDeviceId;
    public static Action<int> releaseCoins;
    public static Action<int> coinPoints;
}
