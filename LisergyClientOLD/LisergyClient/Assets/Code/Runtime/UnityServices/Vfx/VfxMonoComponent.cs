using GameAssets;
using UnityEngine;

public class VfxMonoComponent : MonoBehaviour
{
    public VfxPrefab Effect { get; set; }

    public void StartEffect(Vector3 pos)
    {
        if (gameObject == null) return;
        transform.position = pos;
        gameObject.SetActive(true);
        foreach (var particle in GetComponentsInChildren<ParticleSystem>(true))
        {
            particle.Stop();
            particle.time = 0;
            var main = particle.main;
            particle.gameObject.SetActive(true);
            main.stopAction = ParticleSystemStopAction.Disable;
        }
        var rootParticle = GetComponent<ParticleSystem>();
        if (rootParticle == null) return;
        rootParticle.Stop();
        rootParticle.gameObject.SetActive(true);
        var rootMain = rootParticle.main;
        rootMain.stopAction = ParticleSystemStopAction.Disable;
        rootParticle.time = 0;
        rootParticle.Play();
    }
}