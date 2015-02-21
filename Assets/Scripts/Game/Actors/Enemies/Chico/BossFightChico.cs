/*
 * Project:	Billy's Payback
 * File:	BossFightChico.cs
 * Authors:	Raik Dankworth
 * Editors:	-
 */
using UnityEngine;
using System;
using System.Collections;

/**
 * Let the bossfight again chico start if necessary
 * Open the door to BlackSparrow 
 */
public class BossFightChico : MonoBehaviour 
{
    void OnTriggerEnter(Collider _other)
    {
        // ignore all other except the player
        if (_other.transform.tag != Tags.TAG_PLAYER)
            return;

        // 
        GameObject chico = GameObject.Find("Chico");
        GameObject doorToBS = GameObject.Find("BSDoor");

        Action trigger = () =>
        {
            Destroy(doorToBS);
            Destroy(chico);
        };

        // 
        //if(Game.Instance.ScriptEngine.IsGlobalVar("level_22_higgins_defeated"))
        if (Game.Instance.PlayerData.isPowerUpAvailable(PlayerData.PowerUpType.PUT_KIWANO))
            trigger();
        else
        {
            Boss b = chico.GetComponent<Chico>();
            if (b == null)
            {
                Debug.LogWarning("Chico not found!");
            }
            b.StartBossFight();
            b.EndBossFight(trigger);
        }
    }

}
