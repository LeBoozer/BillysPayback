/*
 * Project:	Billy's Payback
 * File:	T_OnTimeout.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * The transition will be triggered on time-out events
 */
public class T_OnTimeout : FSMTransition
{
	// True if the time-out has been triggered
	private bool 	m_done 				= false;

	// Time-out in seconds
	public float 	m_timeOutInSeconds 	= 0.0f;
	
	// Override: FSMTransition::Update
	void Update()
	{
		// Adjust remaining time
		m_timeOutInSeconds -= Time.deltaTime;
		if(m_timeOutInSeconds < 0 && m_done == false)
		{
			// Set flag
			m_done = true;
			
			// Change to target state
			setTargetFSMState();
		}
	}
}
