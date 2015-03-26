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
    // Override: MonoBehaviour::Start()
    void Start()
    {
        // Set global variable
        if (Game.Instance.ScriptEngine.IsGlobalVar("level_31_visited") == false)
            Game.Instance.ScriptEngine.AddGlobalVar("level_31_visited", true);
    }
}
