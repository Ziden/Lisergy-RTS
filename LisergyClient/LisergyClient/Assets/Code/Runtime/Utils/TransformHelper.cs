﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public static class TransformExtensions
    {

        public static void TurnTo(this Transform transform, Transform target, float damp = 1f)
        {
            var lookPos = target.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = rotation;
        }


        public static T GetRequiredChildComponent<T>(this Transform t, string name) where T : MonoBehaviour
        {
            var obj = t.FindDeepChild(name);
            if (obj == null)
                throw new Exception($"Could not find {name} child of {t.name} in scene");

            var component = obj.GetComponent<T>();
            if (component == null)
                throw new Exception($"{name} child of {t.name} does not have component {typeof(T).Name}");
            return component;
        }

        public static Transform FindDeepChild(this Transform aParent, string aName)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(aParent);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                if (c.name == aName)
                    return c;
                foreach (Transform t in c)
                    queue.Enqueue(t);
            }
            return null;
        }

        public static IEnumerator LerpFromTo(this Transform transform, Vector3 pos1, Vector3 pos2, float duration)
        {
            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                transform.localPosition = Vector3.Lerp(pos1, pos2, t / duration);
                yield return 0;
            }
            transform.localPosition = pos2;
        }
    }
}
