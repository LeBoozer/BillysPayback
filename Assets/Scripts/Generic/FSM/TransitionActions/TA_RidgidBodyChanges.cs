/*
 * Project:	Billy's Payback
 * File:	TA_RidgidBodyChanges.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Transition action which will set attributes to defined ridgid bodies
 */
public class TA_RidgidBodyChanges : FSMAction
{
    // Represents an entry
    [System.Serializable]
    public class Entry
    {
        // The ridgid body
        public Rigidbody m_body;

        // True to set the body to kinematic
        public bool m_isKinematic = false;
    }

    // List with the specified game object
    public Entry[] m_entries;

    // Override: FSMAction::OnAction()
    override public void onAction()
    {
        // Check parameter
        if (m_entries == null || m_entries.Length == 0)
            return;

        // Loop through all entries
        foreach (Entry e in m_entries)
        {
            // Check
            if (e.m_body == null)
                continue;

            // Set attributes
            e.m_body.isKinematic = e.m_isKinematic;
           
            // Wake-up
            e.m_body.WakeUp();
        }
    }
}
