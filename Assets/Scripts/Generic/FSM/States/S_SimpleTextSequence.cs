/*
 * Project:	Billy's Payback
 * File:	S_SimpleTextSequence.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Displays text to the player via the interface (supports simple decisions)
 */
public class S_SimpleTextSequence : FSMState
{
	// The text source
	public TextAsset m_textSource = null;

	// Override: FSMState::Awake()
	void Awake()
	{
		// Text to display?
		if(m_textSource == null)
		{
			Debug.LogError("Invalid text FSM-state (" + gameObject.name + "). No text has been set!");
			return;
		}
	}

	// Override: FSMState::OnEnable()
	void OnEnable()
	{	
	}

	// Override: FSMState::OnDisable()
	void OnDisable()
	{
	}
}
