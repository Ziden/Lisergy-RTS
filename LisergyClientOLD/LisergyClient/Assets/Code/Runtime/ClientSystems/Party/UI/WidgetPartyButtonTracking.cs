using Assets.Code.ClientSystems.Party.UI;
using ClientSDK;
using Cysharp.Threading.Tasks;
using Game.Engine.ECLS;
using Game.Engine.Events.Bus;
using Game.Systems.Battler;
using Game.Systems.Movement;
using Game.Systems.Resources;
using GameAssets;
using System;
using UnityEngine;
using UnityEngine.UIElements;


namespace Party.UI
{
    public enum TaskIcon
    {
        Iddle, Moving, Harvesting
    }

    /// <summary>
    /// Tracks current party state and displays in the party state widget
    /// This is to show your own party states
    /// </summary>
    [UxmlElement]
    public partial class WidgetPartyButtonTracking : WidgetElement, IEventListener
    {
        private IEntity _trackedEntity;
        private TaskIcon _task;
        private readonly VisualElement _greenHpBar;
        private readonly VisualElement _hpBarContainer;
        private readonly VisualElement _taskOverlay;
        private readonly Label _taskNumber;
        private readonly VisualElement _taskIcon;
        private readonly VisualElement _taskSymbol;
        private IVisualElementScheduledItem _progressBar;

        private IClientSdk _client;

        public WidgetPartyButtonTracking()
        {
            LoadUxmlFromResource("WidgetPartyButtonTracking");
            _taskOverlay = this.Q("TaskOverlay").Required();
            _taskSymbol = this.Q("TaskSymbol").Required();
            _taskSymbol.style.display = DisplayStyle.None;
            _hpBarContainer = this.Q("Bar").Required();
            _greenHpBar = this.Q("BarTracker").Required();
            _hpBarContainer.style.display = DisplayStyle.None;
            this.style.position = Position.Absolute;

            if (_taskOverlay == null) return;
            _taskOverlay.style.display = DisplayStyle.None;
            _taskIcon = _taskOverlay.Q("TaskIcon").Required();
            _taskNumber = _taskOverlay.Q<Label>("TaskNumber").Required();
        }

        public void UpdateDetails(Unit unit)
        {
            if (_greenHpBar != null)
            {
                var hpRatio = unit.HpRatio;
                _greenHpBar.style.width = Length.Percent((float)(hpRatio) * 100);
            }
        }

        public void HideBar()
        {
            _hpBarContainer.style.display = DisplayStyle.None;
        }

        public void Track(IEntity e)
        {
            _trackedEntity = e;
            this.MarkDirtyRepaint();
        }

        public void ShowBar()
        {
            _hpBarContainer.style.display = DisplayStyle.Flex;
        }

        public override void OnAddedDuringGame(IClientSdk client)
        {
            var partyWidget = this.parent as WidgetPartyButton;
            if (partyWidget == null) return;

            _client = client;
            _trackedEntity = partyWidget.PartyEntity;
            client.ClientEvents.On<HarvestingUpdateEvent>(this, OnHarvestUpdate);
            client.Modules.Entities.OnUpdate<MovementComponent>(OnCourseUpdate);

            if (_trackedEntity != null)
            {
                UpdatePartyStateIcon();
            }
        }

        public override void OnRemovedDuringGame(IClientSdk client)
        {
            client.ClientEvents.RemoveListener(this);
            client.Modules.Entities.RemoveListener(this);
        }

        private void OnHarvestUpdate(HarvestingUpdateEvent e)
        {
            if (_trackedEntity == null || e.Entity.EntityId != _trackedEntity.EntityId) return;
            UpdatePartyStateIcon();
            UpdateResourceTask(e);
        }

        private void OnCourseUpdate(IEntity e, MovementComponent old, MovementComponent newC)
        {
            if (_trackedEntity == null || e.EntityId != _trackedEntity.EntityId) return;
            UpdatePartyStateIcon();
        }

        private void UpdatePartyStateIcon()
        {
            _client.Log.Debug("[PartyTracking] Updating own party UI state " + _trackedEntity);
            if (_trackedEntity.Components.TryGet<MovementComponent>(out var c) && !c.CourseId.IsZero())
            {
                SetTaskIcon(TaskIcon.Moving).Forget();
            }
            else if (_trackedEntity.Components.Has<HarvestingComponent>())
                SetTaskIcon(TaskIcon.Harvesting).Forget();
            else
                SetTaskIcon(TaskIcon.Iddle).Forget();
        }

        public async UniTaskVoid SetTaskIcon(TaskIcon task)
        {
            if (_task == task) return;
            _task = task;
            Debug.Log("[PartyTracking] Setting view task display: " + task);
            SpritePrefab sprite;
            if (task == TaskIcon.Moving)
            {
                sprite = SpritePrefab.Icon_itemicon_shoeswing;
            }
            else if (task == TaskIcon.Harvesting)
            {
                sprite = SpritePrefab.Icon_itemicon_pickax;
            }
            else
            {
                ClearTask();
                return;
            }
            var s = await _client.UnityServices().Assets.GetSprite(sprite);
            if (IsDestroyed()) return;

            _taskSymbol.style.backgroundImage = new StyleBackground(s);
            _taskSymbol.style.display = DisplayStyle.Flex;

        }

        public void UpdateBar(DateTime start, DateTime end)
        {
            if (_progressBar != null && _progressBar.isActive) return;

            _progressBar = _greenHpBar.schedule.Execute(a =>
            {
                var totalMsElapsed = _client.Game.GameTime - start;
                var totalMsNeeded = end - start;
                var pct = (float)(totalMsElapsed.TotalMilliseconds / (float)totalMsNeeded.TotalMilliseconds);
                _greenHpBar.style.width = Length.Percent(pct * 100);
            }).Every(5).Until(() => _greenHpBar.style.width == Length.Percent(100));
        }

        public void UpdateResourceTask(HarvestingUpdateEvent ev)
        {
            if (ev.Depleted)
            {
                ClearTask();
                return;
            }
            _taskOverlay.style.display = DisplayStyle.Flex;
            var resourceSpec = _client.Game.Specs.Resources[ev.TileResources.Resource.ResourceId];
            _taskIcon.SetBackground(resourceSpec.Art);
            _taskNumber.text = ev.AmountHarvestedTotal.ToString();

            var totalTime = _client.Game.GameTime - ev.InitialState.TimeSnapshot.TimeBlock.StartTime;
            var pct = totalTime.TotalMilliseconds / ev.InitialState.TimeSnapshot.TimeBlock.TotalBlockTime.TotalMilliseconds;
            if (pct < 1)
            {
                ShowBar();
                UpdateBar(ev.InitialState.TimeSnapshot.TimeBlock.StartTime, ev.InitialState.TimeSnapshot.TimeBlock.EndTime);
            }
        }

        public void ClearTask()
        {
            _taskSymbol.style.display = DisplayStyle.None;
            _taskOverlay.style.display = DisplayStyle.None;
            _progressBar?.Pause();
            _progressBar = null;
            HideBar();
        }
    }
}
