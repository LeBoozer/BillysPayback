/*
 * Project:	Billy's Payback
 * File:	SA_DeActivate.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * State action which will activate or deactivate the specified game objects
 */
public class SA_DeActivate : FSMStateAction 
{
    // Represents an entry
    [System.Serializable]
    public class Entry
    {
        // The game object
        public GameObject m_gameObject = null;

        // True to enable the game object, otherwise false
        public bool m_enable = false;

        // True to trigger the activation on enter events, otherwise on leave events
        public bool m_onEnter = true;
    }

    // Represents an internal entry
    private class InternalEntry
    {
        // The object
        public DeActivatable m_object = null;

        // True to activate the object, otherwise false
        public bool m_enable = false;

        // True to trigger the activation on enter events, otherwise on leave events
        public bool m_onEnter = true;
    }

    // List with the specified game object
    public Entry[] m_entries;

    // List with the internal entries
    private List<InternalEntry> m_internalEntries = null;

    // Builds the internal entry list
    private void generateEntries()
    {
        // Local variables
        InternalEntry ent = null;
        Component[] comps = null;
        bool found = false;

        // Create list
        m_internalEntries = new List<InternalEntry>();

        // Extract internal entries
        foreach (Entry e in m_entries)
        {
            // Clear flag
            found = false;

            // Validate
            if (e.m_gameObject == null)
                continue;

            // Extract components
            comps = e.m_gameObject.GetComponents(typeof(DeActivatable));
            if (comps == null || comps.Length == 0)
            {
                Debug.LogError("Only game object with script inherting from 'DeActivatable' can be (de)-activated!");
                continue;
            }

            // Loop through all components
            foreach (Component comp in comps)
            {
                // Requirements present?
                if (comp is DeActivatable == true)
                {
                    // Generate internal entry
                    ent = new InternalEntry();
                    ent.m_enable = e.m_enable;
                    ent.m_onEnter = e.m_onEnter;
                    ent.m_object = comp as DeActivatable;
                    m_internalEntries.Add(ent);

                    // Set flag
                    found = true;
                }
            }

            // Requirements found?
            if (found == false)
                Debug.LogError("Only game object with script inherting from 'DeActivatable' can be (de)-activated!");
        }
    }

    // Will activate/deactivate the specified object
    // _enter: True on action enter events
    private void action(bool _enter)
    {
        // Generate internal entries
        if (m_internalEntries == null)
            generateEntries();

        // Check parameter
        if (m_internalEntries == null || m_internalEntries.Count == 0)
            return;

        // Loop through all entries
        foreach (InternalEntry e in m_internalEntries)
        {
            if (e.m_object == null || e.m_onEnter != _enter)
                continue;
            if (e.m_enable == true)
                e.m_object.onActivate();
            else
                e.m_object.onDeactivate();
        }
    }

    // Override: FSMStateAction::onActionEnter()
    override public void onActionEnter()
    {
        action(true);
    }

    // Override: FSMStateAction::onActionLeave()
    override public void onActionLeave()
    {
        action(false);
    }
}
