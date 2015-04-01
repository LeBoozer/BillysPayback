/*
 * Project:	Billy's Payback
 * File:	InitializeLevel21.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Initializes the level 2.1
 */
public class InitializeLevel21 : MonoBehaviour
{
    // The destroyed entry
    public DestroyEntry m_destroyEntry = null;

    // The game object of the background particle
    public GameObject   m_backgroundParticle = null;

    // Override: MonoBehaviour::Start()
    void Start()
    {
        // Local variables
        AntialiasingAsPostEffect dlaa = GameObject.FindObjectOfType<AntialiasingAsPostEffect>();
        SSAOEffect ssao = GameObject.FindObjectOfType<SSAOEffect>();
        FastBloom bloom = GameObject.FindObjectOfType<FastBloom>();

        // Set global variable
        if (Game.Instance.ScriptEngine.IsGlobalVar("level_21_visited") == false)
            Game.Instance.ScriptEngine.AddGlobalVar("level_21_visited", true);

        // Destroy entry if necessary
        if (m_destroyEntry != null && Game.Instance.ScriptEngine.IsGlobalVar("level_21_cave_destroyed") == true)
            m_destroyEntry.onActivate();

        // Disable background particles if quality level is simple or below
        if (m_backgroundParticle != null && QualityLevel.getCurrentLevelIndex() <= QualityLevel.LEVEL_INDEX_SIMPLE)
            m_backgroundParticle.SetActive(false);

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
