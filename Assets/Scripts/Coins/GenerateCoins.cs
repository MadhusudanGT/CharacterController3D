using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using static Unity.Cinemachine.CinemachineSplineRoll;

public class GenerateCoins : MonoBehaviour
{
    [SerializeField] private PoolManagerGen poolManager;

    public List<Transform> spawnPoints;
    private List<bool> usedSpawnPoints;
    public float checkInterval = 2f;

    public int initialCoinsToSpawn = 5;
    public Transform _parent;

    private void OnEnable()
    {
        EventManager.releaseCoins += ReleasedCoins;
    }

    private void OnDisable()
    {
        EventManager.releaseCoins -= ReleasedCoins;
    }

    void ReleasedCoins(int index)
    {
        usedSpawnPoints[index] = false;
    }

    void Start()
    {
        poolManager = ManagerRegistry.Get<PoolManagerGen>();
        usedSpawnPoints = new List<bool>(new bool[spawnPoints.Count]);
        InvokeRepeating(nameof(SpawnCoins), 0f, checkInterval);
    }

    void SpawnInitialCoins()
    {

        if (poolManager == null)
        {
            Debug.Log("POOL MANAGER WAS NULL");
            return;
        }

        int spawnedCoins = 0;

        while (spawnedCoins < initialCoinsToSpawn)
        {
            int randomIndex = Random.Range(0, spawnPoints.Count);
            if (!usedSpawnPoints[randomIndex])
            {
                usedSpawnPoints[randomIndex] = true;
                var coin = poolManager.GetObject<Coins>(PoolManagerKeys.COINS);

                if (coin != null)
                {
                    coin.InitCoins(randomIndex);
                    coin.transform.localPosition = spawnPoints[randomIndex].position;
                    coin.transform.SetParent(_parent);
                }

                spawnedCoins++;
            }
        }
    }

    void SpawnCoins()
    {
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            if (!usedSpawnPoints[i])
            {
                var coin = poolManager.GetObject<Coins>(PoolManagerKeys.COINS);

                if (coin != null)
                {
                    coin.InitCoins(i);
                    coin.transform.localPosition = spawnPoints[i].position;
                    coin.transform.SetParent(_parent);
                }

                usedSpawnPoints[i] = true;
                break;
            }
        }
    }
}
