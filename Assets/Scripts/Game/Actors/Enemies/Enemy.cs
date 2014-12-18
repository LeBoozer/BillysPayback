/*
 * Project:	Billy's Payback
 * File:	Enemy.cs
 * Authors:	Raik Dankworth
 * Editors:	-
 */

using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	#region Variable
	const float 		MAX_SPEED 			= 3f;
	const float 		m_jumpHeight		= 2f;
	const float 		GRAVITATION 		= 9.81f;
	
	private float 		m_direction;
	private float 		m_fly;
	private float		m_groundFlyValue;
	private float		m_startJumpTime;
	private float 		m_lastX;

	private float		m_lastHit;
	
	public  int			m_lifepoints 		= 1;
	public 	bool		m_canFly			= false;
	public  bool		m_canFall			= false;
	public  bool		m_allowToMove		= true;
	private bool 		first;
	private PlayerData			m_playerData;
	private CharacterController m_controller;
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
		m_lastX = this.transform.position.x;
		m_lastHit = Time.time;
		first = true;


	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!m_allowToMove)
			return;
		if(first)
		{
			first = false;
			RaycastHit hit;
			if(Physics.Raycast (this.transform.position, new Vector3 (0, -1, 0), out hit))
				m_controller.Move(new Vector3(0, -hit.distance, 0));
		}
		// under the map?
		if (this.transform.position.y < -50)
			Destroy (this.gameObject);

		// not at ground
		if(!m_controller.isGrounded)
		{
			if(!m_canFall && !m_canFly)
				turnAround();
			else
				m_fly -= GRAVITATION * Time.deltaTime;
		}
		// at ground
		else
		{
			m_fly = m_groundFlyValue;
			m_startJumpTime = 0;
		}
		// set new position
		m_controller.Move (Time.deltaTime * new Vector3 (m_direction * MAX_SPEED, 							// x-direction
		                   	             					m_fly + m_startJumpTime * Physics.gravity.y, 	// fly/falling value
		                                					-this.transform.position.z / Time.deltaTime) ); // move object to z = 0
		m_startJumpTime += Time.deltaTime;
		// x-coordination haven't change? -> running again some wall
		if (m_lastX == this.transform.position.x)
			turnAround();

		// save last x-coordination
		m_lastX = this.transform.position.x;

	}

	private void turnAround()
	{
		m_direction *= -1;
		this.transform.Rotate(new Vector3(0, 1, 0), 180);
	}

	/*
	 * processed the collisions of this object
	 */
	public void OnControllerColliderHit(ControllerColliderHit _hit)
	{
		// get controller
		CharacterController controller = _hit.controller;
		//Debug.Log (_hit.collider.tag);
		
		// collision with a projectile from the player
		if (_hit.collider.transform.tag == Tags.TAG_PROJECTILE_PLAYER)
		{
			Destroy(_hit.collider.gameObject);
			hit (false);
		}

		// no player?
		if (_hit.collider.transform.tag != Tags.TAG_PLAYER)
			return;
		
		// collision with the player
		PlayerCollision (_hit.gameObject);
		/*Player _p = (Player) _hit.transform.GetComponent<Player>();
		if((_hit.controller.collisionFlags & CollisionFlags.Below) != 0
		   && (controller.collisionFlags & CollisionFlags.Above) != 0)
		{
			_p.jumpingFromAnEnemy ();
			hit(false);
		}
		else if(m_playerData.isPowerUpAvailable(PlayerData.PowerUpType.PUT_KIWANO))
			hit(true);
		else
			_p.hit ();*/
	}

	/**
	 * Collision with the player
	 */
	public void PlayerCollision(Object _ob)
	{
		GameObject g = _ob as GameObject;

		if (g == null)
			return;
		Debug.Log (g.tag);
		// collision with the player
		Player _p = (Player) g.transform.GetComponent<Player>();
		if(g.transform.position.y >= this.transform.position.y + (m_controller.height - m_controller.radius ) * this.transform.localScale.y)
		{
			_p.jumpingFromAnEnemy ();
			hit(false);
		}
		else if(m_playerData.isPowerUpAvailable(PlayerData.PowerUpType.PUT_KIWANO))
			hit(true);
		else
			_p.hit ();


	}

	/*
	 * if the enemy get a hit
	 */
	private void hit (bool _kiwanoHit)
	{
		if(m_lastHit + 1 < Time.time)
		{
			if(_kiwanoHit)
				m_playerData.decreaseStockSizeByValue(PlayerData.PowerUpType.PUT_KIWANO, 1);

			--m_lifepoints;
			m_lastHit = Time.time;
		}
		if (m_lifepoints == 0)
			Destroy (this.gameObject);
	}
}
