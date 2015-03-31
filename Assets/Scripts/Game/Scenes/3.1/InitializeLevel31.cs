﻿/*
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

        // Set global variable
        if (visited == false)
            Game.Instance.ScriptEngine.AddGlobalVar("level_31_visited", true);

        // Disable background particles if quality level is simple or below
        if (m_backgroundParticle != null && QualityLevel.getCurrentLevelIndex() <= QualityLevel.LEVEL_INDEX_SIMPLE)
            m_backgroundParticle.SetActive(false);

        // Delete pursuit of the two birds if the level was already visited by the player
        if(visited == true)
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
