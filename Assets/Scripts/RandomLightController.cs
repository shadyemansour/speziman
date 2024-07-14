using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CartoonFX;

public class RandomLightController : MonoBehaviour
{
    public float minDelay = 0.1f;
    public float maxDelay = .3f;

    private GameObject[] lights;

    void Awake()
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            var effect = this.transform.GetChild(i).gameObject;
            list.Add(effect);

            var cfxrEffect = effect.GetComponent<CFXR_Effect>();
            if (cfxrEffect != null) cfxrEffect.clearBehavior = CFXR_Effect.ClearBehavior.Disable;
        }
        lights = list.ToArray();

    }
    public void StartLights()
    {
        StartCoroutine(ToggleRandomEffect());
    }

    public void StopLights()
    {
        StopAllCoroutines();
        foreach (GameObject effect in lights)
        {
            effect.SetActive(false);
        }
    }

    IEnumerator ToggleRandomEffect()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));

            if (lights.Length > 0)
            {
                // Pick a random effect object and toggle its active state
                int index = Random.Range(0, lights.Length);
                GameObject selectedEffect = lights[index];
                selectedEffect.SetActive(true);

                // Optional: Debug log to track which effect is toggled
                Debug.Log("Toggled effect: " + selectedEffect.name + " to " + selectedEffect.activeSelf);
            }
        }
    }
}