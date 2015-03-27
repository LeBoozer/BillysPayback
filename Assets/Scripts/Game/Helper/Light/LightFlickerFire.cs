/*
 * Project:	Billy's Payback
 * File:	LightFlickerFire.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Helper script to simulate light flicker for fires (e.g. camp fire, torches ...)
 */
[RequireComponent(typeof(Light))]
public class LightFlickerFire : MonoBehaviour
{
    // The min minimum light intensity
    public float m_minMinIntensity = 0.6f;

    // The max minimum light intensity
    public float m_maxMinIntensity = 0.7f;

    // The min maximum light intensity
    public float m_minMaxIntensity = 0.9f;

    // The max maximum light intensity
    public float m_maxMaxIntensity = 1.0f;

    // True to randomize the lower and upper bound of the light intensity every frame
    public bool  m_randomizePerFrame = true;

    // The calculated min intensity
    private float m_minIntensity = 0.0f;

    // The calculated max intensity
    private float m_maxIntensity = 0.0f;

    // The flicker speed
    public float m_flickerSpeed = 0.1f;

    // Override: MonoBehaviour::Awake()
    void Awake()
    {
        // Randomize per frame?
        if (m_randomizePerFrame == false)
        {
            // Get min/max intensities
            m_minIntensity = Random.Range(m_minMinIntensity, m_maxMinIntensity);
            m_maxIntensity = Random.Range(m_minMaxIntensity, m_maxMaxIntensity);
        }

        // Start flicker effect
        StartCoroutine("flicker");
    }

    // Coroutine for simulating a simple flicker algorithm
    IEnumerator flicker()
    {
        // Loop endless
        while (true)
        {
            // Randomize per frame?
            if (m_randomizePerFrame == true)
            {
                // Get min/max intensities
                m_minIntensity = Random.Range(m_minMinIntensity, m_maxMinIntensity);
                m_maxIntensity = Random.Range(m_minMaxIntensity, m_maxMaxIntensity);
            }

            // Calculatue intensity
            GetComponent<Light>().intensity = Random.Range(m_minIntensity, m_maxIntensity);

            // Wait
            yield return new WaitForSeconds(m_flickerSpeed);
        }
    }
}
