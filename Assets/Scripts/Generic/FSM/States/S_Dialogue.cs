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

    // The conversation ID (-1 to use the first found conversation)
    public int                  m_conversationID = -1;

    // The parsed dialogue
    private AdvancedDialogue    m_dialogue = null;

    // The current conversation
    private Conversation        m_conversation = null;

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

        // Get specified conversation
        if (m_conversationID == -1)
            m_conversation = m_dialogue.getConversationByID(m_dialogue.getConversationIDs()[0]);
        else
            m_conversation = m_dialogue.getConversationByID(m_conversationID);
        if(m_conversation == null)
        {
            Debug.LogError("Invalid text FSM-state (" + gameObject.name + "). Specified conversation has not been found!");
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
