/*
 * Project:	Billy's Payback
 * File:	TA_DestroyObjectsSilently.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Transition action which will destroy the sepcified objects silently
 */
public class TA_DestroyObjectsSilently : FSMAction
{
    // List with the game-objects
    public GameObject[] m_objects;

    // Override: FSMAction::OnAction()
    override public void onAction()
    {
        // Check
        if (m_objects == null || m_objects.Length == 0)
            return;

        // Destroy objects
        foreach (GameObject obj in m_objects)
            GameObject.Destroy(obj);
    }
}
