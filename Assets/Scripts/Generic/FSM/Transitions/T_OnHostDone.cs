/*
 * Project:	Billy's Payback
 * File:	T_OnHostDone.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * The transition will be triggered on host-done events
 */
public class T_OnHostDone : FSMTransition
{
	// Override: FSMTransition::onHostStateDone
	public override void onHostStateDone(Object _param)
	{
		// Change to target state
		setTargetFSMState();
	}
}
