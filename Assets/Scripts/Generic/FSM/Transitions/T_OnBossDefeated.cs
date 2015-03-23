/*
 * Project:	Billy's Payback
 * File:	T_OnBossDefeated.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * The transition will be triggered on boss defeated events
 */
public class T_OnBossDefeated : FSMTransition
{
    // The target boss
    public GameObject m_bossObject;

    // The boss interface
    private Boss m_boss = null;

    // Override: FSMTransition::OnEnable
    void OnEnable()
    {
        // Local variables
        Component[] comps = null;

        // Start has been called?
        if (wasStartCalled() == false)
            return;

        // Validate object
        if(m_bossObject == null)
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
        if(m_boss == null)
        {
            Debug.LogError("No valid boss object has been defined!");
            return;
        }

        // Register event
        m_boss.EndBossFight(() => { setTargetFSMState(); });
    }

    // Override: FSMTransition::OnDisable
    void OnDisable()
    {
    }
}
