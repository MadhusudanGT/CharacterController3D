using System.Collections.Generic;
using UnityEngine;

public class LeaderBoardSystem : MonoBehaviour
{
    [SerializeField] private Transform _parent;
    [SerializeField] private FirebaseManager firebaseManager;
    [SerializeField] private List<Ranking> listOfRankingPoolData;
    [SerializeField] private PoolManagerGen poolManager;
    private void OnEnable()
    {
        GetListOfRank();
    }

    private void Start()
    {
        poolManager = ManagerRegistry.Get<PoolManagerGen>();
    }

    void GetListOfRank()
    {
        if (firebaseManager == null)
        {
            firebaseManager = ManagerRegistry.Get<FirebaseManager>();
        }

        if (firebaseManager != null)
        {
            firebaseManager.GetLeaderboardData(ShowLeaderboard);
        }
        else
        {
            Debug.Log("FIRE BASE MANAGER WAS NULL");
        }
    }

    public void ShowLeaderboard(List<User> users)
    {
        if (users == null || users.Count == 0)
        {
            Debug.LogWarning("No users provided to display on the leaderboard.");
            return;
        }

        int displayCount = Mathf.Min(users.Count, 5);

        if (poolManager == null)
        {
            poolManager = ManagerRegistry.Get<PoolManagerGen>();
            if (poolManager == null)
            {
                Debug.LogError("PoolManager is not initialized!");
                return;
            }
        }

        while (listOfRankingPoolData.Count < displayCount)
        {
            listOfRankingPoolData.Add(null);
        }

        for (int i = 0; i < displayCount; i++)
        {
            //Debug.Log($"Processing rank {i}: {users[i].username} - Score: {users[i].score}");

            if (i < listOfRankingPoolData.Count && listOfRankingPoolData[i] != null)
            {
                listOfRankingPoolData[i].InitRank(users[i], i+1);
                SetLeaderboardItemTransform(listOfRankingPoolData[i].transform, i);
            }
            else
            {
                SpawnRankedData(users[i], i);
            }
        }

        for (int i = displayCount; i < listOfRankingPoolData.Count; i++)
        {
            if (listOfRankingPoolData[i] != null)
            {
                listOfRankingPoolData[i].gameObject.SetActive(false);
            }
        }
    }

    void SpawnRankedData(User userData, int rank)
    {
        if (poolManager == null)
        {
            Debug.LogError("PoolManager is not initialized. Cannot spawn ranked data.");
            return;
        }

        var leaderBoard = poolManager.GetObject<Ranking>(PoolManagerKeys.LEADER_BOARD);
        if (leaderBoard == null)
        {
            Debug.LogError("Failed to retrieve leaderboard object from the pool.");
            return;
        }

        leaderBoard.InitRank(userData, rank + 1);
        SetLeaderboardItemTransform(leaderBoard.transform, rank);

        if (rank < listOfRankingPoolData.Count)
        {
            listOfRankingPoolData[rank] = leaderBoard;
        }
        else
        {
            listOfRankingPoolData.Add(leaderBoard);
        }
    }

    void SetLeaderboardItemTransform(Transform itemTransform, int siblingIndex)
    {
        if (itemTransform == null)
        {
            Debug.LogError("Leaderboard item transform is null.");
            return;
        }

        itemTransform.SetParent(_parent);
        itemTransform.SetSiblingIndex(siblingIndex + 1);
    }

}
