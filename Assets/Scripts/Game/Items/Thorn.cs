/*
 * Project:	Billy's Payback
 * File:	Thorn.cs
 * Authors:	Raik Dankworth
 * Editors:	-
 */

using UnityEngine;
using System.Collections;

/**
 * Distribute hits 
 */
public class Thorn : Hitable
{
    /**
     * distibte hits to player if he stay in the trigger
     */
    void OnTriggerStay(Collider _other)
    {
        // the hit collider isnt the player?
        if (!_other.gameObject.CompareTag(Tags.TAG_PLAYER))
            return;

        // get the player script
        Hitable _hit = _other.gameObject.GetComponent<Hitable>();

        // havent a player script?
        if (_hit == null)
            return;
        _hit.onHit(this);
    }

    // ignore all other hits
    public override void onHit(Hitable _other)    { }
}
