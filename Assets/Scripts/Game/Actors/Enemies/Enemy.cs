/*
 * Project:	Billy's Payback
 * File:	Enemy.cs
 * Authors:	Raik Dankworth
 * Editors:	-
 */

using UnityEngine;
using System.Collections;

/**
 * Script to move and controll the enemys
 */
public class Enemy : Hitable
{

	#region Variable	
	protected float 		        m_direction;
    protected float                 m_fly;
	protected float		            m_groundFlyValue;
	private float		            m_lastHit;
    private float                   m_deathValue;
	
	public  int			            m_lifepoints 		= 1;
	public 	bool		            m_canFly			= false;
	public  bool		            m_canFall			= false;
	public  bool		            m_allowToMove		= false;
    public  bool                    m_disableRaycast    = false;
    public  float                   m_speedMultiplier   = 1.0f;
    private  bool                   m_needTurnAround    = false;
	private bool 		            first;
    protected CharacterController   m_controller;

    protected Vector3               m_worldScale = Vector3.zero;

    public string                   m_transmitter = "";
	#endregion

    /**
     * helpfunction for debugging -> only one enemy as transmitter
     */
    private void debug(string _msg)
    {
        if (m_transmitter.Length != 0)
            Debug.Log(m_transmitter + ": " + _msg);
    }

	// Use this for initialization
	internal void Awake ()
    {
        m_direction = (this.transform.rotation.eulerAngles.y < 181 && this.transform.rotation.eulerAngles.y > 179) ? 1 : -1;
		m_fly = 0;

        // Calculate world scale
        m_worldScale = HelperFunctions.getWorldScale(gameObject);

		// get controller
		m_controller = GetComponent<CharacterController>();
        m_groundFlyValue = (!m_canFly) ? -0.001f : 2 * Mathf.Sqrt(GameConfig.ENEMY_JUMP_HEIGHT * m_controller.height * m_worldScale.y * this.transform.localScale.y * Mathf.Abs(Physics.gravity.y));
		m_lastHit = Time.time;
		first = true;

        // calculate death value
        m_deathValue = -Mathf.Sqrt(Mathf.Abs(Physics.gravity.y) * 50 * m_worldScale.y * this.transform.localScale.y);
	}
   
    // Override: Monobehaviour::FixedUpdate()
    internal void FixedUpdate()
    {
        // Local variables
        RaycastHit hit;
        Vector3 rayOrigin = Vector3.zero;
        Vector3 rayDir = Vector3.zero;
        float rayDist = m_controller.stepOffset;
        bool isHit = true;

        // Turn around
        if (m_needTurnAround) 
        {
            m_controller.Move(Vector3.right * -m_direction * m_worldScale.x * 0.05f);
            turnAround();
            m_needTurnAround = false;
        }

        // Allow to move
        if (!m_allowToMove)
            return;

        // First update? Put the enemy on ground
        if (first)
        {
            first = false;
            if (Physics.Raycast(this.transform.position, new Vector3(0, -1, 0), out hit))
                m_controller.Move(new Vector3(0, -hit.distance, 0));
            return;
        }

        // Ignore raycast
        if (m_disableRaycast == false)
        {
            // Calculate ray
            rayOrigin = transform.position + Vector3.left * -m_direction * m_controller.radius * m_worldScale.x;
            rayDir = Vector3.down;

            // Execute raycast
            isHit = Physics.Raycast(rayOrigin, rayDir, out hit, rayDist);
        }

        // nothing hit and not allowed to fall or fly?
        if (isHit == false && !m_canFall && !m_canFly)
            turnAround();
        // on ground
        else if (isHit)
            m_fly = m_groundFlyValue;
        // in the air and allowed to fall/fly
        else
            m_fly += 2 * Physics.gravity.y * Time.deltaTime;
    }

    void OnBecameVisible()
    {
        m_allowToMove = true;
        Debug.Log("HURE");
    }
    void OnBecameInvisible()
    {
        m_allowToMove = false;
        Debug.Log("ARSCH");
    }
	
	// Update is called once per frame
	internal void Update () 
	{
		if (!m_allowToMove)
			return;

		// under the map?
        if (m_fly < m_deathValue)
            Destroy(this.gameObject);

		// set new position
        m_controller.Move(Time.deltaTime * new Vector3(m_direction * GameConfig.ENEMY_MAX_SPEED * m_speedMultiplier, 							// x-direction
		                   	             					m_fly, 	// fly/falling value
		                                					-this.transform.position.z / Time.deltaTime) ); // move object to z = 0
	}


	internal virtual void turnAround()
	{
		m_direction *= -1;
		this.transform.Rotate(new Vector3(0, 1, 0), 180);
	}

    // Override: Hitable::onHit()
    public override void onHit(Hitable _source)
    {
        // Hit by enemy?
        if (_source is Enemy)
            return;

        // Take a hit
        if (m_lastHit + 1 < Time.time)
        {
            --m_lifepoints;
            m_lastHit = Time.time;
        }
        if (m_lifepoints == 0)
            die();
    }

    internal virtual void die()
    {
        Destroy(this.gameObject);
    }

    /**
     * 
     */
    public void OnCharacterControllerHit(object _hitInfo)
    {
        // Local variables
        ControllerColliderHit hitInfo = _hitInfo as ControllerColliderHit;
        Hitable hitable = null;

        // Useless message?
        if (hitInfo == null)
            return;

        // Hitable?
        hitable = hitInfo.controller.gameObject.GetComponent<Hitable>();
        if (hitable == null)
            return;

        // Hit from above?
        if (Vector3.Dot(hitInfo.normal, Vector3.up) >= GameConfig.ENEMY_PLAYER_ABOVE_FACTOR)
        {
            // Player?
            if (hitable is Player)
                (hitable as Player).jumpingFromAnEnemy();
            onHit(hitable);
            return;
        }

        // Notify
        hitable.onHit(this);
    }

    // Override: Monobehaviour::OnControllerColliderHit()
    public void OnControllerColliderHit(ControllerColliderHit _hit)
    {
        // Local variables
        Hitable hitable = _hit.gameObject.GetComponent<Hitable>();

        // Collided sidewards?
        if ((m_controller.collisionFlags & CollisionFlags.CollidedSides) != 0 && hitable is Projectile == false)
        {
            m_needTurnAround = true;
            //m_controller.Move(Vector3.right * -m_direction * m_worldScale.x * 0.05f);
            //turnAround();
        }

        // Hitable?
        if (hitable == null)
            return;

        // Hit from above?
        if (Vector3.Dot(_hit.normal, Vector3.down) >= GameConfig.ENEMY_PLAYER_ABOVE_FACTOR)
        {
            // Player?
            if (hitable is Player)
                (hitable as Player).jumpingFromAnEnemy();
            onHit(hitable);
            return;
        }

        // Notify
        hitable.onHit(this);
    }

}