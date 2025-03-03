using System;
using System.Collections.Generic;
using System.Linq;

public class NodeTree<T>
{
    public T Data;
    private NodeTree<T> _parent;
    private int _level;
    private List<NodeTree<T>> _children;

    public NodeTree(T data)
    {
        Data = data;
        _children = new List<NodeTree<T>>();
        _level = 0;
    }

    public NodeTree(T data, NodeTree<T> parent) : this(data)
    {
        _parent = parent;
        _level = _parent != null ? _parent.Level + 1 : 0;
    }

    private void SetParent(NodeTree<T> parent)
    {
        _parent = parent;
        _level = _parent != null ? _parent.Level + 1 : 0;
    }

    public int Level { get { return _level; } }
    public int Count { get { return _children.Count; } }
    public bool IsRoot { get { return _parent == null; } }
    public bool IsLeaf { get { return _children.Count == 0; } }
    public NodeTree<T> Parent { get { return _parent; } }

    public NodeTree<T> this[int key]
    {
        get { return _children[key]; }
    }

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
        return FindInChildren(x => x.Equals(data));
    }

    public IReadOnlyCollection<T> Children()
    {
        return _children.Select(c => c.Data).ToArray();
    }

    public override string ToString()
    {
        return Data.ToString();
    }

    public NodeTree<T> FindInChildren(Predicate<T> predicate)
    {
        int i = 0, l = Count;
        for (; i < l; ++i)
        {
            NodeTree<T> child = _children[i];
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
        {
            int i = 0, l = Count;
            for (; i < l; ++i) _children[i].Traverse(handler);
        }
    }

    public void TraverseNodes(Func<NodeTree<T>, bool> handler)
    {
        if (handler(this))
        {
            int i = 0, l = Count;
            for (; i < l; ++i) _children[i].TraverseNodes(handler);
        }
    }

}