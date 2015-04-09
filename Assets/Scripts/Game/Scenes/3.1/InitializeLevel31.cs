/*
 * Project:	Billy's Payback
 * File:	InitializeLevel31.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Initializes the level 3.1
 */
public class InitializeLevel31 : MonoBehaviour
{
    // The game object of the background particle
    public GameObject m_backgroundParticle = null;

    // Transition for the pursuit of the two birds
    public GameObject m_transitionPursuit = null;

    // The two birds for the pursuit
    public GameObject m_pursuitBird1 = null;
    public GameObject m_pursuitBird2 = null;

    // Override: MonoBehaviour::Start()
    void Start()
    {
        // Local variables
        bool visited = Game.Instance.ScriptEngine.IsGlobalVar("level_31_visited");
        AntialiasingAsPostEffect dlaa = GameObject.FindObjectOfType<AntialiasingAsPostEffect>();
        SSAOEffect ssao = GameObject.FindObjectOfType<SSAOEffect>();

        // Set global variable
        if (visited == false)
            Game.Instance.ScriptEngine.AddGlobalVar("level_31_visited", true);

        // Disable background particles if quality level is simple or below
        if (m_backgroundParticle != null && QualityLevel.getCurrentLevelIndex() <= QualityLevel.LEVEL_INDEX_SIMPLE)
            m_backgroundParticle.SetActive(false);

        // Disable DLAA if quality level is below beautiful
        if (dlaa != null && QualityLevel.getCurrentLevelIndex() < QualityLevel.LEVEL_INDEX_BEAUTIFUL)
            dlaa.enabled = false;

        // Disable SSAO if quality level is below beautiful
        if (ssao != null && QualityLevel.getCurrentLevelIndex() < QualityLevel.LEVEL_INDEX_BEAUTIFUL)
            ssao.enabled = false;

        // Delete pursuit of the two birds if the level was already visited by the player
        if (visited == true && Game.Instance.ScriptEngine.IsGlobalVar("level_32_visited"))
        {
            if (m_transitionPursuit != null)
                GameObject.Destroy(m_transitionPursuit);
            if (m_pursuitBird1 != null)
                GameObject.Destroy(m_pursuitBird1);
            if (m_pursuitBird2 != null)
                GameObject.Destroy(m_pursuitBird2);
        }
    }
}
