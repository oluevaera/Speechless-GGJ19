using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class WordStolenEffect : MonoBehaviour
{
    [SerializeField]
    private PostProcessVolume postProcessVolume;
    private ChromaticAberration chromaticAberration;

    [SerializeField]
    private float effectLength = 1f;
    [SerializeField]
    private AnimationCurve effectCurve;

    private void Start()
    {
		if(QualitySettings.GetQualityLevel() < 3) {
			//Disables PostPro on low quality levels.
			postProcessVolume.enabled = false;
			Destroy(this);
		}

        if (postProcessVolume.profile.TryGetSettings(out chromaticAberration)) 
        {
            Debug.Log("Got Chromatic Aberration");
        } else
        {
            Debug.LogError("Failed to get Chromatic Aberration!");
        }

        EventManager.AddListener("WORD_LOST", OnWordLost);
    }

    private void OnWordLost()
    {
        StartCoroutine("ChromaticEffect");
    }

    private IEnumerator ChromaticEffect()
    {
        for (float t = 0; t < effectLength; t += Time.deltaTime)
        {
            chromaticAberration.intensity.value = effectCurve.Evaluate(t / effectLength);
            yield return new WaitForEndOfFrame();
        }
        chromaticAberration.intensity.value = 0;
    }
}
