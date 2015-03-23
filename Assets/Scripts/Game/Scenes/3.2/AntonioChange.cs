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

    void OnTriggerEnter(Collider _other)
    {
        // ignore all other game objects
        if (_other.tag != Tags.TAG_PLAYER)
            return;

        // add and remove components
        GameObject _antonio = GameObject.FindGameObjectWithTag(Tags.TAG_COMPANION);
        if (_antonio == null)
        { 
            Debug.LogWarning("AntonioChange: Antonio dont found!");
            return;
        }
        _antonio.GetComponent<Antonio>().enabled = false;
        _antonio.AddComponent("BossAntonio");
        _antonio.tag = Tags.TAG_ENEMY;

        // set antonio to the boss fight area
        if(m_antonioBossFightArea != null)
            _antonio.transform.position = m_antonioBossFightArea.transform.position;

        // destroy the bridge
        if(m_brigde != null)
            Destroy(m_brigde);
    }

}
