/*
 * Project:	Billy's Payback
 * File:	MusicManager.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Manager for the background music in the game
 */
[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour 
{
    private static readonly float PLAY_UPDATE_INTERVAL_SEC = 0.1f;
    private static readonly float WAIT_UPDATE_INTERVAL_SEC = 0.1f;
    private static readonly float DEFAULT_FADE_INTERVAL_SEC = 0.1f;

    // All possible result values for the playing managment
    private enum Result
    {
        eWAIT,
        ePLAY,
        eFADE
    }

    // Class to store informations about the fading for an audio source
    [System.Serializable]
    public class SourceFade
    {
        public bool     m_enabled = false;
        public float    m_fadeSpeedSec = 0.1f;
        public float    m_fadeValue = 0.5f;
    }

    // Class to store informations about an audio clip
	[System.Serializable]
	public class Source
	{
        public string       m_uniqueID = "";
        public bool         m_enabled = true;
        public bool         m_loop = false;
        public bool         m_removeAfterPlay = false;
        public bool         m_restorePosition = false;
        public float        m_targetVolume = 1.0f;
        public AudioClip    m_clip = null;
        public SourceFade   m_fadeIn = new SourceFade();
        public SourceFade   m_fadeOut = new SourceFade();
        [HideInInspector]
        public float        m_floatPosSec = 0.0f;
    }

    // True if the music manager is enabled
    public bool                         m_isEnabled = true;

    // True to select a random start source
    public bool                         m_random = true;

    // List with all registered audio clips (background music)
    public Source[]                     m_backgroundClips = null;

    // List of the background sources
    private Dictionary<string, Source>  m_sources = new Dictionary<string,Source>();

    // Queue of all source IDs (queueing is not supported bei the diectionary!)
    private Queue<string>               m_idQueue = new Queue<string>();

    // The currently active source
    private Source                      m_currentSource = null;

    // The next active source
    private Source                      m_nextSource = null;

    // The current volume
    private float                       m_currentVolume = 0.0f;

    // True if this is the first fade-in run
    private bool                        m_isFirstRunFadeIn = true;

    // Override: MonoBehaviour::Awake()
    void Awake()
    {
        // Local variables
        string[] ids = null;
        int index = 0;
        Source source = null;

        // Disabled?
        if (m_isEnabled == false)
            return;

        // Loop through all registered sources
        foreach(var src in m_backgroundClips)
        {
            // Validate source
            if(src == null || src.m_clip == null || src.m_uniqueID.Equals("") == true)
            {
                Debug.LogError("Music manager: Invalid background clips registered!");
                return;
            }

            // Add to list
            m_sources.Add(src.m_uniqueID, src);
            m_idQueue.Enqueue(src.m_uniqueID);
        }

        // Select random start source?
        if (m_random == true)
        {
            // To array
            ids = m_idQueue.ToArray();
            
            // Try to find suitable source
            while(true)
            {
                // Get source
                index = Random.Range(0, ids.Length);
                source = getSourceByID(ids[index]);
                if (source == null || source.m_enabled == false)
                    continue;

                // Source found, set as next source
                m_nextSource = source;
                break;
            }
        }

        // Kick off the playing
        StartCoroutine("proc_play");
	}

    // Returns a source by its ID
    public Source getSourceByID(string _id)
    {
        // Local variables
        Source res = null;

        // Check parameter
        if (_id == null || _id.Equals("") == true)
            return null;

        // Try to find
        m_sources.TryGetValue(_id, out res);

        return res;
    }

    // Returns the next available audio source
    private Source getNextAudioSource()
    {
        // Local variables
        string id = "";
        Source src = null;

        // Check registered audio sources
        for (int i = 0; i < m_idQueue.Count; ++i)
        {
            // Get source's ID
            id = m_idQueue.Dequeue();
            if (id == null || id.Equals("") == true)
                continue;

            // Get source
            if (m_sources.TryGetValue(id, out src) == false)
                continue;

            // Remove?
            if (src.m_removeAfterPlay == false)
                m_idQueue.Enqueue(id);

            // Active?
            if (src.m_enabled == false)
                continue;

            return src;
        }

        return null;
    }

    // Sets the next audio source forcefully
    public void setNextSourceByID(string _id)
    {
        // Local variables
        Source src = null;

        // Check parameter
        if (_id == null || _id.Equals("") == true)
            return;

        // Try to get source
        if (m_sources.TryGetValue(_id, out src) == false)
            return;

        // Set 
        m_nextSource = src;
    }

    // Sets the next audio source randomly but forcefully
    public void setNextSourceRandomly()
    {
        // Get next source
        m_nextSource = getNextAudioSource();
    }

    // Manages the play of the background music
    private Result play()
    {
        // Current/next source null?
        if (m_currentSource == null && m_nextSource == null)
        {
            // Try to get the next available source
            m_nextSource = getNextAudioSource();
            if (m_nextSource == null)
                return Result.eWAIT;

            // Fade-in not required?
            if (m_nextSource.m_fadeIn == null || m_nextSource.m_fadeIn.m_enabled == false)
            {
                // Set as current and start
                m_currentSource = m_nextSource;
                m_nextSource = null;
                audio.volume = m_currentSource.m_targetVolume;
                audio.clip = m_currentSource.m_clip;
                if (m_currentSource.m_restorePosition == true)
                    audio.time = m_currentSource.m_floatPosSec;
                audio.Play();
                return Result.ePLAY;
            }

            return Result.eFADE;
        }

        // Next source set?
        else if (m_nextSource != null)
        {
            // Save current position
            if (m_currentSource != null && m_currentSource.m_restorePosition == true)
                m_currentSource.m_floatPosSec = audio.time;

            // Fade-out required?
            if (m_currentSource != null && m_currentSource.m_fadeOut != null && m_currentSource.m_fadeOut.m_enabled == true)
                return Result.eFADE;

            // Fade-in not required?
            if (m_nextSource.m_fadeIn != null && m_nextSource.m_fadeIn.m_enabled == true)
                return Result.eFADE;

            // Set next song as current
            m_currentSource = m_nextSource;
            m_nextSource = null;
            audio.volume = m_currentSource.m_targetVolume;
            audio.clip = m_currentSource.m_clip;
            if (m_currentSource.m_restorePosition == true)
                audio.time = m_currentSource.m_floatPosSec;
            audio.Play();
            return Result.ePLAY;
        }

        // Still playing?
        if(audio.isPlaying == false)
        {
            // Reset position
            m_currentSource.m_floatPosSec = 0.0f;

            // Loop?
            if (m_currentSource.m_loop == true)
            {
                // Restart
                audio.Play();
            }
            else
            {
                // Kill current
                m_currentSource = null;
            }
        }

        return Result.ePLAY;
    }

    // Manages the fading process
    // Return: Time for the next check interval. < 0.0f if fading is done
    private float fade()
    {
        // Local variables
        float nextCheckSec = DEFAULT_FADE_INTERVAL_SEC;

        // Get current volume
        m_currentVolume = audio.volume;

        // Fade-out required?
        if (m_currentSource != null && m_currentSource.m_fadeOut != null && m_currentSource.m_fadeOut.m_enabled == true)
        {
            // Decrease volume
            m_currentVolume -= m_currentSource.m_fadeOut.m_fadeValue;
            if (m_currentVolume <= 0.0f)
            {
                // Fade-out is done!
                m_currentVolume = 0.0f;
                m_currentSource = null;

                // Start next audio source
                audio.clip = m_nextSource.m_clip;
                if (m_nextSource.m_restorePosition == true)
                    audio.time = m_nextSource.m_floatPosSec;
                audio.Play();
            }
            else
                nextCheckSec = m_currentSource.m_fadeOut.m_fadeSpeedSec;
        }

        // Fade-in required?
        else if(m_nextSource != null)
        {
            // First run?
            if (m_isFirstRunFadeIn == true)
            {
                m_currentVolume = 0.0f;
                m_isFirstRunFadeIn = false;
            }

            // Increase volume
            m_currentVolume += m_nextSource.m_fadeIn.m_fadeValue;
            if (m_currentVolume >= m_nextSource.m_targetVolume)
            {
                // Fade-in is done!
                m_currentVolume = m_nextSource.m_targetVolume;
                m_currentSource = m_nextSource;
                m_nextSource = null;
                nextCheckSec = -1.0f;

                // Start next audio source
                if (audio.isPlaying == false)
                {
                    audio.clip = m_currentSource.m_clip;
                    if (m_currentSource.m_restorePosition == true)
                        audio.time = m_currentSource.m_floatPosSec;
                    audio.Play();
                }
            }
            else
                nextCheckSec = m_nextSource.m_fadeIn.m_fadeSpeedSec;
        }

        // Set volume
        audio.volume = m_currentVolume;

        return nextCheckSec;
    }

    // Coroutine for the music playing
    private IEnumerator proc_play()
    {
        // Local variables
        Result r = Result.ePLAY;

        // Loop endless
        while (true)
        {
            // Play
            r = play();
            if(r == Result.eFADE)
            {
                // Start fade process
                m_isFirstRunFadeIn = true;
                StartCoroutine("proc_fade");

                // Terminate itself
                yield break;
            }
            else
                yield return new WaitForSeconds(r == Result.ePLAY ? PLAY_UPDATE_INTERVAL_SEC : WAIT_UPDATE_INTERVAL_SEC);
        }
    }

    // Coroutine for the fading
    private IEnumerator proc_fade()
    {
        // Local variables
        float r = 0.0f;

        // Loop endless
        while (true)
        {
            // Fade
            r = fade();
            if (r < 0.0f)
            {
                // Start play process
                StartCoroutine("proc_play");

                // Terminate itself
                yield break;
            }
            else
                yield return new WaitForSeconds(r);
        }
    }
}
