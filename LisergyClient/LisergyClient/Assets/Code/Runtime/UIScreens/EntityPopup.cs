using Assets.Code.Assets.Code.Runtime.UIScreens.Layout;
using Assets.Code.Assets.Code.UIScreens.Base;
using Assets.Code.Views;
using Game;
using Game.ECS;
using Game.Events.Bus;
using Game.Systems.Battler;

using GameAssets;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Assets.Code.UI
{

    public class EntityDetailsParams : IGameUiParam
    {
        public IEntity Entity;
    }

    public class EntityDetails : GameUi, IEventListener
    {
        public override UIScreen UiAsset => UIScreen.EntityDetails;

        private BattleGroupComponentUI _battleGroup;

        public override void OnOpen()
        {
            var setup = GetParameter<EntityDetailsParams>();
            var battleGroup = setup.Entity.Components.Get<BattleGroupComponent>();
            if (battleGroup.Units.Valids > 0)
            {
                _battleGroup = new BattleGroupComponentUI(GameClient, Root.Q("BattleGroupComponent").Required());
                _battleGroup.DisplayGroup(setup.Entity, battleGroup);
            }
        }
    }
}
