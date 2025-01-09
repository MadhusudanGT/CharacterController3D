using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SpawnEnvirnoment : MonoBehaviour
{
    [SerializeField] private AssetReferenceGameObject _environment;
    private GameObject _instanceReference;

    private void LoadEnvironment()
    {
        if (_instanceReference != null)
        {
            Debug.Log("Environment is already spawned. Skipping duplicate spawn.");
            return;
        }

        if (_environment != null)
        {
            _environment.LoadAssetAsync<GameObject>().Completed += OnAddressableLoaded;
        }
        else
        {
            Debug.LogError("Environment asset reference is not set.");
        }
    }

    private void OnAddressableLoaded(AsyncOperationHandle<GameObject> handler)
    {
        if (handler.Status == AsyncOperationStatus.Succeeded)
        {
            if (_instanceReference == null)
            {
                _instanceReference = Instantiate(handler.Result);
            }
        }
        else
        {
            Debug.LogError("Failed to load environment asset: " + handler.OperationException);
        }
    }

    [Button("LOAD RESOURCE")]
    public void LoadResource()
    {
        LoadEnvironment();
    }

    [Button("RELEASE RESOURCE")]
    public void UnloadResource()
    {
        if (_instanceReference != null)
        {
            Destroy(_instanceReference);
            _environment.ReleaseAsset();
            _instanceReference = null;
        }
        else
        {
            Debug.LogWarning("No instance to release.");
        }
    }
}
