
using Game;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameAssets;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetContainer<K, T> where T : UnityEngine.Object where K: IComparable, IFormattable, IConvertible
{
    private Dictionary<string, AsyncOperationHandle<T>> _loaded = new Dictionary<string, AsyncOperationHandle<T>>();

    public async Task LoadAsync(K key, Action<T> onComplete) 
    {
        if (!typeof(K).IsEnum)
            throw new Exception("Not enum parameter");
        
        var i = Convert.ToInt32(key);
        if (!AddressIdMap.IdMap.TryGetValue(i, out var addr))
        {
            throw new Exception("Could not find asset address for "+key);
        }
        await LoadAsync(addr, onComplete);
    }

    public async Task LoadAsync(string address, Action<T> onComplete)
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
        onComplete(handle.Result);
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

    public async Task InstantiateAsync<K>(K key, Vector3 pos, Quaternion rot, Action<GameObject> onComplete) where K : IComparable, IFormattable, IConvertible
    {
        var handle = Addressables.InstantiateAsync(GetAddress(key), pos, rot);
        await handle.Task;
        onComplete?.Invoke(handle.Result);
    }

    public async Task InstantiateAsync(string address, Vector3 pos, Quaternion rot, Action<GameObject> onComplete)
    {
        var handle = Addressables.InstantiateAsync(address, pos, rot);
        await handle.Task;
        onComplete?.Invoke(handle.Result);
    }
}