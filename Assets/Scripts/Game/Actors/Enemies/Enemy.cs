/*
 * Project:	Billy's Payback
 * File:	Enemy.cs
 * Authors:	Raik Dankworth
 * Editors:	-
 */

using UnityEngine;
using System.Collections;

public class Enemy : Hitable
{

	#region Variable
	const float 		MAX_SPEED 			= 3f;
	const float 		m_jumpHeight		= 2f;
	const float 		GRAVITATION 		= 9.81f;
	
	private float 		m_direction;
	private float 		m_fly;
	private float		m_groundFlyValue;
	private float		m_startJumpTime;

	private float		m_lastHit;
	
	public  int			m_lifepoints 		= 1;
	public 	bool		m_canFly			= false;
	public  bool		m_canFall			= false;
	public  bool		m_allowToMove		= false;
	private bool 		first;
	private PlayerData			m_playerData;
	private CharacterController m_controller;

    private Vector3     m_worldScale = Vector3.zero;
	#endregion

	// Use this for initialization
	void Start () 
	{
		m_direction = (this.transform.localRotation.eulerAngles.y == 180) ? 1: -1;
		m_fly = 0;
		m_startJumpTime = 0;
		// Get player data
		m_playerData = Game.Instance.PlayerData;

		// get controller
		m_controller = GetComponent<CharacterController>();
		m_groundFlyValue = (!m_canFly) ? - 0.001f : m_jumpHeight * m_controller.height * this.transform.localScale.y * Mathf.Abs (Physics.gravity.y);
		m_lastHit = Time.time;
		first = true;

        // Calculate world scale
        m_worldScale = HelperFunctions.getWorldScale(gameObject);
	}

    // Override: Monobehaviour::FixedUpdate()
    void FixedUpdate()
    {
        // Local variables
        RaycastHit hit;
        Vector3 rayOrigin = Vector3.zero;
        Vector3 rayDir = Vector3.zero;
        float rayDist = m_controller.stepOffset * m_worldScale.y;
        bool isHit = false;

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

        // Calculate ray
        rayOrigin = transform.position + Vector3.left * -m_direction * m_controller.radius * m_worldScale.x + Vector3.up;
        rayDir = Vector3.down;

        // Execute raycast
        isHit = Physics.Raycast(rayOrigin, rayDir, out hit, rayDist);
        if (isHit == false && !m_canFall && !m_canFly)
            turnAround();
        else if (isHit == false)
            m_fly -= Physics.gravity.y * Time.deltaTime;
        else if (isHit == true)
        {
            m_fly = m_groundFlyValue;
            m_startJumpTime = 0;
        }
    }
	
	// Update is called once per frame
	void Update () 
	{
		if (!m_allowToMove)
			return;

		// under the map?
		if (this.transform.position.y < -50)
			Destroy (this.gameObject);

		// set new position
		m_controller.Move (Time.deltaTime * new Vector3 (m_direction * MAX_SPEED, 							// x-direction
		                   	             					m_fly + m_startJumpTime * Physics.gravity.y, 	// fly/falling value
		                                					-this.transform.position.z / Time.deltaTime) ); // move object to z = 0
		m_startJumpTime += Time.deltaTime;
	}

	private void turnAround()
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
            m_controller.Move(Vector3.right * -m_direction * m_worldScale.x * 0.01f);
            turnAround();
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