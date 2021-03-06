﻿/*
 * Project:	Billy's Payback
 * File:	S_Dialogue.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Timers;

/*
 * Displays a dialogue to the player (supports decisions)
 */
public class S_Dialogue : FSMState
{
    // The text source
    public TextAsset            m_textSource = null;

    // The conversation ID (-1 to use the first found conversation)
    public int                  m_conversationID = -1;

    // True to block the player movement
    public bool                 m_blockPlayerMovement = true;

    // True to close the dialog window on leave
    public bool                 m_closeDialogWindowOnLeave = false;

    // The parsed dialogue
    private AdvancedDialogue    m_dialogue = null;

    // The current conversation
    private Conversation        m_conversation = null;

    // The current dialog text
    private DialogueText        m_text = null;

    // The index of the current text-part
    private int                 m_currentTextPartIndex = -1;

    // The current on-screen text object
    private Transform           m_currentOnScreenTextObject = null;

    // True if we are ready to show the next text part
    private bool                m_isHandleNextTextPart = false;

    // The dialog window instance
    private DialogueWindowScript m_window = null;

    // The timer object
    private Timer                m_timer = null;

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

    // Override: MonoBehaviour::Start()
    protected override void Start()
    {
        // Call parent
        base.Start();
    }

    // Override: MonoBehaviour::Update()
    void Update()
    {
        // Next text-part?
        if(m_isHandleNextTextPart == true)
        {
            m_isHandleNextTextPart = onNextTextPart();
        }
    }

    // Override: FSMState::onEnter()
    public override void onEnter()
    {
        // Local variables
        int startConvID = -1;

        // Call parent
 	    base.onEnter();

        // Get instance of the dialog window
        m_window = GameObject.Find("Dialog").GetComponent<DialogueWindowScript>();
        if (m_window == null)
        {
            Debug.LogError("Dialog window has not been found!");
            return;
        }

        // Block player?
        if (m_blockPlayerMovement == true)
            Game.Instance.Player.blockMovement(true);

        // Get specified conversation
        startConvID = m_dialogue.GetOverrideStartConversationID;
        if (startConvID != -1)
            m_conversation = m_dialogue.getConversationByID(startConvID);
        else
        {
            if (m_conversationID == -1)
                m_conversation = m_dialogue.getConversationByID(m_dialogue.getConversationIDs()[0]);
            else
                m_conversation = m_dialogue.getConversationByID(m_conversationID);
        }
        if (m_conversation == null)
        {
            Debug.LogError("Invalid text FSM-state (" + gameObject.name + "). Specified conversation has not been found!");
            return;
        }

        // Execute all scripts with a run-type: awake
        m_dialogue.executeScriptFromRunType(DynamicScript.RunType.RUN_AWAKE);

        // Set text
        setTextByID(m_conversation.StartTextID);
    }

    // Override: FSMState::onLeave()
    public override void onLeave()
    {
        // Call parent
        base.onLeave();

        // Kill timer
        if (m_timer != null)
        {
            m_timer.Stop();
            m_timer = null;
        }

        // Delete on-screen text
        if (m_currentOnScreenTextObject != null)
        {
            GameObject.Destroy(m_currentOnScreenTextObject.gameObject);
            m_currentOnScreenTextObject = null;
        }

        // Unregister event callback
        DialogTextScript.AnswerClicked -= onChoiceClicked;

        // Delete choices
        if (m_window != null)
            m_window.removeAnswers();

        // Close window?
        if (m_closeDialogWindowOnLeave == true && m_window != null)
            m_window.closeDialogWindow();

        // Unblock the player movement?
        if (m_blockPlayerMovement == true)
            Game.Instance.Player.blockMovement(false);
    }

    /**
     * Set the active text instance
     */
    private bool setTextByID(int _id)
    {
        // Get start text, text-part
        m_currentTextPartIndex = -1;
        m_text = m_conversation.getTextByID(_id);
        if (m_text == null)
        {
            Debug.LogError("Invalid text FSM-state (" + gameObject.name + "). Specified text has not been found!");
            return false;
        }

        // Delete choices and close window
        if (m_window != null)
        {
            m_window.removeAnswers();
            m_window.closeDialogWindow();
        }

        // Update text on next update run
        m_isHandleNextTextPart = true;

        return true;
    }

