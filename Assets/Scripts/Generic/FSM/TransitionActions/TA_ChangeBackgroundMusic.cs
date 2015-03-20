/*
 * Project:	Billy's Payback
 * File:	TA_ChangeBackgroundMusic.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * The transition action will change the background music
 */
public class TA_ChangeBackgroundMusic : FSMAction
{
    // Class to store informations about an audio clip
    [System.Serializable]
    public class Source
    {
        public string   m_uniqueID = "";
        public bool     m_enabled = true;
        public bool     m_loop = false;
        public bool     m_removeAfterPlay = false;
        public float    m_targetVolume = 1.0f;
    }

    // The music manager instance
    public MusicManager m_musicManager = null;

    // The unique ID of the next active source
    public string m_nextActiveSource = "";

    // Randomly select an audio source
    public bool m_selectRandomly = false;

    // List of source updates
    public Source[] m_sources = null;

    // Override: FSMAction::OnAction()
    override public void onAction()
    {
        // Local variables
        MusicManager.Source src = null;

        // Music manager instance available?
        if (m_musicManager == null)
        {
            Debug.LogError("Music manager instance not set!");
            return;
        }

        // Update all sources
        foreach(var s in m_sources)
        {
            // Check
            if (s == null)
                continue;

            // Get source
            src = m_musicManager.getSourceByID(s.m_uniqueID);
            if (src == null)
                continue;

            // Update source
            src.m_enabled = s.m_enabled;
            src.m_loop = s.m_loop;
            src.m_removeAfterPlay = s.m_removeAfterPlay;
            src.m_targetVolume = s.m_targetVolume;
        }

        // Set next source
        if (m_nextActiveSource != null && m_nextActiveSource.Equals("") == false && m_selectRandomly == false)
            m_musicManager.setNextSourceByID(m_nextActiveSource);
        if (m_selectRandomly == true)
            m_musicManager.setNextSourceRandomly();
    }
}
