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
    // True if valid
    private bool        m_isValid = false;

    // The update interval for checking if the music is still playing (in seconds)
    private float       m_updateCheckInterval = 0.1f;

    // Current playing audio clip (index)
    private int         m_currentAudioClip = 0;

    // The currently used volume
    private float       m_currentVolume = 0.0f;

    // True if fading in, otherwise false
    private bool        m_isFadeIn = true;

    // True if the music manager is enabled
    public bool         m_isEnabled = true;

    // The target volume
    public float        m_targetVolume = 1.0f;

    // The fading speed (in seconds)
    public float        m_fadeSpeed = 0.1f;

    // The fading value
    public float        m_fadeValue = 0.1f;

    // List with audio clips (will be played sequentially)
    public AudioClip[]  m_audioClips = null;

    // Override: MonoBehaviour::Awake()
	void Awake() 
    {
	    // Audio clips registered?
        if(m_audioClips == null || m_audioClips.Length == 0)
            return;

        // Check audio clips
        foreach(var clip in m_audioClips)
        {
            // Empty or 3D sound?
            if(clip == null)
            {
                Debug.LogError("Invalid audio clip in music manager");
                return;
            }
        }

        // Set volume
        audio.volume = m_currentVolume;

        // Set flag
        m_isValid = true;

        // Start first audio clip
        if (m_isEnabled == true)
            StartCoroutine("proc_fade");
	}

    // Override: MonoBehaviour::FixedUpdate()
	void FixedUpdate () 
    {
	    // Valid and enabled?
        if (m_isValid == false || m_isEnabled == false)
            return;

        // Still playing?
        if (audio.isPlaying == true)
            return;

        // Change audio clip
        audio.clip = m_audioClips[(m_currentAudioClip++) % m_audioClips.Length];
        audio.Play();
	}

    // Fades the audio clip in/out
    // Return: True to terminate coroutine
    private bool fade()
    {
        // Local variables
        bool done = false;

        // Fading in?
        if (m_isFadeIn == true)
        {
            // Increase volume
            m_currentVolume += m_fadeValue;
            if (m_currentVolume > m_targetVolume)
            {
                m_currentVolume = m_targetVolume;
                done = true;
            }
        }
        else
        {
            // Decrease volume
            m_currentVolume -= m_fadeValue;
            if (m_currentVolume < 0.0f)
            {
                m_currentVolume = 0.0f;
                done = true;
            }
        }

        // Set volume
        audio.volume = m_currentVolume;

        // Done?
        return done;
    }

    // Plays the audio clip
    private void play()
    {
        // Still playing?
        if (audio.isPlaying == true)
            return;

        // Change audio clip
        audio.clip = m_audioClips[(++m_currentAudioClip) % m_audioClips.Length];
        audio.Play();
    }

    // Coroutine for the music fading
    private IEnumerator proc_fade()
    {
        // Loop endless
        while (true)
        {
            if (fade() == true)
            {
                StartCoroutine("proc_play");
                yield break;
            }
            else
                yield return new WaitForSeconds(m_fadeSpeed);
        }
    }

    // Coroutine for the music playing
    private IEnumerator proc_play()
    {
        // Loop endless
        while (true)
        {
            play();
            yield return new WaitForSeconds(m_updateCheckInterval);
        }
    }
}
