using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleFOW
{
    [RequireComponent(typeof(MeshRenderer))]
    public class FogOfWarShaderControl : MonoBehaviour
    {
        
        [Header("Maximum amount of revealing points")]
        public uint maximumPoints = 512;

        [Header("Game Camera")]
        public Camera cam;

        [Header("With test mode enabled : Click on mesh in game view to add points")]
        public bool testMode;

        MeshRenderer rend;

        bool init = false;

        [HideInInspector]
        public List<Vector4> points = new List<Vector4>();

        [HideInInspector]
        public Vector2 meshSize, meshExtents;

        Vector4[] sendBuffer;

        //Initialize required variables
        public void Init()
        {
            rend = GetComponent<MeshRenderer>();
            meshExtents = rend.bounds.extents;
            meshSize = rend.bounds.size;

            points = new List<Vector4>();
            init = true;

            sendBuffer = new Vector4[maximumPoints];
        }

        //Transform world point to UV coordinate of FOW mesh
        public Vector2 WorldPointToMeshUV(Vector2 wp)
        {
            Vector2 toRet = Vector2.zero;

            toRet.x = (transform.position.x - wp.x + meshExtents.x) / meshSize.x;
            toRet.y = (transform.position.y - wp.y + meshExtents.y) / meshSize.y;

            return toRet;
        }

        //Show or hide FOW
        public void SetEnabled(bool on)
        {
            rend.enabled = on;
        }

        //Add revealing point to FOW renderer if amount of points is lower than MAX_POINTS
        public void AddPoint(Vector2 worldPoint)
        {
            if (points.Count < maximumPoints)
                points.Add(WorldPointToMeshUV(worldPoint));
        }

        //Remove FOW revealing point
        public void RemovePoint(Vector2 worldPoint)
        {
            if (points.Contains(WorldPointToMeshUV(worldPoint)))
            {
                points.Remove(WorldPointToMeshUV(worldPoint));
            }
        }

        //Send any change to revealing point list to shader for rendering
        public void SendPoints()
        {
            points.ToArray().CopyTo(sendBuffer, 0);

            rend.material.SetVectorArray("_PointArray", sendBuffer);
            rend.material.SetInt("_PointCount", points.Count);
        }

        //Send new range value to shader
        public void SendRange(float range)
        {
            rend.material.SetFloat("_RadarRange", range);
        }

        //Send new scale value to shader
        public void SendScale(float scale)
        {
            rend.material.SetFloat("_Scale", scale);
        }


        public void Update()
        {

            if (!init)
            {
                Init();
            }

            
            if (testMode && Input.GetMouseButtonDown(0))
            {
                AddPoint(cam.ScreenToWorldPoint(Input.mousePosition));
                SendPoints();
            }
            

        }

    }

}

