/*
 * Project:	Billy's Payback
 * File:	TA_LoadLevel.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Transition action which will load the specified level on action
 */
public class TA_LoadLevel : FSMAction
{
    // The level name
    public string m_levelName = "";

    // Override: FSMAction::OnAction()
    override public void onAction()
    {
        // Load level
        if (m_levelName != null && m_levelName.Length > 0)
            Application.LoadLevel(m_levelName);
    }
}
