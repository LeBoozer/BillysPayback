/*
 * Project:	Billy's Payback
 * File:	AreaMoodBackgroundMusic.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Changes to background music to the specified mood music if the reference has entered a certain area
 */
public class AreaMoodBackgroundMusic : MonoBehaviour 
{
    // The reference
    public GameObject   m_reference = null;

    // The music manager instance
    public MusicManager m_musicManager = null;

    // The unique source ID of the audio source
    public string       m_uniqueSourceID = "";

    // The collider list
    public Collider[]   m_collider = null;

    // Number of areas the player has entered
    private int         m_inAreaCount = 0;

    // Override: MonoBehaviour::Start
	void Start() 
    {
        // Local variables
        FSMEventHighjack hj = null;

	    // Validate parameter
        if(m_reference == null)
        {
            Debug.LogError("No reference set!");
            return;
        }
        if (m_musicManager == null)
        {
            Debug.LogError("Music manager instance not set!");
            return;
        }

        // Validate collider
        foreach(Collider c in m_collider)
        {
            // Validate
            if(c == null || c.isTrigger == false)
            {
                Debug.LogError("Invalid collider registered (trigger colliders are needed)!");
                return;
            }
        }

        // Highjack collider
        foreach (Collider c in m_collider)
        {
            // Inject script
			hj = c.gameObject.AddComponent<FSMEventHighjack>();
            hj.FSMOnTriggerEnter += (Collider _other) =>
            {
                onTriggerAction(_other, true);
            };
            hj.FSMOnTriggerLeave += (Collider _other) =>
            {
                onTriggerAction(_other, false);
            };
        }
	}

    // Override: MonoBehaviour::OnDestroy
    void OnDestroy()
    {
        // Local variables
        FSMEventHighjack hj = null;

        // Delete highjack from trigger objects
        foreach (Collider c in m_collider)
        {
            // Check
            if (c == null)
                continue;

            // Remove
            hj = c.gameObject.GetComponent<FSMEventHighjack>();
            if (hj != null)
                Component.Destroy(hj);
        }
    }
	
	// Handles trigger events
    private void onTriggerAction(Collider _other, bool _entered)
    {
        // Local variables
        int oldCount = m_inAreaCount;

        // Reference triggered event?
        if (_other.name.Equals(m_reference.name) == false)
            return;

        // Adjust count
        m_inAreaCount += (_entered == true ? 1 : -1);
        if (oldCount == 0 && m_inAreaCount > 0 && m_uniqueSourceID != null && m_uniqueSourceID.Equals("") == false)
        {
            // Entered, change sound
            m_musicManager.setNextSourceByID(m_uniqueSourceID);
        }
        if(oldCount > 0 && m_inAreaCount == 0)
        {
            // Left, change sound to normal background music
            m_musicManager.setNextSourceRandomly();
        }
    }
}
