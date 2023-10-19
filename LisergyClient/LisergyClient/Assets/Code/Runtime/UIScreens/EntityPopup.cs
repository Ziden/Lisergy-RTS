using Assets.Code.Assets.Code.Runtime.UIScreens.Layout;
using Assets.Code.Assets.Code.UIScreens.Base;
using Assets.Code.Views;
using Game;
using Game.ECS;
using Game.Events.Bus;
using Game.Systems.Battler;
using Game.Systems.Dungeon;
using Game.Systems.Party;
using Game.Tile;
using GameAssets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Code.UI
{

    public class EntityPopupSetup : UIScreenSetup
    {
        public IEntity Entity;
        public List<Type> Components;
    }

    public class EntityDetails : UITKScreen, IEventListener
    {
        public override UIScreen ScreenAsset => UIScreen.EntityDetails;

        private Dictionary<Type, VisualElement> _components = new Dictionary<Type, VisualElement>();

        public override void OnLoaded(VisualElement root)
        {
            _components[typeof(BattleGroupComponent)] = root.Q("BattleGroupComponent");
        }

        public override void OnClose()
        {
          
        }

        public override void OnOpen()
        {
        
            var setup = GetSetup<EntityPopupSetup>();
        }

       
    }
}
