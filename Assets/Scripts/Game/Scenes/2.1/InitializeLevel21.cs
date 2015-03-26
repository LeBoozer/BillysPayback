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

    // Override: MonoBehaviour::Start()
    void Start()
    {
        // Set global variable
        if (Game.Instance.ScriptEngine.IsGlobalVar("level_21_visited") == false)
            Game.Instance.ScriptEngine.AddGlobalVar("level_21_visited", true);

        // Destroy entry if necessary
        if (m_destroyEntry != null && Game.Instance.ScriptEngine.IsGlobalVar("level_21_cave_destroyed") == true)
            m_destroyEntry.onActivate();
    }
}
