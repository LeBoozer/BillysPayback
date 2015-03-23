/*
 * Project:	Billy's Payback
 * File:	CheckPoint.cs
 * Authors:	Raik Dankworth
 * Editors:	-
 */

using UnityEngine;
using System.Collections;
using System;

/*
 * Controll the trigger to set checkpoints for the player
 */
public class CheckPoint : MonoBehaviour, DeActivatable
{

    protected   Player      m_player;
    protected   Action      m_checkPointAction;
    public      bool        m_isActived = true;

	// Use this for initialization
	protected void Awake () 
    {
        m_player = GameObject.FindGameObjectWithTag(Tags.TAG_PLAYER).GetComponent<Player>();
        m_checkPointAction = null;
	}

    /**
     * if the player run in the checkpoint
     */
    public void OnTriggerEnter(Collider _obj)
    {
        if (!_obj.gameObject.tag.Equals(Tags.TAG_PLAYER))
            return;

        trigger();
    }

    /**
     * execute the checkpoint routine
     * possible to trigger from outside
     */
    public void trigger()
    {
        // trigger not actived
        if(!m_isActived)
            return;

        // set checkpoint position and the attendant action
        m_player.setCheckPoint(transform.position, m_checkPointAction);

        // deactived itself
        m_isActived = false;
    }

    public bool isActivated()
    {
        return m_isActived;
    }

    public void onActivate()
    {
        m_isActived = true;
    }

    public void onDeactivate()
    {
        m_isActived = false;
    }


}
