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
        headLight.intensity = LevelManager.distanceTraveled < LevelManager.sectionDistance * 3 ?
            (LevelManager.sectionDistance * 3 - LevelManager.distanceTraveled) / (LevelManager.sectionDistance * 3) : 
            0;
        /*
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
            case LevelSection.VOID:
                headLight.intensity = voidLight;
                break;
        }*/
        
        }
}
