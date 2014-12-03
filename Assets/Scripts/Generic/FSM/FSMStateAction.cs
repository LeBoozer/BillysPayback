/*
 * Project:	Billy's Payback
 * File:	FSMStateAction.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Represents the basic state-action for the FSM
 */
public abstract class FSMStateAction : MonoBehaviour
{
	// Will be called as soon as the action is being triggered on enter events of the parental state
	abstract public void onActionEnter();
	
	// Will be called as soon as the action is being triggered on leave events of the parental state
	abstract public void onActionLeave();
	
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
