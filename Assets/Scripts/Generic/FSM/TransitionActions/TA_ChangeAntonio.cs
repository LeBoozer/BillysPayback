/*
 * Project:	Billy's Payback
 * File:	TA_ChangeAntonio.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * The transition action will set attributes to antonio
 */
public class TA_ChangeAntonio : FSMAction
{
    // True to auto-find the game object of antonio
    public bool m_autoFindAntonio = true;

    // The game object of antonio
    public GameObject m_antonio = null;

    // True to follow the player
    public bool m_followPlayer = true;

    // Override: FSMAction::OnAction()
    override public void onAction()
    {
        // Local variables
        Antonio ant = null;

        // Auto-find?
        if(m_antonio == null && m_autoFindAntonio == true)
            m_antonio = GameObject.FindGameObjectWithTag(Tags.TAG_COMPANION);
        if(m_antonio == null)
        {
            Debug.LogError("Game-object of antonio is missing!");
            return;
        }

        // Get script
        ant = m_antonio.GetComponent<Antonio>();
        if(ant == null)
        {
            ant = m_antonio.GetComponentInChildren<Antonio>();
            if (ant == null)
            {
                Debug.LogError("Game-object of antonio is invalid!");
                return;
            }
        }

        // Set attributes
        ant.m_chase = m_followPlayer;
        ant.m_targetDistance = float.MaxValue;
	}
}
