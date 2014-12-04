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
	private float 		m_speed;
	private float 		m_fly;
	private bool 		m_jump;
	private bool 		m_flyStart;
	private float 		m_oldPositionX 			= 0.0f;
	private float 		m_lastHit;
	private	bool		m_gameOver;
	
	private PlayerData			m_playerData;
	private CharacterController m_controller;

	#endregion

	#region Start
	// Use this for initialization
	private void Start () 
	{
		m_speed 	= 0;
		m_fly 		= 0;
		m_jump 		= false;
		m_flyStart 	= false;
		m_lastHit 	= Time.time - 2;
		m_gameOver 	= false;
		
		// Get player data
		m_playerData = Game.Instance.PlayerData;
		
		// Get character controller
		m_controller = GetComponent<CharacterController> ();
	}
	#endregion

	#region Update
	// Update is called once per frame
	void Update () 
	{
		if (m_gameOver)
			return;

		// pressed keys
		bool keyD 			= Input.GetButton 		(KeyMapping.KEY_ACTION_MOVE_RIGHT);
		bool keyA 			= Input.GetButton 		(KeyMapping.KEY_ACTION_MOVE_LEFT);
		bool jumpKeyDown 	= Input.GetButtonDown 	(KeyMapping.KEY_ACTION_JUMP);
		bool jumpKey 		= Input.GetButton 		(KeyMapping.KEY_ACTION_JUMP);
		bool shoot 			= Input.GetButtonDown	(KeyMapping.KEY_ACTION_SHOOT);

		// falling?
		if(!m_controller.isGrounded)
			m_jump = true;

		// movement right&left
		if (keyD)
		{
			m_speed += GameConfig.BILLY_MAX_SPEED * 0.5f * Time.deltaTime;
			if(m_speed > GameConfig.BILLY_MAX_SPEED)
				m_speed = GameConfig.BILLY_MAX_SPEED;
		} 
		else if(keyA)
		{
			m_speed -= GameConfig.BILLY_MAX_SPEED * 0.5f * Time.deltaTime;
			if(m_speed <  -GameConfig.BILLY_MAX_SPEED)
				m_speed = -GameConfig.BILLY_MAX_SPEED;
		}
		else
		{
			if(Mathf.Abs(m_speed - (m_speed * (1 - Time.deltaTime * 2))) > GameConfig.BILLY_MAX_SPEED * 0.5f *Time.deltaTime)
				m_speed -= GameConfig.BILLY_MAX_SPEED * 0.5f * Time.deltaTime * (m_speed / Mathf.Abs(m_speed));
			else
				m_speed *= (1 - Time.deltaTime * 2);
			if(m_speed < 0.1f && m_speed > -0.1f)
				m_speed = 0;
		}

		// jump and fly
		// movement high&down
		if ((jumpKeyDown || jumpKey) && !m_jump) 
		{
			m_jump = true;
			m_fly = GameConfig.BILLY_JUMP_START_SPEED;
		} 
		else if (jumpKeyDown && m_jump) 
		{
			// flying
			m_flyStart = true;
			if(m_fly > 0)
				m_fly += Physics.gravity.y * Time.deltaTime;
			else
				m_fly += Physics.gravity.y * Time.deltaTime * GameConfig.BILLY_FLYING_FACTOR;
		} 
		else if (m_flyStart && m_jump && jumpKey && m_playerData.isPowerUpAvailable(PlayerData.PowerUpType.PUT_ORANGE)) 
		{
			if(m_fly > 0)
				m_fly += Physics.gravity.y * Time.deltaTime;
			else
				m_fly += Physics.gravity.y * Time.deltaTime * GameConfig.BILLY_FLYING_FACTOR;
		} 
		else if (m_jump) 
		{
			m_fly += Physics.gravity.y * Time.deltaTime;
		} 
		// nothing?
		else 
		{
		}

		// set new position
		m_controller.Move (new Vector3 (m_speed, m_fly, 0) * Time.deltaTime);

		// x-coord haven't changed?
		if(m_oldPositionX == this.transform.position.x)
			m_speed = 0.0f;
		
		// save last x-coordination
		m_oldPositionX = this.transform.position.x;

		// jump again something?
		if ((m_controller.collisionFlags & CollisionFlags.Above) != 0 && m_fly > 0)
			m_fly = 0;

		// jump/fly finished?
		if(m_controller.isGrounded)
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
		m_fly = GameConfig.BILLY_JUMP_START_SPEED_ENEMY;
	}

	/*
	 * if the enemy get a hit
	 */
	public void hit()
	{
		if(m_lastHit + 2 < Time.time)
		{
			m_playerData.LifePoints--;
			m_lastHit = Time.time;
		}
		if (m_playerData.LifePoints == 0)
			m_gameOver = true;	//Destroy (this.gameObject);
	}
	#endregion


}
