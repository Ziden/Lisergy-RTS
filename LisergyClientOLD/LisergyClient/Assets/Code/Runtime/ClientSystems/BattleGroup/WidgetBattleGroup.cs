using Assets.Code.Code.Runtime.UnityServices.UI.Base;
using Assets.Code.UI;
using ClientSDK;
using Game.Engine.ECLS;
using Game.Systems.Battler;
using Party.UI;
using UnityEngine.UIElements;

namespace Assets.Code.Code.Runtime.ClientSystems.BattleGroup
{
    public class WidgetBattleGroup : VisualStruct
    {
        private WidgetPartyButton[] _parties = new WidgetPartyButton[4];

        public WidgetBattleGroup(IClientSdk client, VisualElement root) : base(root, client)
        {
            Client = client;
            Root = root;
            for (var x = 0; x < _parties.Length; x++)
            {
                _parties[x] = Root.Q<WidgetPartyButton>($"Party-{x + 1}").Required();
            }
        }

        public void DisplayComponent(IEntity owner, in BattleGroupComponent component)
        {
            for (int x = 0; x < 4; x++)
            {
                if(component.Units.Group.Count <= x)
                {
                    _parties[x].style.display = DisplayStyle.None;
                    continue;
                }
                var unit = component.Units[x];
                if (!unit.Valid)
                {
                    _parties[x].style.display = DisplayStyle.None;
                }
                else
                {
                    _parties[x].style.display = DisplayStyle.Flex;
                    _parties[x].DisplayUnit(unit).Forget();
                    _parties[x].OnClick = () => Client.UnityServices().UI.Open<WidgetUnitDetails>(new UnitDetailsSetup()
                    {
                        Entity = owner,
                        Unit = unit
                    });

                }
            }
        }

        public override void Dispose()
        {

        }
    }
}