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
        // Same state?
        if(m_currentFSMState != null && _nextState != null)
        {
            if (m_currentFSMState.Equals(_nextState) == true)
                return;
        }

		// Notify the current one
		if(m_currentFSMState != null)
		{
            m_currentFSMState.setEnabled(false);
			m_currentFSMState.onLeave();
			m_currentFSMState = null;
		}
		
		// Set next state
		m_currentFSMState = _nextState;
		
		// Notify new state
		if(m_currentFSMState != null)
		{
            m_currentFSMState.setEnabled(true);
			m_currentFSMState.onEnter();
		}
	}
	
	// Override: MonoBehaviour::Start()
	public void Start()
	{
        // Local variables
        FSMState[] states = null;

        // Retrieve all states
        states = gameObject.GetComponentsInChildren<FSMState>();
        foreach (FSMState s in states)
        {
            // Disable states
            s.setEnabled(false);
        }

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