using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TriForge
{
    //A very basic top down cam - for demo and testing purposes
    public class TopDownCam : MonoBehaviour
    {
        public float CameraScrollSpeed = 1.0f;
        public float CameraZoomSpeed = 0.5f;
        public GameObject Camera;

        private void MoveCamera()
        {
            //move XZ
                transform.Translate(Input.GetAxis("Horizontal") * CameraScrollSpeed * Time.deltaTime, 0.0f, Input.GetAxis("Vertical") * CameraScrollSpeed * Time.deltaTime);

            //zoom
            Camera.transform.Translate(Vector3.forward * CameraZoomSpeed * Time.deltaTime * Input.GetAxis("Mouse ScrollWheel"));

        }

        private void Update()
        {
            MoveCamera();
        }
    }

}

