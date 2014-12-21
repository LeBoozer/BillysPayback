/*
 * Project:	Billy's Payback
 * File:	S_Dialogue.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Displays a dialogue to the player (supports decisions)
 */
public class S_Dialogue : FSMState
{
    // The text source
    public TextAsset            m_textSource = null;

    // The parsed dialogue
    private AdvancedDialogue    m_dialogue = null;

    // Override: FSMState::Awake()
    protected override void Awake()
    {
        // Call parent
        base.Awake();

        // Text to display?
        if (m_textSource == null)
        {
            Debug.LogError("Invalid text FSM-state (" + gameObject.name + "). No text has been set!");
            return;
        }

        // Parse dialog
        m_dialogue = AdvancedDialogueParser.parseDialog(m_textSource.text);
        if (m_dialogue == null || m_dialogue.Valid == false)
        {
            Debug.LogError("Invalid text FSM-state (" + gameObject.name + "). Text data is invalid!");
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
