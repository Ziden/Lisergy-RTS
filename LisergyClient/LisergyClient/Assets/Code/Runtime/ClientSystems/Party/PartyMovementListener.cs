using Assets.Code;
using ClientSDK;
using Game.Engine.Events.Bus;
using Game.Systems.Party;
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
        _client.ClientEvents.Register<MovementInterpolationStart>(this, OnMoveStart);
        _client.ClientEvents.Register<MovementInterpolationEnd>(this, OnMoveEnd);
    }

    private void OnMoveStart(MovementInterpolationStart e)
    {
        if(e.Entity is PartyEntity party)
        {
            var view = party.GetEntityView();
            foreach(var unitView in view.UnitViews)
            {
                unitView.Animations.PlayAnimation(UnitAnimation.Running);
                unitView.GameObject.transform.LookAt(new Vector3(e.To.X, unitView.GameObject.transform.position.y, e.To.Y));
            }
        }
    }

    private void OnMoveEnd(MovementInterpolationEnd e)
    {
        if (e.LastStep && e.Entity is PartyEntity party)
        {
            var view = party.GetEntityView();
            foreach (var unitView in view.UnitViews)
            {
                unitView.Animations.PlayAnimation(UnitAnimation.Iddle);
            }
        }
    }

}