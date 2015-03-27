/*
 * Project:	Billy's Payback
 * File:	AntonioChange.cs
 * Authors:	Raik Dankworth
 */
using UnityEngine;
using System.Collections;

/**
 * Special Trigger for Level 3.2
 * Set Antonio to boss fight area
 * Remove and Add important components
 */
public class AntonioChange : MonoBehaviour
{
    public GameObject m_brigde;
    public GameObject m_antonioBossFightArea;
    public GameObject m_antonio;

    void OnTriggerEnter(Collider _other)
    {
        // ignore all other game objects
        if (_other.tag != Tags.TAG_PLAYER)
            return;

        // antonio not set?
        if (m_antonio == null)
        {
            Debug.Log("AntonioChange: Antinio dont set!");
            return;
        }

        // enabled the antonio script
        m_antonio.GetComponent<Antonio>().enabled = false;

        // add the BossAntonio script if necessary
        if(m_antonio.GetComponent<BossAntonio>() == null)
            m_antonio.AddComponent<BossAntonio>();

        // set the new tag of antonio
        m_antonio.tag = Tags.TAG_ENEMY;

        // set antonio to the boss fight area
        if(m_antonioBossFightArea != null)
            m_antonio.transform.position = m_antonioBossFightArea.transform.position;

        // destroy the bridge
        if (m_brigde != null)
        {
            Destroy(m_brigde);
            m_brigde = null;
        }
    }

}
