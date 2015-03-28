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

    // Override: MonoBehaviour::Start()
    void Start()
    {
        // Set global variable
        if (Game.Instance.ScriptEngine.IsGlobalVar("level_31_visited") == false)
            Game.Instance.ScriptEngine.AddGlobalVar("level_31_visited", true);

        // Disable background particles if quality level is simple or below
        if (m_backgroundParticle != null && QualityLevel.getCurrentLevelIndex() <= QualityLevel.LEVEL_INDEX_SIMPLE)
            m_backgroundParticle.SetActive(false);
    }
}
