/*
 * Project:	Billy's Payback
 * File:	InitializeLevel32.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Initializes the level 3.2
 */
public class InitializeLevel32 : MonoBehaviour
{
    // Higgins
    public Higgins m_higgins = null; 

    // Override: MonoBehaviour::Start()
    void Start()
    {
        // Shield from Bob collected?
        if (m_higgins != null && Game.Instance.ScriptEngine.IsGlobalVar("level_2_bob_received_shield") == true)
            m_higgins.gameObject.SetActive(true);
    }
}
