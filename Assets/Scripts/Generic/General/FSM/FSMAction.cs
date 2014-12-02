/*
 * Project:	Billy's Payback
 * File:	FSMAction.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Represents the basic action for the FSM
 */
public abstract class FSMAction : MonoBehaviour
{	
	// Will be called as soon as the action is being triggered
	abstract public void onAction();

	// Enables/disables the action
	public void setEnabled(bool _onOff)
	{
		gameObject.SetActive(_onOff);
	}
	
	// Override: MonoBehaviour::Awake()
	void Awake()
	{		
		// Disable this action
		setEnabled(false);		
	}
}