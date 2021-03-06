﻿/*
 * Project:	Billy's Payback
 * File:	FSMState.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Represents the basic state for the FSM
 */
public abstract class FSMState : MonoBehaviour
{
	// The FSM
	private FSM 					m_fsm = null;
	
	// List with all transitions for this FSM-state
	private List<FSMTransition> 	m_transitions = new List<FSMTransition>();
	
	// List of actions which will be activated as soon as the state is being entered or left
	public List<FSMStateAction> 	m_actions = new List<FSMStateAction>();
	
	// Returns the FSM instance
	public FSM 	getFSMInstance()
	{
		return m_fsm;
	}
	
	// Can be called to inform all attached transitions that the state's work has been done
	protected void onNotifyDone(object _param)
	{
		// Inform transitions
		foreach(FSMTransition t in m_transitions)
			t.onHostStateDone(_param);
	}
	
	// Enables/disables the state
	public void setEnabled(bool _onOff)
	{
		gameObject.SetActive(_onOff);
        foreach (FSMTransition t in m_transitions)
        {
            if (t != null)
                t.setEnabled(_onOff);
        }
	}
	
	// Will be called as soon as the state is being entered
	public virtual void onEnter()
	{
		// Notify actions
		foreach(FSMStateAction a in m_actions)
			a.onActionEnter();
	}
	
	// Will be called as soon as the state is being entered
	public virtual void onLeave()
	{
		// Notify actions
		foreach(FSMStateAction a in m_actions)
			a.onActionLeave();
	}

	// Override: MonoBehaviour::Awake()
	protected virtual void Awake()
	{
		// Local variables
		FSMTransition[] trans 	= null;
		FSMStateAction[] action = null;

		// Retrieve all transitions
        trans = gameObject.GetComponentsInChildren<FSMTransition>();
        m_transitions.AddRange(trans);

        // Retrieve all actions
        action = gameObject.GetComponentsInChildren<FSMStateAction>();
        m_actions.AddRange(action);		
	}
	
	// Override: MonoBehaviour::Start()
	protected virtual void Start()
	{
		// Retrieve FSM instance
        m_fsm = gameObject.GetComponentInParent<FSM>();
	}
}