using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TriForge
{
    [ExecuteAlways]
    public class TopDownWind : MonoBehaviour
    {
        [Range(0.0f, 1.0f)]
        public float WindStrength = 0.3f;

        private Vector3 WindDirection;

        private void Update()
        {
            WindDirection = transform.right.normalized;
            Shader.SetGlobalVector("TD_WindDirection", WindDirection);
            Shader.SetGlobalFloat("TD_WindStrength", WindStrength);
        }
    }
}


