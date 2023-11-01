using UnityEngine;
using System.Collections;


public class AtmoXfade : MonoBehaviour 
{
	
	public Material skyMat;
	public Color skyBright = Color.grey;
	public Color skyDark = Color.black;
	//Color sCache = Color.white;
	
	public Light dirLight;
	public Color lightBright = Color.grey;
	public Color lightDark = Color.black;
	//Color lCache = Color.white;
	
	public float minLightIntensity = 0.2f;
	public float maxLightIntensity = 0.85f;
	float curIntensity = 0.0f;
	
	public bool useRenderFog = true;
	public Color fogBright = Color.grey;
	public Color fogDark = Color.black;
	//Color fCache = Color.white;
	
	public float minFog = 0.004f;
	public float maxFog = 0.02f;
	//float curFog = 0.0f;
	
	public enum FadeState 
	{
		FadeDark,
		FadeBright
	}
	public FadeState fadeState = FadeState.FadeBright;
	
	public float fadeTime = 80.0f; // 400 
	
	void Start () 
	{
		if (skyMat)
			skyMat.SetColor("_Tint", skyBright);
		
		if (dirLight)
			dirLight.color = lightBright;
		
		if (useRenderFog)
		{
			RenderSettings.fog = true;
			RenderSettings.fogColor = fogBright;
		}
		else
			RenderSettings.fog = false;
		curIntensity = maxLightIntensity;
	}
	
	
	
	
	void OnTriggerEnter (Collider c)
	{
		if (c.sharedMaterial != null && c.sharedMaterial.name == "Player")
		{
			fadeState = FadeState.FadeDark;
			StartCoroutine (FadeDark());
		}
	}
	
	void OnTriggerExit (Collider c)
	{
		if (c.sharedMaterial != null && c.sharedMaterial.name == "Player")
		{
			fadeState = FadeState.FadeBright;
			StartCoroutine (FadeBright());
		}
	}
	
	
	
	IEnumerator FadeDark ()
	{
		float t = 0.00001f;
		
		while (fadeState == FadeState.FadeDark && curIntensity > minLightIntensity)
		{
			skyMat.SetColor ("_Tint", Color.Lerp (skyMat.GetColor("_Tint"), skyDark, t));
			
			dirLight.color = Color.Lerp (dirLight.color, lightDark, t);
			curIntensity = dirLight.intensity;
			dirLight.intensity = Mathf.SmoothStep (curIntensity, minLightIntensity, t);
			
			if (useRenderFog)
			{
				RenderSettings.fogColor = Color.Lerp (RenderSettings.fogColor, fogDark, t);
				RenderSettings.fogDensity = Mathf.SmoothStep (RenderSettings.fogDensity, maxFog, t);
			}
			
			yield return null;
			t += Time.deltaTime / fadeTime;
		}
	}
	 
	IEnumerator FadeBright ()
	{
		float t = 0.00001f;
		
		while (fadeState == FadeState.FadeBright && curIntensity < maxLightIntensity)
		{
			skyMat.SetColor ("_Tint", Color.Lerp (skyMat.GetColor("_Tint"), skyBright, t));
			
			dirLight.color = Color.Lerp (dirLight.color, lightBright, t);
			curIntensity = dirLight.intensity;
			dirLight.intensity = Mathf.SmoothStep (curIntensity, maxLightIntensity, t);
			
			if (useRenderFog)
			{
				RenderSettings.fogColor = Color.Lerp (RenderSettings.fogColor, fogBright, t);
				RenderSettings.fogDensity = Mathf.SmoothStep (RenderSettings.fogDensity, minFog, t);
			}
			
			yield return null;
			t += Time.deltaTime / fadeTime;
		}
	}
}
