
using Game.Events.Bus;
using UnityEngine;

namespace Assets.Code.Views
{
    public interface IEntityView : IEventListener, IGameObject
    {
    }
}
