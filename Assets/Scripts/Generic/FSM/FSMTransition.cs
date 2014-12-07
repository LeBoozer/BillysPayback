/*
 * Project:	Billy's Payback
 * File:	FSMTransition.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Represents the basic transition for the FSM
 */
public abstract class FSMTransition : MonoBehaviour
{	    
	// The host FSM-state
	protected FSMState 		m_hostState = null;
	
	// The FSM instance
	protected FSM 			m_fsm = null;
	
	// The target FSM-state
	public FSMState 		m_targetState = null;
	
	// List of actions which will be activated as soon as the transition is being activated
	public List<FSMAction>	m_actionsOnTransition = new List<FSMAction>();
	
	// Sets the target FSM-state 
	protected void setTargetFSMState()
	{
		// Trigger actions
		foreach(FSMAction a in m_actionsOnTransition)
			a.onAction();
	
		// Set state
		if(m_fsm != null)
			m_fsm.setFSMState(m_targetState);
	}
	
	// Will be called as soon as the host-state has finished its work (this call is not guaranteed and depends heavily on the host state!)
	public virtual void onHostStateDone(object _param)
	{
	}
	
	// Override: MonoBehaviour::Awake()
	void Awake()
	{
		// Local variables
		FSMAction action = null;
		
		// Retrieve all actions
		foreach(Transform child in gameObject.transform)
		{
			// Get component
			action = child.gameObject.GetComponent<FSMAction>();
			if(action == null)
				continue;
			
			// Add to list
			m_actionsOnTransition.Add(action);
		}		
	}
	
	// Override: MonoBehaviour::Start()
	void Start()
	{	
		// Retrieve FSM instance
		m_fsm = gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponent<FSM>();	
		
		// Retrieve host FSM-state
		m_hostState = gameObject.transform.parent.gameObject.GetComponent<FSMState>();
	}
}