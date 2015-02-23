/*
 * Project:	Billy's Payback
 * File:	PowerUpsOnDestroy.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Script which will be called as soon as a power-up has been destroyed or collected
 */
public class PowerUpsOnDestroy : MonoBehaviour
{
    // Audio clip which should be played on destroy
    public AudioClip m_audioClip = null;

    // True if the object has been activated
    private bool m_isActivated = false;

    // The audio source for playing sound effects
    private AudioSource m_audioSource = null;

    // The particle effect
    private ParticleSystem m_particle = null;

    // Override: MonoBehaviour::Awake()
    void Awake()
    {
        // Create audio source
        if (m_audioClip != null)
        {
            m_audioSource = gameObject.AddComponent<AudioSource>();
            m_audioSource.clip = m_audioClip;
        }

        // Try to get the particle effect
        m_particle = GetComponent<ParticleSystem>();
    }

    // Override: MonoBehaviour::FixedUpdate()
    void FixedUpdate()
    {
        // Activated?
        if(m_isActivated == false)
            return;

        // Audio source done?
        if (m_audioSource != null && m_audioSource.isPlaying == true)
            return;

        // Particle syste done?
        if (m_particle != null && m_particle.isPlaying == true)
            return;

        // Kill object
        m_isActivated = false;
        GameObject.Destroy(gameObject);
	}

    // Will be called as soon as the power-up has been destroyed or collected
    public void onAction()
    {
        // Play audio clip
        if (m_audioSource != null)
            m_audioSource.Play();

        // Start particle effect
        if (m_particle != null)
            m_particle.Play();

        // Set flag
        m_isActivated = true;
    }
}