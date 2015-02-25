/*
 * Project:	Billy's Payback
 * File:	HuntingEnemy.cs
 * Authors:	Raik Dankworth
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/**
 * 
 */
public class HuntingEnemy : Hitable 
{

    public int                      m_numberOfModels = 1;
    public GameObject               m_model;
    public float                    m_modelScale = 1f;
    public float                    m_huntingVelocityFactor = 1f;

    private GameObject              m_player;
    private Vector3                 m_startPosition;
    List<GameObject>                m_rotatedModels;


	// Use this for initialization
	void Start () 
    {
        // seek the player
        m_player = GameObject.FindGameObjectWithTag(Tags.TAG_PLAYER);


        // save start position
        m_startPosition = transform.position;

        if (m_model == null)
            return;

        // get temporare the sphere
        SphereCollider sphere = GetComponent<SphereCollider>();

        // get temporare the world scale
        Vector3 worldScale = HelperFunctions.getWorldScale(this.gameObject);

        // create instances
        m_rotatedModels = new List<GameObject>();
        GameObject instance;
        for (int i = m_numberOfModels; --i >= 0; )
        {
            instance = Instantiate(m_model) as GameObject;
            instance.transform.parent = this.transform;
            instance.transform.localScale = m_model.transform.localScale * m_modelScale;
            m_rotatedModels.Add(instance);
        }

        Vector3 deepestPointOfSphere = new Vector3(0, -sphere.radius * worldScale.y, 0);
        // if there are only one instance, set the model in the middle
        if (m_numberOfModels == 1)
        {
            instance = m_rotatedModels[0];
            instance.transform.position = this.transform.position + deepestPointOfSphere;
        }
        else
        {
            // init values 
            float cos, sin;
            
            // set new start position for the kiwanos
            cos = Mathf.Cos(2 * Mathf.PI / m_numberOfModels);
            sin = Mathf.Sin(2 * Mathf.PI / m_numberOfModels);

            foreach (GameObject inst in m_rotatedModels)
            {
                inst.transform.position = this.transform.position + deepestPointOfSphere;
                deepestPointOfSphere = new Vector3(cos * deepestPointOfSphere.x - sin * deepestPointOfSphere.y,
                                         sin * deepestPointOfSphere.x + cos * deepestPointOfSphere.y, 0);
            }
        }

        

        foreach (GameObject inst in m_rotatedModels)
        {

            var comps = inst.GetComponents<Component>();
            foreach (Component com in comps)
            {
                // ignore the transform
                if (com is Transform)
                    continue;

                // remove all other components
                Destroy(com);

            }
        }

	}
	
	// Update is called once per frame
	void Update () 
    {
        if (m_numberOfModels != 1)
        {
            float angle = 1;
            Vector3 axis = new Vector3(0, 0, 1);
            this.transform.Rotate(axis, angle);

            foreach (GameObject obj in m_rotatedModels)
                obj.transform.Rotate(axis, angle, Space.Self);
        }

        // move to player
        if (m_player == null)
            return;

        // calculate the direction to the player
        Vector3 directionToPlayer = (m_player.transform.position - transform.position).normalized;

        // move
        transform.position +=  directionToPlayer * GameConfig.HUNTING_ENEMY_VELOCITY * Time.deltaTime * m_huntingVelocityFactor;
    }

    void OnTriggerEnter(Collider _other)
    {
        // ignore all except the player
        if (_other.tag != Tags.TAG_PLAYER || m_player == null)
            return;

        // get  player data 
        PlayerData data = Game.Instance.PlayerData;
        Player _player = m_player.GetComponent<Player>();

        // save whether the kiwano is available
        bool kiwano = data.isPowerUpAvailable(PlayerData.PowerUpType.PUT_KIWANO);

        // ensure that the player cannot survive the hit
        data.setPowerUpAvailable(PlayerData.PowerUpType.PUT_KIWANO, false);
        data.LifePoints = 1;
        _player.letAcceptNextHit();

        // hand out the hit
        _player.onHit(this);

        // let the player again use the kiwano if possible
        data.setPowerUpAvailable(PlayerData.PowerUpType.PUT_KIWANO, kiwano);

        // reset own position
        resetPosition();
    }

    public override void onHit(Hitable _source)
    {
        // Ignore! 
    }

    public void resetPosition()
    { 
        transform.position = m_startPosition;
    }
}
