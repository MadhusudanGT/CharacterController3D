using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class UIManager : MonoBehaviour
{
    public TMP_Text leaderboardText;
    private FirebaseManager firebaseManager;
    private GameManager gameManager;

    void Start()
    {
        firebaseManager = ManagerRegistry.Get<FirebaseManager>();
        gameManager = ManagerRegistry.Get<GameManager>();
    }

    [Button("UPDATE DEVICE ID")]
    public void UpdateDeviceId(string deviceId)
    {
        if (gameManager == null)
        {
            Debug.Log("Game Manager instance was null");
            return;
        }
        gameManager.DeviceId = deviceId;
    }

    [Button("SAVE USER")]
    public void SaveUser(string PlayerName, int score)
    {
        if (firebaseManager == null)
        {
            Debug.Log("firebaseManager instance was null");
            return;
        }

        if (gameManager == null)
        {
            Debug.Log("Game Manager instance was null");
            return;
        }

        string deviceId = gameManager.DeviceId;
        if (string.IsNullOrEmpty(gameManager.DeviceId))
        {
            Debug.Log("Device id was empty");
            return;
        }

        firebaseManager.SaveUserData(deviceId, PlayerName, score);
        firebaseManager.GetLeaderboardData(ShowLeaderboard);
    }

    [Button("UPDATE USER SCORE")]
    public void UpdateUserScore(int newScore)
    {
        if (firebaseManager == null)
        {
            Debug.Log("firebaseManager instance was null");
            return;
        }

        string deviceId = gameManager.DeviceId;
        if (string.IsNullOrEmpty(gameManager.DeviceId))
        {
            Debug.Log("Device id was empty");
            return;
        }

        firebaseManager.UpdateUserScore(deviceId, newScore);
    }

    [Button("GET LEADER BOARD")]
    public void ShowLeaderboard(List<User> users)
    {
        if (users == null) return;

        //leaderboardText.text = "Leaderboard:\n";
        foreach (var user in users)
        {
            //leaderboardText.text += $"{user.username}: {user.score}\n";
            Debug.Log(user.username + "....USER DATA...." + user.score);
        }
    }
}
