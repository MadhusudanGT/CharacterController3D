using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text deviceIdTxt;
    [SerializeField] private TMP_Text playerScore;

    private FirebaseManager firebaseManager;
    private GameManager gameManager;

    private void Awake()
    {
        ManagerRegistry.Register<UIManager>(this);
    }
    void OnEnable()
    {
        EventManager.updateDeviceId += DeviceId;
        EventManager.coinPoints += UpdateCoins;
    }

    void OnDisable()
    {
        EventManager.updateDeviceId -= DeviceId;
        EventManager.coinPoints -= UpdateCoins;
    }

    void DeviceId(string deviceId)
    {
        deviceIdTxt.SetText(deviceId);
        GetUserData();
    }

    void UpdateCoins(int cointPoints)
    {
        UpdateUserScore(cointPoints);
    }

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

        firebaseManager.UpdateUserCoins(deviceId, newScore);
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

    [Button("GET USER SCORE")]
    public void GetUserData()
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

        firebaseManager.GetOrCreatUserScore(deviceId, "Madhu", UpdateUserInfo);
    }

    public void UpdateUserInfo(int score)
    {
        playerScore.SetText(string.Concat("Score: ", score.ToString()));
    }
}
