using Assets.Code.UI;
using ClientSDK;
using Game.Engine.ECLS;
using Game.Systems.Battler;
using Party.UI;
using UnityEngine.UIElements;

public class WidgetBattleGroup : VisualStruct
{
    private VisualElement _root;
    private IGameClient _client;

    private WidgetPartyButton[] _parties = new WidgetPartyButton[4];

    public WidgetBattleGroup(IGameClient client, VisualElement root) : base(root, client)
    {
        _client = client;
        _root = root;
        for (var x = 0; x < _parties.Length; x++)
        {
            _parties[x] = _root.Q<WidgetPartyButton>($"Party-{x + 1}").Required();
        }
    }

    public void DisplayComponent(IEntity owner, in BattleGroupComponent component)
    {
        for (int x = 0; x < 4; x++)
        {
            var unit = component.Units[x];
            if (!unit.Valid)
            {
                _parties[x].style.display = DisplayStyle.None;
            }
            else
            {
                _parties[x].style.display = DisplayStyle.Flex;
                _parties[x].DisplayUnit(unit).Forget();
                _parties[x].OnClick = () => _client.UnityServices().UI.Open<WidgetUnitDetails>(new UnitDetailsSetup()
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