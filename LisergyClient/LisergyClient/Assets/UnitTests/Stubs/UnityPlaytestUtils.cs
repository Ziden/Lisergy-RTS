using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Assets.UnitTests.Stubs
{
    public class UnityPlaytestUtils
    {
        public static IEnumerator LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            yield return new WaitForSeconds(1);
            Debug.Log("Scene loaded");
        }
    }
}
