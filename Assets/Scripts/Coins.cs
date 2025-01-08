using Sirenix.OdinInspector;
using UnityEngine;

public class Coins : MonoBehaviour
{
    [SerializeField] private PoolManagerGen poolManager;
    [SerializeField] private int _coinId = -1;
    [SerializeField] int cointPoint = 5;

    private void Awake()
    {
        poolManager = ManagerRegistry.Get<PoolManagerGen>();
    }
    public void InitCoins(int id)
    {
        //Debug.Log("INIT COINS");
        _coinId = id;
    }

    [Button("Release Coin")]
    public void ReleaseCoin()
    {
        if (poolManager == null)
        {
            Debug.Log("POOL MANAGER WAS NULL");
            return;
        }
        poolManager.ReturnObject<Coins>(PoolManagerKeys.COINS, this);
        EventManager.releaseCoins?.Invoke(_coinId);
        _coinId = -1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //Debug.Log($"Trigger Enter: {other.tag}");
            EventManager.coinPoints?.Invoke(cointPoint);
            ReleaseCoin();
        }
    }
}
