using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LowPolyMedievalCastlePack
{
    /// <summary>
    /// Spins an object along its local Z axis by a defined spin speed value. Used to spin the propeller of the WindmillBase object.
    /// </summary>
    public class PropellerSpin : MonoBehaviour
    {
        public float spinSpeed = 13;

        // Update is called once per frame
        void Update()
        {
            transform.RotateAround(transform.position, transform.forward, Time.deltaTime * -spinSpeed);
        }
    }
}
