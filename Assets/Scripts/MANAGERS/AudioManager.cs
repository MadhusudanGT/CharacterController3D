using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private void Awake()
    {
        ManagerRegistry.Register<AudioManager>(this);
    }
}
