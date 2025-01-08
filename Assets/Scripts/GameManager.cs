using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private string deviceId;
    public string DeviceId
    {
        get { return deviceId; }
        set { if (!string.IsNullOrEmpty(value)) { deviceId = value; } }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        GetDeviceId();

        instance = this;
        DontDestroyOnLoad(gameObject);

        ManagerRegistry.Register<GameManager>(this);
    }

    void GetDeviceId()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
UpdateDeviceId(GlobalStatic.GetDeviceIdForAndriod());
#elif !UNITY_EDITOR && UNITY_IOS
  Debug.LogError("DEVICE TYPE IOS");
#else
        Debug.LogError("DEVICE ID WAS EMPTY");
#endif
    }

    void UpdateDeviceId(string _deviceId)
    {
        if (!string.IsNullOrEmpty(_deviceId))
        {
            DeviceId = _deviceId;
            EventManager.updateDeviceId?.Invoke(_deviceId);
        }
        else
        {
            Debug.LogError("DEVICE ID WAS EMPTY");
        }
    }
}
