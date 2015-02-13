﻿/*
 * Project:	Billy's Payback
 * File:	Higgins.cs
 * Authors:	Raik Dankworth
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Represent the Boss Higgins
 * Let him jump and throw eggs
 */
public class Higgins : Enemy
{

    private Transform   m_player;
    private float       m_sqrActivateRadius;
    private float       m_lastEggThrow;
    private GameObject  ThrownEgg;
    private float       m_startX;
    private Antonio     m_antonioKI;
    public  bool        m_antonioStartChaseValue;
	// Use this for initialization
    void Start () 
    {
        // set lifepoints
        m_lifepoints = GameConfig.HIGGINS_LIFE_POINTS;

        // seek the player 
        GameObject _p = GameObject.FindGameObjectWithTag(Tags.TAG_PLAYER);
        if (_p == null)
            m_player = null;
        else
            m_player = _p.transform;

        // seek Antonio
        m_antonioKI = null;
        GameObject[] _a = GameObject.FindGameObjectsWithTag(Tags.TAG_COMPANION);
        foreach(GameObject g in _a)
            if (g.name.Equals(GameConfig.ANTONIO_GAME_OBJECT_NAME))
            {
                m_antonioKI = g.GetComponent<Antonio>();
                if (m_antonioKI != null)
                    m_antonioStartChaseValue = m_antonioKI.m_chase;
                break;
            }

        // load the ThrownEgg prefab
        ThrownEgg = Resources.Load<GameObject>("Items/ThrownEgg");

        // init last egg throw
        m_lastEggThrow = Time.time - GameConfig.HIGGINS_THROW_DIFFENRENCE;

        // save start X
        m_startX = transform.position.x;

        // let call the method of the base
        m_canFly = true;
        base.Start();

        // calculate the radius to activate Higgins
        m_sqrActivateRadius = Mathf.Pow(m_controller.radius * m_worldScale.x * 5, 2);
	}

    void FixedUpdate()
    {
        m_canFly = true;
        m_allowToMove = true;
        base.FixedUpdate();
    }
	
	// Update is called once per frame
	void Update ()
    {
        // not usefull to update?
        if (m_player == null || (m_player.position - this.transform.position).sqrMagnitude > m_sqrActivateRadius)
        {
            if (m_antonioKI != null)
                m_antonioKI.m_chase = m_antonioStartChaseValue;
            return;
        }

        // let Antonio wait
        if (m_antonioKI != null)
            m_antonioKI.m_chase = false;

        // watch to player
        if (m_direction != Mathf.Sign(m_player.position.x - transform.position.x))
            turnAround();


        // allowed to throw an egg?
        if (m_lastEggThrow + GameConfig.HIGGINS_THROW_DIFFENRENCE < Time.time)
        { 
            // throw egg
            GameObject g = Instantiate(ThrownEgg) as GameObject;
            Rigidbody r = g.GetComponent<Rigidbody>();
            if (r == null)
                Debug.LogError("no Rigidbody");
            else
            {
                r.transform.position = this.transform.position + new Vector3(0, m_controller.height / 2 * transform.localScale.y * m_worldScale.y
                                                                                , -m_controller.radius * transform.localScale.z * m_worldScale.z);
                // big eggs for higgins
                r.transform.parent = transform.parent;
                r.transform.localScale *= 2;
                r.useGravity = false;

                //Debug.Log("Player: " + m_player.position.x + "\tr: " + r.transform.position.x);
                // calculate force to throw this egg
                Vector3 impact = new Vector3((m_player.position.x - r.transform.position.x) / GameConfig.HIGGINS_IMPACT_TIME * m_worldScale.x ,
                                                ((m_player.position.y - r.transform.position.y) * m_worldScale.y / GameConfig.HIGGINS_IMPACT_TIME - m_worldScale.y * Physics.gravity.y / 2 * GameConfig.HIGGINS_IMPACT_TIME),
                                                (m_player.position.z - r.transform.position.z) / GameConfig.HIGGINS_IMPACT_TIME * m_worldScale.z);
                impact = m_player.position - r.transform.position;
                impact *= 100;
                
                r.AddForce(impact, ForceMode.Force);
            }
            // save current time 
            m_lastEggThrow = Time.time;
        }
        
        // movement
        m_controller.Move(Time.deltaTime * new Vector3(m_direction * ((Mathf.Pow(m_startX - transform.position.x, 2) < m_sqrActivateRadius) ? 1 : 0),
                                                        m_fly, 	// fly/falling value
                                                        -this.transform.position.z / Time.deltaTime)); // move object to z = 0
        
	}

}