using Assets.Code.Assets.Code.UIScreens.Base;
using GameAssets;
using System;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Assets.Code.Assets.Code.Runtime.UIScreens
{
    public class TransitionScreen : GameUi
    {
        private Length RIGHT = Length.Percent(100f);
        private Length CENTER = Length.Percent(33f);
        private Length LEFT = Length.Percent(0f);

        private float _x = 0;

        public override UIScreen UiAsset => UIScreen.TransitionScreen;

        public override void OnOpen()
        {
            FillScreen();
        }

        public async UniTask RunWhenScreenFilled(Action a)
        {
            while (_x != 200) await Task.Delay(2);
            a();
        }

        private async UniTask Close()
        {
            await Task.Delay(2000);
            _uiService.Close(this);
        }

        public void CloseTransition(Action callback = null)
        {
            FadeAway();
            _ = Close();
        }

        public override void OnClose()
        {
            Reset();
        }

        public void Reset()
        {
            _root.style.translate = new Translate(0, 0);
        }

        private void MoveRight()
        {
            _x++;
            _root.style.translate = new Translate(Length.Percent(_x), 0);
        }

        public void FillScreen()
        {
            _root.schedule.Execute(MoveRight).Every(2).Until(() => _x >= 200);
        }

        public void FadeAway()
        {
            _root.schedule.Execute(MoveRight).Every(2).Until(() => _x >= 400);
        }
    }
}
