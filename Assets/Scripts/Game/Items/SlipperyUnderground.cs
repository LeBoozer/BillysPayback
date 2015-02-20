/*
 * Project:	Billy's Payback
 * File:	SlipperyUnderground.cs
 * Authors:	Raik Dankworth
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/**
 * Let the player slip over a model
 */
public class SlipperyUnderground : MonoBehaviour {

    // Variables
    public bool         m_blockJumping = false;
    public float        m_velocityFactor = 1f;
    public Vector2      m_slipDirection = Vector2.zero;


    /**
     * impede the player if he enter the trigger
     */
    void OnTriggerEnter(Collider _other)
    {
        // the hit collider isnt the player?
        if (!_other.gameObject.CompareTag(Tags.TAG_PLAYER))
            return;

        // get the player script
        Player _p = _other.gameObject.GetComponent<Player>();

        // havent a player script?
        if (_p == null)
            return;

        // set impediments
        _p.setImpedimentJumping(m_blockJumping);
        _p.setImpedimentVelocity(m_velocityFactor);
        _p.setImpedimentSlip(m_slipDirection);
        Debug.Log("Enter");
    }

    /**
     * remove the impediment for the player if he exit the trigger
     */
    void OnTriggerExit(Collider _other)
    {
        // the hit collider isnt the player?
        if (!_other.gameObject.CompareTag(Tags.TAG_PLAYER)) 
            return;

        // get the player script
        Player _p = _other.gameObject.GetComponent<Player>();

        // havent a player script?
        if (_p == null)
            return;

        // remove impediments
        _p.setImpedimentJumping(false);
        _p.setImpedimentVelocity(1f);
        _p.setImpedimentSlip(Vector2.zero);

        Debug.Log("Exit");
    }

}
