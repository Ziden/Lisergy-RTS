using Assets.Code.Assets.Code.Runtime.UIScreens.Parts;
using Assets.Code.UI;
using ClientSDK;
using Game.ECS;
using Game.Systems.Battler;
using UnityEngine.UIElements;

public class BattleGroupComponentUI
{
    private VisualElement _root;
    private IGameClient _client;

    private PartyButton[] _parties = new PartyButton[4];

    public BattleGroupComponentUI(IGameClient client, VisualElement root)
    {
        _client = client;
        _root = root;
        for (var x = 0; x < _parties.Length; x++)
        {
            _parties[x] = new PartyButton(client, _root.Q($"PartyButton-{x + 1}").Required());
        }
    }

    public void DisplayGroup(IEntity owner, in BattleGroupComponent component)
    {
        for(int x = 0; x < 4; x++)
        {
            var unit = component.Units[x];
            _parties[x].DisplayLeader(unit);
            if (!unit.Valid) continue;
            _parties[x].OnClick(() => _client.UnityServices().Screen.Open<UnitDetails>(new UnitDetailsSetup()
            {
                Entity = owner,
                Unit = unit
            }));
        }
    }
}