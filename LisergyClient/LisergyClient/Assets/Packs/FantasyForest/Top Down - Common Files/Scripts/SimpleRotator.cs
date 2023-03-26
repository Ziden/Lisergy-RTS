using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TriForge
{
    public class SimpleRotator : MonoBehaviour
    {
        public float RotationSpeed = 1.0f;
        public bool RotateX = false;
        public bool RotateY = false;
        public bool RotateZ = false;

        void RotateObject()
        {
            gameObject.transform.Rotate(RotationSpeed * (RotateX ? 1 : 0) * Time.deltaTime, RotationSpeed * (RotateY ? 1 : 0) * Time.deltaTime, RotationSpeed * (RotateZ ? 1 : 0) * Time.deltaTime);
        }

        void Update()
        {
            RotateObject();
        }
    }
}
