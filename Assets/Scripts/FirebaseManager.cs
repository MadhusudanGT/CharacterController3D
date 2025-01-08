using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    private DatabaseReference _databaseReference;

    private void Awake()
    {
        ManagerRegistry.Register<FirebaseManager>(this);
    }
    private void Start()
    {
        InitializeFireBase();
    }
    public void InitializeFireBase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                _databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase initialized successfully.");
            }
            else
            {
                Debug.LogError($"Firebase initialization failed: {task.Result}");
            }
        });
    }

    public void SaveUserData(string deviceId, string username, int score)
    {
        if (_databaseReference == null)
        {
            Debug.Log("DATA BASE REFERENCE WAS NULL");
            return;
        }

        User user = new(username, score);
        string json = JsonUtility.ToJson(user);
        _databaseReference.Child("users").Child(deviceId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("User data saved successfully.");
            }
            else
            {
                Debug.LogError($"Failed to save user data: {task.Exception}");
            }
        });
    }

    public void GetLeaderboardData(System.Action<List<User>> callback)
    {
        if (_databaseReference == null)
        {
            Debug.Log("DATA BASE REFERENCE WAS NULL");
            return;
        }

        _databaseReference.Child("users").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                List<User> users = new();

                foreach (DataSnapshot userSnapshot in snapshot.Children)
                {
                    string json = userSnapshot.GetRawJsonValue();
                    Debug.Log("USERS DATA..." + json);
                    User user = JsonUtility.FromJson<User>(json);
                    users.Add(user);
                }

                users.Sort((u1, u2) => u2.score.CompareTo(u1.score));

                callback(users);
            }
            else
            {
                Debug.LogError($"Failed to retrieve leaderboard data: {task.Exception}");
                callback(null);
            }
        });
    }

    public void UpdateUserScore(string deviceId, int newScore)
    {
        if (_databaseReference == null)
        {
            Debug.Log("DATA BASE REFERENCE WAS NULL");
            return;
        }

        _databaseReference.Child("users").Child(deviceId).Child("score").SetValueAsync(newScore).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log($"Score updated successfully for device ID: {deviceId}");
            }
            else
            {
                Debug.LogError($"Failed to update score: {task.Exception}");
            }
        });
    }
}

[System.Serializable]
public class User
{
    public string username;
    public int score;

    public User(string username, int score)
    {
        this.username = username;
        this.score = score;
    }
}
