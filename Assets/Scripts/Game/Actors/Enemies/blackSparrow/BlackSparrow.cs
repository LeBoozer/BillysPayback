/*
 * Project:	Billy's Payback
 * File:	BlackSparrow.cs
 * Authors:	Raik Dankworth
 * Editors:	-
 */
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


/**
 * Controll the boss BlackSparrow
 */
public class BlackSparrow : Enemy, Boss
{
    #region Struct

    class Feather
    {
        public Rigidbody    m_feather;
        public float        m_timeTillTurningPoint;
        public Vector3      m_direction;

        public Feather()
        {
            m_feather = null;
            m_timeTillTurningPoint = 0;
            m_direction = Vector3.zero;
        }

        public bool update(float _timeDelta)
        {
            // feather no alive?
            if (m_feather == null)
                return false;

            // calculate the time until the feather must turn around
            m_timeTillTurningPoint -= _timeDelta;
            // achieved turning point?
            if (m_timeTillTurningPoint < 0)
            {
                m_feather.AddForce(-2 * GameConfig.BS_FEATHER_VELOCITY * m_direction);
                return false;
            }

            return true;
        }
    }

    #endregion

    #region Variable


    private float               m_nextFeatherThrow;

    // death handling
    private bool                m_isActive;
    private LinkedList<Action>  m_deathEvent = new LinkedList<Action>();
    private LinkedList<Feather>  m_flyingFeather;

    // external Objects
    private GameObject          FeatherPrefabs;
    private Transform           m_player;

    #endregion

    // Use this for initialization
	void Awake () 
    {
        // init values
        m_isActive = false;
        m_deathEvent = new LinkedList<Action>();
        m_flyingFeather = new LinkedList<Feather>();
        m_allowToMove = false;
        m_canFly = true;
        m_lifepoints = GameConfig.BS_LIFE_POINTS;
        m_nextFeatherThrow = 0;

        // load feather
        FeatherPrefabs = Resources.Load<GameObject>("Items/Feather");
        if (FeatherPrefabs == null)
            Debug.Log("BS: Feather dont found!");

        // seek player
        GameObject _obj = GameObject.FindGameObjectWithTag(Tags.TAG_PLAYER);
        if (_obj == null)
            Debug.Log("BS: Player dont found!");
        else
            m_player = _obj.transform;

        // call base start
        base.Awake();
	}

    void FixedUpdate()
    {
        // is active?
        if (!m_isActive)
            return;

        base.FixedUpdate();
    }

	// Update is called once per frame
	void Update () 
    {
        // is active?
        if (!m_isActive)
            return;

        updateFlyingFeather();

        // throw feather
        throwFeather();

        // call base update
        base.Update();
    }

    private void updateFlyingFeather()
    {
        if (m_flyingFeather.Count == 0)
            return;

        LinkedListNode<Feather> node = m_flyingFeather.First;
        LinkedListNode<Feather> nextNode;
        do
        {
            Feather f = node.Value;
            nextNode = node.Next;
            if (!f.update(Time.deltaTime))
                m_flyingFeather.Remove(node);
            node = nextNode;
        }
        while (node != null);
    }

    /**
     * 
     */
    private void throwFeather()
    {
        // update next feather throw time
        m_nextFeatherThrow -= Time.deltaTime;

        // cannot throw feather?
        if (m_nextFeatherThrow > 0)
            return;

        createFeather(new Vector3(m_direction * m_controller.radius * m_worldScale.x, m_controller.height / 2 * m_worldScale.y, 0));
        createFeather(new Vector3(0, m_controller.height / 2 * m_worldScale.y, m_controller.radius * m_worldScale.z));
        createFeather(new Vector3(0, m_controller.height / 2 * m_worldScale.y, -m_controller.radius * m_worldScale.z));

        // 
        m_nextFeatherThrow = GameConfig.BS_THROW_DIFFENRENCE;
    }

    private void createFeather(Vector3 _offset)
    {
        // instantiate
        GameObject _newFeather = Instantiate(FeatherPrefabs) as GameObject;

        // set new parent
        _newFeather.transform.parent = this.transform.parent;

        // set position
        _newFeather.transform.position = this.transform.position + _offset;

        // set scale
        _newFeather.transform.localScale = FeatherPrefabs.transform.localScale;

        // get Rigidbody component
        Rigidbody _rig = _newFeather.GetComponent<Rigidbody>();
        if (m_player != null)
        {
            Feather f = new Feather();
            f.m_feather = _rig;
            f.m_direction = m_player.position - _newFeather.transform.position;
            f.m_direction = f.m_direction.normalized;
            f.m_timeTillTurningPoint = 2f;

            _rig.AddForce(GameConfig.BS_FEATHER_VELOCITY * f.m_direction);
            _rig.AddTorque(Vector3.up * 10);

            m_flyingFeather.AddLast(f);
        }
    }

    #region Boss

    // let the boss fight start
    public void StartBossFight()
    {
        m_isActive = true;
    }

    // let the boss fight end
    public void EndBossFight(Action _event)
    {
        m_deathEvent.AddLast(_event);
    }

    // let the boss fight break
    public void BreakBossFight() 
    { 
        m_isActive = false; 
    }

    internal override void die()
    {
        foreach (Action _a in m_deathEvent)
            _a();
        base.die();
    }

    #endregion

}
