using Assets.Code.Assets.Code.UIScreens.Base;
using GameAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Code.Assets.Code.Runtime.UIScreens
{
    public class TransitionScreen : UITKScreen
    {
        private Length RIGHT = Length.Percent(100f);
        private Length CENTER = Length.Percent(33f);
        private Length LEFT = Length.Percent(0f);

        private float _x = 0;

        private VisualElement _root;

        public override UIScreen ScreenAsset => UIScreen.TransitionScreen;

        public override void OnLoaded(VisualElement root)
        {
            _root = root;
        }

        public override void OnOpen()
        {
            FillScreen();
        }

        public async Task RunWhenScreenFilled(Action a) 
        {
            while (_x != 200) await Task.Delay(2);
            a();
        }

        public void CloseTransition(Action callback = null)
        {
            FadeAway();
            _ = MainBehaviour.RunAsync(() =>
            {
                _screenService.Close(this);
                callback?.Invoke();
            }, 2);
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
