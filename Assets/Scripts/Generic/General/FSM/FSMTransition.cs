/*
 * Project:	Billy's Payback
 * File:	FSMTransition.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Represents the basic transition for the FSM
 */
public abstract class FSMTransition : MonoBehaviour
{	    
	// The host FSM-state
	protected FSMState 	m_hostState = null;
	
	// The FSM instance
	protected FSM 		m_fsm = null;
	
	// The target FSM-state
	public FSMState 	m_targetState = null;
	
	// Sets the target FSM-state 
	protected void setTargetFSMState()
	{
		// Set...
		if(m_fsm != null)
			m_fsm.setFSMState(m_targetState);
	}
	
	// Override: MonoBehaviour::Start()
	private void Start()
	{	
		// Retrieve FSM instance
		m_fsm = gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponent<FSM>();	
		
		// Retrieve host FSM-state
		m_hostState = gameObject.transform.parent.gameObject.GetComponent<FSMState>();
	}
}