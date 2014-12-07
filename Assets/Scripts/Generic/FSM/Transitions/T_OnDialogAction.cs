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
	public SimpleDialogue.Action m_targetAction;

	// Override: FSMTransition::onHostStateDone
	public override void onHostStateDone(object _param)
	{
		// Right descision?
		if(_param is SimpleDialogue.Action && m_targetAction.Equals(_param) == true)
		{
			// Change to target state
			setTargetFSMState();
		}
	}
}