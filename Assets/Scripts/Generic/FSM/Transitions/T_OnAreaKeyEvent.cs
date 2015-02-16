/*
 * Project:	Billy's Payback
 * File:	T_OnAreaKeyEvent.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * The transition will be triggered on an area-key-event
 */
public class T_OnAreaKeyEvent : FSMTransition
{
    // The area-key-event object
    public OnAreaKeyEvent m_areaKeyEvent;

    // Override: FSMTransition::OnEnable
    void OnEnable()
    {
        // Check area object
        if (m_areaKeyEvent == null)
        {
            Debug.LogWarning("Transition's area-key-event must not be null!");
            return;
        }

        // Add event
        m_areaKeyEvent.OnKeyEventPressed += () =>
        {
            // Set target game-state
            setTargetFSMState();
        };
    }

    // Override: FSMTransition::OnDisable
    void OnDisable()
    {
        // Remove event
        m_areaKeyEvent = null;
    }
}
