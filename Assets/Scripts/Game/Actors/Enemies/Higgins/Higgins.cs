/*
 * Project:	Billy's Payback
 * File:	Higgins.cs
 * Authors:	Raik Dankworth
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/*
 * Represent the Boss Higgins
 * Let him jump and throw eggs
 */
public class Higgins : Enemy, Boss
{

    private Transform               m_player;
    private float                   m_lastEggThrow;
    private GameObject              ThrownEgg;
    private float                   m_startX;
    private Antonio                 m_antonioKI;
    public  bool                    m_antonioStartChaseValue;
    private float                   m_sqrActivateRadius;

    private bool                    m_active;
    private LinkedList<Action>      m_deathEvent = new LinkedList<Action>();
    
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
                m_antonioKI = g.GetComponentInChildren<Antonio>();
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
        base.Awake();

        // higgins not active at start
        m_active = false;

        // calculate the radius to activate Higgins
        m_sqrActivateRadius = Mathf.Pow(m_controller.radius * m_worldScale.x * 5, 2);

        //turnAround();
        m_direction *= -1;
        this.m_groundFlyValue /= 4;
	}

    new void FixedUpdate()
    {
        m_canFly = true;
        m_allowToMove = true;
        base.FixedUpdate();
    }
	
	// Update is called once per frame
    new void Update()
    {
        // not usefull to update?
        if (m_player == null || !m_active)
            return;

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
                // big eggs for higgins
                r.transform.parent = transform.parent;
                r.transform.position = this.transform.position + new Vector3(0, m_controller.height / 2 * m_worldScale.y
                                                                                , -m_controller.radius * m_worldScale.z);
                r.transform.localScale *= 2;
                r.useGravity = false;

                // calculate force to throw this egg
                Vector3 impact = m_player.position - r.transform.position;
                impact.Normalize();
                impact *= GameConfig.HIGGINS_VELOCITY_OF_THE_EGGS;
                
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


    // let the boss fight start
    public void StartBossFight()
    { 
        m_active = true; 
    }

    public void BreakBossFight()
    {
        m_active = false;

        // destroy the eggs
        Transform parent = this.transform.parent;

        for (int i = parent.childCount; i > 0; )
            if (parent.GetChild(i).gameObject != this.gameObject)
                Destroy(parent.GetChild(i).gameObject);
    }

    public void EndBossFight(Action _event)
    {
        m_deathEvent.AddLast(_event);
    }

    internal override void die()
    {
        foreach (Action _a in m_deathEvent)
            _a();
        base.die();
    }
}
