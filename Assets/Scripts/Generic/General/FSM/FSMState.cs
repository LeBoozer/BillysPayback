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

	// Override: MonoBehaviour::Awake()
	void Awake()
	{
		// Local variables
		FSMTransition trans = null;	
	
		// Retrieve all transitions
		foreach(Transform child in gameObject.transform)
		{
			// Get component
			trans = child.gameObject.GetComponent<FSMTransition>();
			if(trans == null)
				continue;
			
			// Add to list
			m_transitions.Add(trans);
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