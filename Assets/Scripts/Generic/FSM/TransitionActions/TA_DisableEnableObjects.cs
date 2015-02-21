/*
 * Project:	Billy's Payback
 * File:	TA_DisableEnableObjects.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * The transition will enable/disable the specified objects on action
 */
public class TA_DisableEnableObjects : FSMAction
{
    // Represents an entry
    [System.Serializable]
    public class Entry
    {
        // The game object
        public GameObject m_gameObject = null;

        // True to enable the game object, otherwise false
        public bool m_enable = false;
    }

    // List with the specified game object
    public Entry[] m_entries;

    // Override: FSMAction::OnAction()
    override public void onAction()
    {
        // Loop through all entries
        foreach(Entry e in m_entries)
        {
            // Enable/disable
            if (e.m_gameObject != null)
                e.m_gameObject.SetActive(e.m_enable);
        }
    }
}
