/*
 * Project:	Billy's Payback
 * File:	Chico.cs
 * Authors:	Raik Dankworth
 * Editors:	-
 */
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


/**
 * Controll the boss Chico
 */
public class Chico : Enemy, Boss
{
    #region Variable

    private float                       m_lastSpawnTime;
    private bool                        m_allowToSpawn;
    private GameObject                  m_spawningCrewPrefab;
    private GameObject                  m_chicosActiveCrew;
    private Vector3                     m_littleBirdLocalScale;

    // death handling
    private bool                        m_isAlive;
    private LinkedList<Action>          m_deathEvent = new LinkedList<Action>();
    private Antonio                     m_antonio;


    #endregion

    // Use this for initialization
	void Start () 
    {
        // init values
        m_allowToMove = true;
        m_canFly = false;
        m_canFall = false;

        // base start
        base.Awake();

        // set some important values
        m_direction = 0;
        m_lifepoints = GameConfig.CHICO_LIFE_POINTS;

        // last spawn time for the chico birds
        m_allowToSpawn = false;
        m_lastSpawnTime = 0;
        m_spawningCrewPrefab = Resources.Load<GameObject>("Actors/Enemies/ChicosCrew");
        if (m_spawningCrewPrefab == null)
            Debug.LogWarning("Chico: Crew not found!");
        else if (m_spawningCrewPrefab.transform.childCount == 0)
            Debug.LogWarning("Chico: Prefabs havent birds!");
        else
            m_littleBirdLocalScale = m_spawningCrewPrefab.transform.GetChild(0).localScale;

        // create new 
        m_chicosActiveCrew = new GameObject("ChicosCrew");
        m_chicosActiveCrew.transform.parent = this.transform.parent;
        m_chicosActiveCrew.transform.localScale = new Vector3(1, 1, 1);

        // init death values 
        m_isAlive = true;
        m_deathEvent = new LinkedList<Action>();

        // init antonio
        m_antonio = null;
        GameObject obj = GameObject.FindGameObjectWithTag(Tags.TAG_COMPANION);
        if (obj == null)
        {
            Debug.Log("BS: Antonio dont found!");
            return;
        }

        m_antonio = obj.GetComponent<Antonio>();
	}

    new void FixedUpdate()
    {
        //base.FixedUpdate();
    }

	// Update is called once per frame
    new void Update() 
    {
        if (!m_isAlive && m_chicosActiveCrew.transform.childCount == 0)
        {
            bossFightEnd();
            return;
        }

        //
        if (m_allowToSpawn && m_lastSpawnTime <= 0 && m_chicosActiveCrew.transform.childCount + m_spawningCrewPrefab.transform.childCount <= GameConfig.CHICO_MAXIMALE_NUMBER_OF_LITTLE_BIRD)
        {
            // create new crew
            GameObject newCrew = Instantiate(m_spawningCrewPrefab) as GameObject;
            newCrew.transform.parent = transform.parent;
            newCrew.transform.localScale = m_spawningCrewPrefab.transform.localScale;
            newCrew.transform.position = this.transform.position;

            // put on the child of the new crew to the chicos active crew
            for (int i = newCrew.transform.childCount; --i >= 0; )
            {
                Transform currentChild = newCrew.transform.GetChild(i);
                currentChild.parent = m_chicosActiveCrew.transform;
                currentChild.localScale = m_littleBirdLocalScale;
            }

            // destroy the now empty game object
            Destroy(newCrew);
            
            // save time
            m_lastSpawnTime = GameConfig.CHICO_SPAWING_TIME_DIFFERENCE;

            // let jump 
            m_fly = Mathf.Sqrt(GameConfig.ENEMY_JUMP_HEIGHT * m_controller.height * m_worldScale.y * this.transform.localScale.y * Mathf.Abs(Physics.gravity.y));
        }
        else if (m_allowToSpawn && m_lastSpawnTime > 0)
        {
            m_lastSpawnTime -= Time.deltaTime;
            m_fly += Physics.gravity.y * 2 * Time.deltaTime;
        }

        base.Update();
	}

    // override turnAround
    internal override void turnAround() { }


    /**
     * let begin the boss fight
     */
    public void StartBossFight()
    {
        m_allowToSpawn = true;
        if (m_antonio != null)
            m_antonio.m_allowToThrowPowerUps = false;
        m_lastSpawnTime = 0;
    }

    public void EndBossFight(Action _event)
    {
        m_deathEvent.AddLast(_event);
    }


    public void BreakBossFight()
    {
        m_allowToSpawn = false;
        if (m_antonio != null)
            m_antonio.m_allowToThrowPowerUps = true;

        // destroy the active crew
        for (int i = m_chicosActiveCrew.transform.childCount; --i >= 0; )
            Destroy(m_chicosActiveCrew.transform.GetChild(i).gameObject);

    }

    internal override void die()
    {
        if(m_antonio != null)
            m_antonio.m_allowToThrowPowerUps = true;
        // destroy the chico model
        Destroy(transform.GetChild(0).gameObject);

        // remove the collider/controller component
        this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        this.gameObject.GetComponent<CharacterController>().enabled = false;

        // let dont spawn new birds
        m_allowToSpawn  = false;
        m_isAlive       = false;

        // boss fight at end?
        if (m_chicosActiveCrew.transform.childCount == 0)
            bossFightEnd();
    }

    private void bossFightEnd()
    {
        foreach(Action _event in m_deathEvent)
            _event();

        Destroy(this.gameObject);
    }

}
