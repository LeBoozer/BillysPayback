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
	public TextAsset 		m_textSource 	= null;

	// The parsed dialogue
	private SimpleDialogue 	m_dialogue 		= null;

	// Override: FSMState::Awake()
	protected override void Awake()
	{
		// Call parent
		base.Awake();
		
		// Text to display?
		if(m_textSource == null)
		{
			Debug.LogError("Invalid text FSM-state (" + gameObject.name + "). No text has been set!");
			return;
		}
		
		// Parse dialog
		m_dialogue = SimpleDialogueParser.parseText(m_textSource.text);
		if(m_dialogue == null || m_dialogue.isValid() == false)
		{
			Debug.LogError("Invalid text FSM-state (" + gameObject.name + "). Text data is invalid!");
			return;		
		}
	}

	// Override: FSMState::OnEnable()
	void OnEnable()
	{
		onNotifyDone(SimpleDialogue.Action.YES);
	}

	// Override: FSMState::OnDisable()
	void OnDisable()
	{
	}
}
