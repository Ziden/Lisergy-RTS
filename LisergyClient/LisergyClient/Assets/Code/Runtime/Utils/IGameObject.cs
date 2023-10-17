using UnityEngine;

namespace Assets.Code
{
    // Objects that have a root game object
    public interface IGameObject
    {
        GameObject GameObject { get; }
    }
}
