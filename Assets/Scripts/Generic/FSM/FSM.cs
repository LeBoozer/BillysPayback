/*
 * Project:	Billy's Payback
 * File:	FSM.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Represents the basic class for the FMS (finite state machine)
 */
public class FSM : MonoBehaviour
{
	// The current FSM-state
	private FSMState 	m_currentFSMState = null;
	
	// The start FSM-state
	public FSMState 	m_startState = null;
	
	// Changes the FSM-state
	public void setFSMState(FSMState _nextState)
	{	
		// Notify the current one
		if(m_currentFSMState != null)
		{
			m_currentFSMState.onLeave();
			m_currentFSMState.setEnabled(false);
			m_currentFSMState = null;
		}
		
		// Set next state
		m_currentFSMState = _nextState;
		
		// Notify new state
		if(m_currentFSMState != null)
		{
			m_currentFSMState.onEnter();
			m_currentFSMState.setEnabled(true);
		}
	}
	
	// Override: MonoBehaviour::Start()
	public void Start()
	{	
		// Start state defined?
		if(m_startState != null)
			setFSMState(m_startState);
	}
	
	// Override: MonoBehaviour::OnDestroy()
	public void OnDestroy()
	{
		// Terminate current state
		setFSMState(null);	
	}
}