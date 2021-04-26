using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightManager : MonoBehaviour
{
    public Light2D headLight;

    public float groundCrustLight;
    public float mantleLight;
    public float coreLight;
    public float voidLight;

    void Start()
    {
        
    }

    void Update()
    {
        switch(LevelManager.levelSection)
        {
            case LevelSection.GROUND:
            case LevelSection.CRUST:
                headLight.intensity = groundCrustLight;
                break;
            case LevelSection.MANTLE:
                headLight.intensity = mantleLight;
                break;
            case LevelSection.CORE:
                headLight.intensity = coreLight;
                break;
            case LevelSection.BOTTOM:
                headLight.intensity = voidLight;
                break;
        }
        
        }
}
