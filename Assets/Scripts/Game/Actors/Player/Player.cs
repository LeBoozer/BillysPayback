/*
 * Project:	Billy's Payback
 * File:	Player.cs
 * Authors:	Raik Dankworth
 * Editors:	Byron Worms
 */

using UnityEngine;
using System.Collections;


/*
 * Represent the player
 * Controll the Movement and so on
 */
public class Player : MonoBehaviour {

	#region Variable

	const float 		MAX_SPEED 					= 5f;
	const float 		JUMP_START_SPEED			= 10f;
	const float 		JUMP_START_SPEED_FROM_ENEMY	= 10f;
	const float 		GRAVITATION 				= 9.81f;
	const float 		FLYING_FACTOR				= 0.25f;

	//public bool			m_helpJump			= false;
	//public bool			m_helpJumpDown		= false;

	private float 		m_speed;
	private float 		m_fly;
	private bool 		m_jump;
	private bool 		m_flyStart;
	
	private PlayerData	m_playerData;

	#endregion

	#region Start
	// Use this for initialization
	private void Start () 
	{
		m_speed 	= 0;
		m_fly 		= 0;
		m_jump 		= false;
		m_flyStart 	= false;
		
		// Get player data
		m_playerData = Game.Instance.PlayerData;
	}
	#endregion

	#region Update
	// Update is called once per frame
	void Update () 
	{
		// pressed keys
		bool keyD 			= Input.GetButton 		(KeyMapping.KEY_ACTION_MOVE_RIGHT);
		bool keyA 			= Input.GetButton 		(KeyMapping.KEY_ACTION_MOVE_LEFT);
		bool jumpKeyDown 	= Input.GetButtonDown 	(KeyMapping.KEY_ACTION_JUMP);
		bool jumpKey 		= Input.GetButton 		(KeyMapping.KEY_ACTION_JUMP);
		bool shoot 			= Input.GetButtonDown	(KeyMapping.KEY_ACTION_SHOOT);

		// get controller
		CharacterController controller = GetComponent<CharacterController> ();

		// falling?
		if(!controller.isGrounded)
			m_jump = true;

		// movement right&left
		if (keyD)
		{
			m_speed += MAX_SPEED * 0.5f * Time.deltaTime;
			if(m_speed > MAX_SPEED)
				m_speed = MAX_SPEED;
		} 
		else if(keyA)
		{
			m_speed -= MAX_SPEED * 0.5f * Time.deltaTime;
			if(m_speed <  -1 * MAX_SPEED)
				m_speed = - 1* MAX_SPEED;
		}
		else
		{
			if(Mathf.Abs(m_speed - (m_speed * (1 - Time.deltaTime * 2))) > MAX_SPEED * 0.5f *Time.deltaTime)
				m_speed -= MAX_SPEED * 0.5f * Time.deltaTime * (m_speed / Mathf.Abs(m_speed));
			else
				m_speed *= (1 - Time.deltaTime * 2);
			if(m_speed < 0.1f && m_speed > -0.1f)
				m_speed = 0;
		}

		// jump and fly
		// movement high&down
		if (jumpKeyDown && !m_jump) 
		{
			m_jump = true;
			m_fly = JUMP_START_SPEED;
		} 
		else if (jumpKeyDown && m_jump) 
		{
			// flying
			m_flyStart = true;
			if(m_fly > 0)
				m_fly -= GRAVITATION * Time.deltaTime;
			else
				m_fly -= GRAVITATION * Time.deltaTime * FLYING_FACTOR;
		} 
		else if (m_flyStart && m_jump && jumpKey && m_playerData.isPowerUpAvailable(PlayerData.PowerUpType.PUT_ORANGE)) 
		{
			if(m_fly > 0)
				m_fly -= GRAVITATION * Time.deltaTime;
			else
				m_fly -= GRAVITATION * Time.deltaTime * FLYING_FACTOR;
		} 
		else if (m_jump) 
		{
			m_fly -= GRAVITATION * Time.deltaTime;
		} 
		// nothing?
		else 
		{
		}

		// save last x-coordination
		float x = this.transform.position.x;

		// set new position
		controller.Move (new Vector3 (m_speed, m_fly, 0) * Time.deltaTime);

		// x-coordination haven't change? -> running again some wall
		if (x == this.transform.position.x)
			m_speed = 0;

		// jump again something?
		if ((controller.collisionFlags & CollisionFlags.Above) != 0 && m_fly > 0)
			m_fly = 0;

		// jump/fly finished?
		if(controller.isGrounded)
		{
			m_jump = false;
			m_flyStart = false;
			m_fly = 0;
		}

	}
	#endregion

	#region public methoden
	/*
	 * after colliding with an enemy
	 */
	public void jumpingFromAnEnemy()
	{
		m_jump = true;
		m_fly = JUMP_START_SPEED_FROM_ENEMY;
	}

	#endregion


}
