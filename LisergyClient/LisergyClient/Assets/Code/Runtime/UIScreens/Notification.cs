using Assets.Code.Assets.Code.UIScreens.Base;
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
    public abstract class Notification : UITKScreen
    {
        public override void OnOpen()
        {
            //MainBehaviour.RunCoroutine(NotificationCoroutine());
        }

        private IEnumerator NotificationCoroutine()
        {
            yield return null;
            Root.style.top = 80;
            yield return new WaitForSeconds(5);
            Root.style.top = 0;
            yield return new WaitForSeconds(2);
            ScreenService.Close(this);
        }

        public override void OnLoaded(VisualElement root) => Root.Q().style.top = -30;
        public override void OnClose() => Root.Q().style.top = -30;

        private void HideUp()
        {
            Root.Q().schedule.Execute(LeftUp).Every(10).ForDuration(1000);
        }

        private void LeftUp()
        {
            var e = Root.Q();
            e.style.top = e.style.top.value.value - 1;
        }
    }
}
