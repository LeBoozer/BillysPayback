/*
 * Project:	Billy's Payback
 * File:	Player.cs
 * Authors:	Raik Dankworth
 * Editors:	-
 */

using UnityEngine;
using System.Collections;


/*
 * Represent the player
 * Controll the Movement
 */
public class Player : MonoBehaviour {

	#region Variable

	const float 		MAX_SPEED 					= 5f;
	const float 		JUMP_START_SPEED			= 10f;
	const float 		JUMP_START_SPEED_FROM_ENEMY	= 10f;
	const float 		GRAVITATION 				= 9.81f;
	const float 		FLYING_FACTOR				= 0.25f;

	private float 		m_speed;
	private float 		m_fly;
	public bool 		m_jump;
	private bool 		m_flyStart;
	
	public  int			m_livepoints 		= 5;

	// temporär public 
	public  int			m_kiwanoPowerUp 	= 0;
	public  int			m_RaspberryPowerUp 	= 0;
	public  bool		m_canFlying 		= false;
	public  bool		m_canUseKiwanos		= false;
	public  bool		m_canUseRaspberry	= false;

	#endregion

	#region Start
	// Use this for initialization
	private void Start () 
	{
		m_speed 	= 0;
		m_fly 		= 0;
		m_jump 		= false;
		m_flyStart 	= false;
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
		if ((jumpKeyDown || jumpKey) && !m_jump) 
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
		else if (m_flyStart && m_jump && jumpKey && m_canFlying) 
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

	#region enable/disable Feature
	/*
	 * enable or disable flying feature
	 */
	public bool CanFlying
	{
		get { return m_canFlying; }
		set { m_canFlying = value; }
	}

	/*
	 * enable or disable kiwano feature
	 */
	public bool CanUseKiwano
	{
		get { return m_canUseKiwanos; }
		set { m_canUseKiwanos = value; }
	}
	
	/*
	 * enable or disable kiwano feature
	 */
	public bool CanUseRaspberry
	{
		get { return m_canUseRaspberry; }
		set { m_canUseRaspberry = value; }
	}
	
	#endregion
	
	/*
	 * return the current live points
	 */
	public int Livepoints
	{
		get {return m_livepoints; }
	}

	/*
	 * decrease live point 
	 * @return: player alive?
	 */
	public bool reduceLivePoint() 				{ --m_livepoints; return (0 >= m_livepoints); }
	
	/*
	 * add one live point 
	 */
	public void addLivePoint()					{ ++m_livepoints; }

	/*
	 * add kiwano power
	 */
	public void addKiwanoPower(int _value)		{ if(m_canUseKiwanos) m_kiwanoPowerUp += _value; }

	/*
	 * @return: true if the player can use kiwanos
	 */
	public bool haveKiwanoPower()				{ return (!m_canUseKiwanos && m_kiwanoPowerUp > 0); }

	/*
	 * decrease the kiwano power
	 */
	public void reduceKiwanoPower()				{ m_kiwanoPowerUp = Mathf.Max (0, m_kiwanoPowerUp - 1); }

	/*
	 * add the raspberry power
	 */
	public void addRaspberryPower(int _value)	{ m_RaspberryPowerUp += _value; }

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
