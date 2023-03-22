using System;
using System.Collections;
using UnityEngine;

namespace Assets.UnitTests.Stubs
{
    public class Wait
    {
        public static IEnumerator Until(Func<bool> condition, string error, float timeout = 10f)
        {
            float timePassed = 0f;
            while (!condition() && timePassed < timeout)
            {
                yield return new WaitForEndOfFrame();
                timePassed += Time.deltaTime;
            }
            if (timePassed >= timeout)
            {
                throw new TimeoutException(error);
            }
        }
    }
}
