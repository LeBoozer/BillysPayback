/*
 * Project:	Billy's Payback
 * File:	TA_SetPlayerPowerups.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Transition action which will set the availability of the defined power-ups 
 */
public class TA_SetPlayerPowerups : FSMAction
{
    // Represents an entry
    [System.Serializable]
    public class Entry
    {
        // The game object
        public PlayerData.PowerUpType m_type;

        // True to enable the power-up, otherwise false
        public bool m_available = false;
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
            Game.Instance.PlayerData.setPowerUpAvailable(e.m_type, e.m_available);
    }
}
