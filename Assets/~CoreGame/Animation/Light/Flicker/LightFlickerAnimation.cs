using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlickerAnimation : MonoBehaviour
{

    public float MaxIntensity;
    public float MinIntensity;
    public float MaxRange;
    public float MinRange;
    public float MaxChangeDeltaTime;
    public float MinChangeDeltaTime;

    #region External Dependencies
    private Light lightComponent;
    #endregion

    private float currentChangeDeltaTime;
    private float elapsedTime;


    private void Start()
    {
        this.currentChangeDeltaTime = Random.Range(MinChangeDeltaTime, MaxChangeDeltaTime);
        this.lightComponent = GetComponent<Light>();
        this.RandomizeLightValues();
    }

    void Update()
    {
        this.elapsedTime += Time.deltaTime;
        if(this.elapsedTime >= this.currentChangeDeltaTime)
        {
            this.elapsedTime -= this.currentChangeDeltaTime;
            this.currentChangeDeltaTime = Random.Range(MinChangeDeltaTime, MaxChangeDeltaTime);
            this.RandomizeLightValues();
        }
    }

    private void RandomizeLightValues()
    {
        this.lightComponent.range = Random.Range(MinRange, MaxRange);
        this.lightComponent.intensity = Random.Range(MinIntensity, MaxIntensity);
    }
}
