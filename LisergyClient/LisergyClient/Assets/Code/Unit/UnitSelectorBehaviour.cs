using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class UnitSelectorBehaviour : MonoBehaviour
    {

        private float angle;
        private float h;
        private float startH;

        void Start()
        {
            startH = transform.localPosition.y;
        }

        void Update()
        {
            transform.rotation = Quaternion.Euler(0, angle, 0);
            angle += 1;
            if(angle > 360)
            {
                angle = 0;
            }
        }
    }
}