    /**
     * Handles the timing and displaying of the text-parts.
     * Returns true to indicate new text is available to be handled, otherwise false
     */
    private bool onNextTextPart()
    {
        // Local variables
        TextPart part = null;

        // Increase index
        ++m_currentTextPartIndex;

        // Reached or hidden?
        if ((m_text.IsShowParts == false || m_currentTextPartIndex >= m_text.TextPartCount) && m_isHandleNextTextPart == true)
        {
            // Display choices
            if(onDisplayChoices() == false)
            {
                // Auto generated/executed choice available
                if(m_text.AutoChoiceType != DialogueText.ChoiceType.CHOICE_NONE)
                {
                    // Exit?
                    if (m_text.AutoChoiceType == DialogueText.ChoiceType.CHOICE_EXIT)
                        onConversationExit(m_text.ExitValue);
                    else
                    {
                        if (!setTextByID(m_text.NextTextID))
                            onConversationExit(AdvancedDialogue.DIALOGUE_NO_CHOICE_EXIT_VALUE);
                        else
                            return true;
                    }

                    return false;
                }

                // No choices available!
                onConversationExit(AdvancedDialogue.DIALOGUE_NO_CHOICE_EXIT_VALUE);
            }
            return false;
        }

        // Get text part
        part = m_text.getTextPartByIndex(m_currentTextPartIndex);

        // Show text
        if (m_currentOnScreenTextObject != null)
        {
            GameObject.Destroy(m_currentOnScreenTextObject.gameObject);
            m_currentOnScreenTextObject = null;
        }
        m_currentOnScreenTextObject = m_window.createNewSpokenText("<size=18><color=silver><b>" + part.Character + ":</b></color></size> " + part.Text, Vector3.zero);

        // Create timer
        if (m_timer != null)
        {
            m_timer.Stop();
            m_timer = null;
        }
        m_timer = new Timer(part.DisplayTime * 1000);
        m_timer.Enabled = true;
        m_timer.AutoReset = false;
        m_timer.Elapsed += (object _s, ElapsedEventArgs _e) => { m_isHandleNextTextPart = true; };

        return false;
    }

    /**
     * Displays the choices
     */
    private bool onDisplayChoices()
    {
        // Local variables
        List<string> choiceTexts = new List<string>();
        List<int> choiceIDs = null;
        Choice choice = null;

        // Choices available?
        if (m_text.ChoiceCount <= 0)
            return false;

        // Create choice texts
        choiceIDs = m_text.getChoicesIDs();
        foreach (int id in choiceIDs)
        {
            // Get choice
            choice = m_text.getChoiceByID(id);

            // Enabled
            if (choice.IsEnabled == false)
                continue;

            // Add text
            choiceTexts.Add(m_text.getChoiceByID(id).Text);
        }
        if (choiceTexts.Count == 0)
            return false;

        // Register event callback
        DialogTextScript.AnswerClicked += onChoiceClicked;

        // Open window
        m_window.openDialogWindow();

        // Add choices to window
        m_window.addAnswers(choiceTexts.ToArray());

        return true;
    }

    /**
     * Will be called as soon as the text of a certain choice has been chosen
     */
    private void onChoiceClicked(UnityEngine.UI.Text _choiceText)
    {
        // Local variables
        Choice choice = null;
        List<int> choiceIDs = null;

        // Text set?
        if (m_text == null)
            return;

        // Compare answer with all choices
        choiceIDs = m_text.getChoicesIDs();
        foreach (int id in choiceIDs)
        {
            // Get choice and compare
            choice = m_text.getChoiceByID(id);
            if(choice.Text.Equals(_choiceText.text) == true)
            {
                // Notify choice
                choice.OnClick();

                // Exit conversation?
                if (choice.Type == Choice.ChoiceType.CHOICE_EXIT)
                    onConversationExit(choice.ExitValue);
                else
                    setTextByID(choice.NextTextID);
                return;
            }
        }
    }

    /*
     * Will be called if the conversion is about to exit
     */
    private void onConversationExit(string _exitCode)
    {
        // Clear
        m_conversation = null;
        m_text = null;

        // Notify transitions
        onNotifyDone(_exitCode);
    }
}
