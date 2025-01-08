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

    public void UpdateUserCoins(string deviceId, int coinsToAdd)
    {
        if (_databaseReference == null)
        {
            Debug.LogError("DATABASE REFERENCE WAS NULL");
            return;
        }

        _databaseReference.Child("users").Child(deviceId).Child("score").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int currentCoins = 0;

                if (snapshot.Exists && int.TryParse(snapshot.Value.ToString(), out currentCoins))
                {
                    //Debug.Log($"Current score for device ID {deviceId}: {currentCoins}");
                }
                else
                {
                    Debug.Log($"No coin data found for device ID {deviceId}, initializing to 0.");
                }

                int updatedCoins = currentCoins + coinsToAdd;

                _databaseReference.Child("users").Child(deviceId).Child("score").SetValueAsync(updatedCoins).ContinueWithOnMainThread(updateTask =>
                {
                    if (updateTask.IsCompleted)
                    {
                        //Debug.Log($"score updated successfully for device ID {deviceId}. New total: {updatedCoins}");
                    }
                    else
                    {
                        //Debug.LogError($"Failed to update score for device ID {deviceId}: {updateTask.Exception}");
                    }
                });
            }
            else
            {
                Debug.LogError($"Failed to retrieve coin data for device ID {deviceId}: {task.Exception}");
            }
        });
    }

    public void GetOrCreatUserScore(string deviceId, string userName, System.Action<int> callback)
    {
        _databaseReference.Child("users").Child(deviceId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    int score = int.Parse(snapshot.Child("score").Value.ToString());
                    Debug.Log($"Device ID {deviceId} exists. Retrieved score: {score}");
                    callback(score);
                }
                else
                {
                    SaveUserData(deviceId, userName, 0);
                    Debug.Log($"Device ID {deviceId} does not exist. Created new user with score = 0.");
                    callback(0);
                }
            }
            else
            {
                Debug.LogError($"Failed to check if device ID {deviceId} exists: {task.Exception}");
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
