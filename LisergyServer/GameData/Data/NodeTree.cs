using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class NodeTree<T>
{
	// Children are serialized
	private List<NodeTree<T>> _children;

	// Non-serialized parent reference
	[NonSerialized] private NodeTree<T> _parent;

	// This will store parent-child relationships during serialization
	private Dictionary<int, ParentReference> _parentRelations;

	// Core data
	public T Data;

	public NodeTree(T data)
	{
		Data = data;
		_children = new List<NodeTree<T>>();
		Level = 0;
	}

	public NodeTree(T data, NodeTree<T> parent) : this(data)
	{
		_parent = parent;
		Level = _parent != null ? _parent.Level + 1 : 0;
	}

	// Properties and indexers
	public int Level { get; private set; }

	public int Count => _children.Count;
	public bool IsRoot => _parent == null;
	public bool IsLeaf => _children.Count == 0;
	public NodeTree<T> Parent => _parent;

	public NodeTree<T> this[int key] => _children[key];

	private void SetParent(NodeTree<T> parent)
	{
		_parent = parent;
		Level = _parent != null ? _parent.Level + 1 : 0;
	}

	// Methods
	public void Clear()
	{
		_children.Clear();
	}

	public NodeTree<T> AddChild(NodeTree<T> node)
	{
		_children.Add(node);
		node.SetParent(this);
		return node;
	}

	public NodeTree<T> AddChild(T data)
	{
		var node = new NodeTree<T>(data);
		return AddChild(node);
	}

	public bool HasChild(T data)
	{
		return FindInChildren(data) != null;
	}

	public bool HasChild(Predicate<T> predicate)
	{
		return FindInChildren(predicate) != null;
	}

	public NodeTree<T> FindInChildren(T data)
	{
		return FindInChildren(x => EqualityComparer<T>.Default.Equals(x, data));
	}

	public IReadOnlyCollection<T> Children()
	{
		return _children.Select(c => c.Data).ToArray();
	}

	public IReadOnlyCollection<NodeTree<T>> ChildrenNodes()
	{
		return _children.ToArray();
	}

	public override string ToString()
	{
		return Data?.ToString() ?? "null";
	}

	public NodeTree<T> FindInChildren(Predicate<T> predicate)
	{
		for (var i = 0; i < _children.Count; i++)
		{
			var child = _children[i];
			if (predicate(child.Data)) return child;
		}

		return null;
	}

	public bool RemoveChild(NodeTree<T> node)
	{
		return _children.Remove(node);
	}

	public void Traverse(Func<T, bool> handler)
	{
		if (handler(Data))
			for (var i = 0; i < _children.Count; i++)
				_children[i].Traverse(handler);
	}

	public NodeTree<T> FindElement(Func<T, bool> condition)
	{
		NodeTree<T> el = default;
		TraverseNodes(e =>
		{
			if (condition(e.Data))
			{
				el = e;
				return false;
			}

			return true;
		});
		return el;
	}

	public void TraverseNodes(Func<NodeTree<T>, bool> handler)
	{
		if (handler(this))
			for (var i = 0; i < _children.Count; i++)
				_children[i].TraverseNodes(handler);
	}

	public string ToFormattedString()
	{
		return ToString(0);
	}

	private string ToString(int indentLevel)
	{
		var indent = new string(' ', indentLevel * 2);
		var result = $"{indent}{Data}\n";
		foreach (var child in _children) result += child.ToString(indentLevel + 1);
		return result;
	}

	public void OnSerializing()
	{
		// Store parent-child relationships before serializing
		_parentRelations = new Dictionary<int, ParentReference>();

		for (var i = 0; i < _children.Count; i++) _parentRelations[i] = new ParentReference {ChildIndex = i};
	}

	public void OnDeserialized()
	{
		// Restore parent references after deserialization
		if (_children != null)
			foreach (var child in _children)
				child.SetParent(this);

		// Clean up temporary serialization data
		_parentRelations = null;
	}

	public List<T> Flatten()
	{
		var result = new List<T>();
		Traverse(t =>
		{
			result.Add(t);
			return true;
		});
		return result;
	}

	// For tracking parent during serialization
	[Serializable]
	private struct ParentReference
	{
		public int ChildIndex;
	}
}