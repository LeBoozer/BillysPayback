/*
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
	
	// Enables/disables the state
	public void setEnabled(bool _onOff)
	{
		gameObject.SetActive(_onOff);
	}
	
	// Will be called as soon as the state is being entered
	public void onEnter()
	{
		// Notify actions
		foreach(FSMStateAction a in m_actions)
			a.onActionEnter();
	}
	
	// Will be called as soon as the state is being entered
	public void onLeave()
	{
		// Notify actions
		foreach(FSMStateAction a in m_actions)
			a.onActionLeave();
	}

	// Override: MonoBehaviour::Awake()
	void Awake()
	{
		// Local variables
		FSMTransition trans 	= null;
		FSMStateAction action 	= null;
	
		// Retrieve all transitions
		foreach(Transform child in gameObject.transform)
		{
			// Get transition component
			trans = child.gameObject.GetComponent<FSMTransition>();
			if(trans != null)
				m_transitions.Add(trans);
			
			// Get action component
			action = child.gameObject.GetComponent<FSMStateAction>();
			if(action != null)
				m_actions.Add(action);
		}	
	
		// Disable this state with all its transitions
		setEnabled(false);		
	}
	
	// Override: MonoBehaviour::Start()
	void Start()
	{
		// Retrieve FSM instance
		m_fsm = gameObject.transform.parent.gameObject.GetComponent<FSM>();
	}
}