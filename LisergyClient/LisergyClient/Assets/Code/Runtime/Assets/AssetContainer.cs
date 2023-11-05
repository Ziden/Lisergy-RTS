using System;
using System.Collections.Generic;
using GameAssets;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

public class AssetContainer<K, T> where K: IComparable, IFormattable, IConvertible
{
    private Dictionary<string, AsyncOperationHandle<T>> _loaded = new Dictionary<string, AsyncOperationHandle<T>>();

    public async UniTask<T> LoadAsync(K key, Action<T> onComplete) 
    {
        if (!typeof(K).IsEnum)
            throw new Exception("Not enum parameter");
        
        var i = Convert.ToInt32(key);
        if (!AddressIdMap.IdMap.TryGetValue(i, out var addr))
        {
            throw new Exception("Could not find asset address for "+key);
        }
        return await LoadAsync(addr, onComplete);
    }

    public async UniTask<T> LoadAsync(string address, Action<T> onComplete)
    {
        if (_loaded.TryGetValue(address, out var handle))
        {
            if (!handle.IsValid())
            {
                Debug.LogError("Error loading " + address);
                return default(T);
            }
            onComplete?.Invoke(handle.Result);
            return handle.Result;
        }
        handle = Addressables.LoadAssetAsync<T>(address);
        await handle.Task;
        _loaded[address] = handle;
        onComplete?.Invoke(handle.Result);
        return handle.Result;
    }
}


public class PrefabContainer
{
    private string GetAddress<K>(K key) where K : IComparable, IFormattable, IConvertible
    {
        if (!typeof(K).IsEnum)
            throw new Exception("Not enum parameter");

        var i = Convert.ToInt32(key);
        if (!AddressIdMap.IdMap.TryGetValue(i, out var addr))
        {
            throw new Exception("Could not find asset address for " + key);
        }
        return addr;
    }

    public async UniTask<GameObject> InstantiateAsync<K>(K key, Vector3 pos, Quaternion rot, Action<GameObject> onComplete) where K : IComparable, IFormattable, IConvertible
    {
        var handle = Addressables.InstantiateAsync(GetAddress(key), pos, rot);
        await handle.Task;
        onComplete?.Invoke(handle.Result);
        return handle.Result;
    }

    public async UniTask<GameObject> InstantiateAsync(string address, Vector3 pos, Quaternion rot, Action<GameObject> onComplete)
    {
        var handle = Addressables.InstantiateAsync(address, pos, rot);
        await handle.Task;
        onComplete?.Invoke(handle.Result);
        return handle.Result;
    }
}