using System;
using ClientSDK;
using Game.World;
using UnityEngine;


/// <summary>
/// Game engine agnostic object representation code
/// </summary>
public struct UnityGameObject : IGameObject
{
	public UnityGameObject(GameObject n)
	{
		Node = n;
	}
	
	public GameObject Node { get; }
	
	public string Name
	{
		get => Node.name;
		set => Node.name = value;
	}

	public Location Location
	{
		get => Node.transform.position.ToPosition();
		set => Node.transform.position = value.ToUnityVector3();
	
	}

	public bool Visible
	{
		get => Node.activeInHierarchy;
		set => Node.SetActive(value);

	}

	public void AddChild(IGameObject child)
	{
		var otherNode = ((UnityGameObject) child).Node;
		otherNode.transform.SetParent(Node.transform);
	}

	public void DestroyChild(IGameObject child)
	{
		var n = ((UnityGameObject) child).Node;
		UnityEngine.Object.Destroy(n);
	}

	public T Get<T>() where T : class
	{
		if (typeof(T) == typeof(GameObject)) return Node as T;
		if (typeof(T) == typeof(Transform)) return Node.transform as T;
		return Node.GetComponent<T>();
	}

	public void Destroy()
	{
		UnityEngine.Object.Destroy(Node);
	}
}
