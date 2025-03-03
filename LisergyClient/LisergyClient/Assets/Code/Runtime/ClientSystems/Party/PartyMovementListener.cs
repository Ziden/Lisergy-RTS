using Assets.Code;
using Assets.Code.World;
using ClientSDK;
using Game.Entities;
using Game.Engine.Events.Bus;
using UnityEngine;

/// <summary>
/// Perform map animations like movement , battle smoke etc
/// </summary>
public class PartyMovementListener : IEventListener
{
    private IGameClient _client;

    public PartyMovementListener(IGameClient client)
    {
        _client = client;
        _client.ClientEvents.On<MovementInterpolationStart>(this, OnMoveStart);
        _client.ClientEvents.On<MovementInterpolationEnd>(this, OnMoveEnd);
    }

    private void OnMoveStart(MovementInterpolationStart e)
    {
        if (e.Entity.EntityType == EntityType.Party)
        {
            var view = e.Entity.GetView<PartyView>();
            foreach (var unitView in view.UnitViews)
            {
                unitView.Animations.PlayAnimation(UnitAnimation.Running);
                unitView.GameObject.transform.LookAt(new Vector3(e.To.X, unitView.GameObject.transform.position.y, e.To.Y));
            }
        }
    }

    private void OnMoveEnd(MovementInterpolationEnd e)
    {
        if (e.LastStep && e.Entity.EntityType == EntityType.Party)
        {
            var view = e.Entity.GetView<PartyView>();
            foreach (var unitView in view.UnitViews)
            {
                unitView.Animations.PlayAnimation(UnitAnimation.Iddle);
            }
        }
    }

}