/*
 * Project:	Billy's Payback
 * File:	Portal.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Manages a two-entry portal crated from area-key-events
 */
public class Portal : MonoBehaviour
{
    // The player game object
    private GameObject      m_player;

    // The first entry
    private OnAreaKeyEvent  m_entry0;

    // The second entry
    private OnAreaKeyEvent  m_entry1;

    // Will be called as soon as one key-event is being triggered
    private void onEnter(OnAreaKeyEvent _src, OnAreaKeyEvent _dst)
    {
        // Check parameter
        if (_dst == null)
            return;

        // Move player to destination
        m_player.gameObject.transform.position = _dst.gameObject.transform.position;
    }

    // Override: MonoBehaviour::Awake
	void Awake()
    {
        // Local variables
        OnAreaKeyEvent[] entries = null;

        // Get player
        m_player = GameObject.FindWithTag(Tags.TAG_PLAYER);
        if(m_player == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

	    // Get entries
        entries = gameObject.GetComponentsInChildren<OnAreaKeyEvent>();
        if(entries == null || entries.Length != 2)
        {
            Debug.LogError("Portals must contain 2 entries!");
            return;
        }

        // Copy objects
        m_entry0 = entries[0];
        m_entry1 = entries[1];

        // Add event: first entry
        m_entry0.OnKeyEventTriggered += () =>
        {
            onEnter(m_entry0, m_entry1);
        };

        // Add event: second entry
        m_entry1.OnKeyEventTriggered += () =>
        {
            onEnter(m_entry1, m_entry0);
        };
	}
}
