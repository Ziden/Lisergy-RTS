using Assets.Code.Assets.Code.Audio;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class AudioReference
{
    public SoundFx Key;
    public AssetReferenceT<AudioClip> Reference;
}

[Serializable]
public class PrefabReference
{
    public string Key;
    public AssetReferenceT<GameObject> Reference;
}

[Serializable]
[CreateAssetMenu(fileName = "AssetReferences", menuName = "ScriptableObjects/AssetReferences", order = 1)]
public class AssetReferences : ScriptableObject
{
    public List<AudioReference> SoundEffects = new List<AudioReference>();
    public List<PrefabReference> MapObjects = new List<PrefabReference>();
}