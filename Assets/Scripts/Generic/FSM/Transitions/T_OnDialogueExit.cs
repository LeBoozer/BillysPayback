/*
 * Project:	Billy's Payback
 * File:	T_OnDialogueExit.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * The transition will be triggered on certain host-done events
 */
public class T_OnDialogueExit : FSMTransition
{
    // Array with accepted dialogue exit values
    public string[] m_dialogueExitValues = null;

    // True to respond to the event: dialogue has been cancelled by the user
    public bool m_considerDialogueCancellation = false;

    // True to repsond to the event: dialogue has no choices for the user
    public bool m_considerNoChoiceExits = false;

    // Override: FSMTransition::onHostStateDone
    public override void onHostStateDone(object _param)
    {
        // Check parameter
        if (_param is string == false)
            return;

        // Cancellation event?
        if (m_considerDialogueCancellation == true && _param.Equals(AdvancedDialogue.DIALOGUE_CANCELLED_EXIT_VALUE) == true ||
            m_considerNoChoiceExits == true && _param.Equals(AdvancedDialogue.DIALOGUE_NO_CHOICE_EXIT_VALUE) == true)
        {
            // Change to target state
            setTargetFSMState();
            return;
        }

        // Exit values defined?
        if (m_dialogueExitValues == null)
            return;

        // Right descision?
        foreach (string s in m_dialogueExitValues)
        {
            // Compare
            if (s.Equals(_param) == true)
            {
                // Change to target state
                setTargetFSMState();
                break;
            }
        }
    }
}
