
using Game;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetContainer<K, T> where T : UnityEngine.Object
{
    private Dictionary<string, AsyncOperationHandle<T>> _loaded = new Dictionary<string, AsyncOperationHandle<T>>();
    private Dictionary<K, AssetReferenceT<T>> _references = new Dictionary<K, AssetReferenceT<T>>();

    public void RegisterReference(K key, AssetReferenceT<T> reference)
    {
        _references[key] = reference;
    }

    public async Task LoadAsync(K key, Action<T> onComplete)
    {
        if (_references.TryGetValue(key, out var reference))
        {
            if (reference.Asset == null || !reference.IsValid())
            {
                var handle = reference.LoadAssetAsync();
                await handle.Task;
                _loaded[reference.AssetGUID] = handle;
            }
            onComplete?.Invoke((T)reference.Asset);
            return;
        }
        else throw new Exception($"Requested invalid reference for {key}. Register in AssetReferences.asset");
    }

    public async Task InstantiateAsync(K key, Vector3 position, Quaternion rot, Action<GameObject> onComplete)
    {
        if (_references.TryGetValue(key, out var reference))
        {
            var handle = reference.InstantiateAsync(position, rot);
            await handle.Task;
            var instantiated = handle.Result;
            onComplete?.Invoke(instantiated);
            return;
        }
        throw new Exception($"Requested invalid reference for {key}. Register in AssetReferences.asset");
    }

    public async Task LoadAsyncByAddress(string address, Action<T> onComplete)
    {
        if (_loaded.TryGetValue(address, out var handle))
        {
            if (!handle.IsValid())
            {
                Log.Error("Error loading " + address);
                return;
            }
            onComplete(handle.Result);
            return;
        }
        handle = Addressables.LoadAssetAsync<T>(address);
        await handle.Task;
        _loaded[address] = handle;
        onComplete?.Invoke(handle.Result);
    }
}