/*
 * Project:	Billy's Payback
 * File:	InitializeLevel11.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Initializes the level 1.1
 */
public class InitializeLevel11 : MonoBehaviour
{
    // Override: MonoBehaviour::Start()
    void Start()
    {
        // Local variables
        AntialiasingAsPostEffect dlaa = GameObject.FindObjectOfType<AntialiasingAsPostEffect>();
        SSAOEffect ssao = GameObject.FindObjectOfType<SSAOEffect>();
        FastBloom bloom = GameObject.FindObjectOfType<FastBloom>();

        // Set global variable
        if (Game.Instance.ScriptEngine.IsGlobalVar("level_11_visited") == false)
            Game.Instance.ScriptEngine.AddGlobalVar("level_11_visited", true);

        // Disable DLAA if quality level is below beautiful
        if (dlaa != null && QualityLevel.getCurrentLevelIndex() < QualityLevel.LEVEL_INDEX_BEAUTIFUL)
            dlaa.enabled = false;

        // Disable SSAO if quality level is below beautiful
        if (ssao != null && QualityLevel.getCurrentLevelIndex() < QualityLevel.LEVEL_INDEX_BEAUTIFUL)
            ssao.enabled = false;

        // Disable bloom if quality level is below fantastic
        if (bloom != null && QualityLevel.getCurrentLevelIndex() < QualityLevel.LEVEL_INDEX_FANTASTIC)
            bloom.enabled = false;
    }
}
