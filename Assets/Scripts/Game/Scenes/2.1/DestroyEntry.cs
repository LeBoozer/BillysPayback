/*
 * Project:	Billy's Payback
 * File:	DestroyEntry.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Destroys the entrie of the cave
 */
public class DestroyEntry : MonoBehaviour, DeActivatable
{
    // True if the transition is activated
    public bool m_isActivated = true;

    // Delay in seconds for the first sound
    public float m_source0Delay = 0.0f;

    // Delay in seconds for the second sound
    public float m_source1Delay = 0.0f;

    // The first audio source
    private AudioSource m_source0 = null;

    // The second audio source
    private AudioSource m_source1 = null;

    // The particle system
    private ParticleSystem m_ps = null;

    // Override: MonoBehaviour::Start()
    void Start()
    {
        // Local variables
        AudioSource[] sources = null;

        // Get particle system
        m_ps = GetComponentInChildren<ParticleSystem>();
        if (m_ps == null)
        {
            Debug.LogError("The object is supposed to have a particle system!");
            return;
        }

        // Get audio sources
        sources = GetComponents<AudioSource>();
        if(sources == null || sources.Length != 2)
        {
            Debug.LogError("The object is supposed to have to two audio sources!");
            return;
        }
        m_source0 = sources[0];
        m_source1 = sources[1];

        // Disable all child object
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
    }

    // Override: DeActivatable::isActivated()
    public bool isActivated()
    {
        return m_isActivated;
    }

    // Override: DeActivatable::onActivate()
    public void onActivate()
    {
        // Set flag
        m_isActivated = true;

        // Enable all child object
        foreach (Transform child in transform)
            child.gameObject.SetActive(true);

        // Start particle system
        m_ps.Play();

        // Play sounds
        m_source0.PlayDelayed(m_source0Delay);
        m_source1.PlayDelayed(m_source1Delay);
    }

    // Override: DeActivatable::onDeactivate()
    public void onDeactivate()
    {
        // Clear flag
        m_isActivated = false;
    }
}