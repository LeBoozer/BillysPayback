/*
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

    // The script handling the click-event on choices
    private DialogTextScript     m_eventScript = null;

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

        // Get start text, text-part
        m_text = m_conversation.getTextByID(m_conversation.StartTextID);
        if(m_text == null)
        {
            Debug.LogError("Invalid text FSM-state (" + gameObject.name + "). Specified text has not been found!");
            return;
        }
    }

    // Override: MonoBehaviour::Start()
    protected override void Start()
    {
        // Call parent
        base.Start();

        // Get instance of the dialog window
        m_window = GameObject.Find("Dialog").GetComponent<DialogueWindowScript>();
        if (m_window == null)
        {
            Debug.LogError("Dialog window has not been found!");
            return;
        }

        // Get event script of the dialogue
        m_eventScript = m_window.GetComponentInChildren<DialogTextScript>();
        if (m_eventScript == null)
        {
            Debug.LogError("Event script for the dialogue has not been found!");
            return;
        }

        // Open window
        m_window.openDialogWindow();

        // Start with the first text-part
        onNextTextPart();
    }

    // Override: MonoBehaviour::Update()
    void Update()
    {
        // Next text-part?
        if(m_isHandleNextTextPart == true)
        {
            m_isHandleNextTextPart = false;
            onNextTextPart();
        }
    }

    // Override: FSMState::OnEnable()
    void OnEnable()
    {
    }

    // Override: FSMState::OnDisable()
    void OnDisable()
    {
        // Kill timer
        if (m_timer != null)
        {
            m_timer.Stop();
            m_timer = null;
        }

        // Hide window
        if (m_window != null)
            m_window.closeDialogWindow();
    }

    /**
     * Handles the timing and displaying of the text-parts
     */
    private void onNextTextPart()
    {
        // Local variables
        TextPart part = null;

        // Increase index
        ++m_currentTextPartIndex;

        // Reached?
        if (m_currentTextPartIndex >= m_text.TextPartCount)
        {
            // Display choices
            if(!onDisplayChoices())
            {
                // No choices available!
                onConversationExit(AdvancedDialogue.DIALOGUE_NO_CHOICE_EXIT_VALUE);
            }
            return;
        }

        // Get text part
        part = m_text.getTextPartByIndex(m_currentTextPartIndex);

        // Show text
        if (m_currentOnScreenTextObject != null)
        {
            GameObject.Destroy(m_currentOnScreenTextObject.gameObject);
            m_currentOnScreenTextObject = null;
        }
        m_currentOnScreenTextObject = m_window.createNewSpokenText(part.Text, Vector3.zero);

        // Create timer
        if (m_timer != null)
        {
            m_timer.Stop();
            m_timer = null;
        }
        m_timer = new Timer(part.DisplayTime * 1000);
        m_timer.Enabled = true;
        m_timer.Elapsed += (object _s, ElapsedEventArgs _e) => { m_isHandleNextTextPart = true; };
    }

    /**
     * Displays the choices
     */
    private bool onDisplayChoices()
    {
        // Local variables
        List<string> choiceTexts = new List<string>();
        List<int> choiceIDs = null;

        // Choices available?
        if (m_text.ChoiceCount <= 0)
            return false;

        // Create choice texts
        choiceIDs = m_text.getChoicesIDs();
        foreach (int id in choiceIDs)
        {
            // Add text
            choiceTexts.Add(m_text.getChoiceByID(id).Text);
        }

        // Register event callback
        DialogTextScript.AnswerClicked += onChoiceClicked;

        // Add choices to window
        m_window.addAnswers(choiceTexts.ToArray());

        return true;
    }

    /**
     * Will be called as soon as the text of a certain choice has been chosen
     */
    private void onChoiceClicked(UnityEngine.UI.Text _choiceText)
    {
    }

    /*
     * Will be called if the conversion is about to exit
     */
    private void onConversationExit(string _exitCode)
    {
        // Notify transitions
        onNotifyDone(_exitCode);
    }
}
