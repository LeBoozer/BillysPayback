/*
 * Project:	Billy's Payback
 * File:	T_OnBossBreak.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * The transition will be triggered on boss break events
 */
public class T_OnBossBreak : FSMTransition
{
    // The target bosses
    public GameObject[] m_bossObjects;

    // The boss interface list
    private List<Boss>  m_bosses = new List<Boss>();

    // Override: FSMTransition::OnEnable
    void OnEnable()
    {
        // Local variables
        Component[] comps = null;

        // Start has been called?
        if (wasStartCalled() == false)
            return;

        // Reset
        m_bosses.Clear();

        // Loop through all registered bosses
        foreach (GameObject obj in m_bossObjects)
        {
            // Validate object
            if (obj == null)
            {
                Debug.LogError("Invalid boss object has been defined!");
                return;
            }

            // Extract components
            comps = obj.GetComponents(typeof(Boss));
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
                    // Register event
                    (comp as Boss).OnBreakBossFight(() =>
                    {
                        setTargetFSMState();
                    });

                    // Add to list
                    m_bosses.Add(comp as Boss);

                    // Set
                    break;
                }
            }
        }
    }

    // Override: FSMTransition::OnDisable
    void OnDisable()
    {
    }
}
