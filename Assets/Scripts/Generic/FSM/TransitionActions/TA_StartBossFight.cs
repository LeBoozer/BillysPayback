/*
 * Project:	Billy's Payback
 * File:	TA_StartBossFight.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Transition action which will activate or deactivate the specified game objects
 */
public class TA_StartBossFight : FSMAction
{
    // The target boss
    public GameObject m_bossObject;

    // The boss interface
    private Boss m_boss = null;

    // Override: FSMAction::OnAction()
    override public void onAction()
    {
        // Local variables
        Component[] comps = null;

        // Validate object
        if (m_bossObject == null)
        {
            Debug.LogError("No boss object has been defined!");
            return;
        }

        // Extract components
        comps = m_bossObject.GetComponents(typeof(Boss));
        if (comps == null || comps.Length == 0)
        {
            Debug.LogError("Only game object with script inherting from 'Boss' can be used!");
            return;
        }

        // Loop through all components
        foreach (Component comp in comps)
        {
            // Requirements present?
            if (comp is Boss == true)
            {
                // Set
                m_boss = comp as Boss;
                break;
            }
        }
        if (m_boss == null)
        {
            Debug.LogError("No valid boss object has been defined!");
            return;
        }

        // Start fight
        m_boss.StartBossFight();
    }
}
