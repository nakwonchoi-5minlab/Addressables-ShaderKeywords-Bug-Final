using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LoadAddressable : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] AssetReference reference;

    /// Start the load operation on start
    void Start()
    {
        AsyncOperationHandle handle = reference.LoadAssetAsync<Material>();
        handle.Completed += Handle_Completed;
    }

    // Instantiate the loaded prefab on complete
    private void Handle_Completed(AsyncOperationHandle obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            meshRenderer.SetMaterials(new List<Material> { reference.Asset as Material });
        }
        else
        {
            Debug.LogError("AssetReference failed to load.");
        }
    }

    // Release asset when parent object is destroyed
    private void OnDestroy()
    {
        reference.ReleaseAsset();
    }
}
