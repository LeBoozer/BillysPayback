/*
 * Project:	Billy's Payback
 * File:	T_OnDialogAction.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * The transition will be triggered on host-done events
 */
public class T_OnDialogAction : FSMTransition
{
	// The target descision
	public SimpleDialogue.Action[] m_targetActions;

	// Override: FSMTransition::onHostStateDone
	public override void onHostStateDone(object _param)
	{
        // Dialog action?
        if (m_targetActions != null || _param is SimpleDialogue.Action == false)
            return;

		// Right descision?
		foreach(SimpleDialogue.Action a in m_targetActions)
		{
            // Compare
            if (a.Equals(_param) == true)
            {
                // Change to target state
                setTargetFSMState();
                break;
            }
		}
	}
}