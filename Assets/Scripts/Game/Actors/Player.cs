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
 * Controll the Movement and so on
 */
public class Player : MonoBehaviour {

	#region Variable

	const float 		MAX_SPEED 			= 5f;
	const float 		JUMP_START_SPEED	= 5f;
	const float 		GRAVITATION 		= 2.5f;

	private float 		m_speed;
	private float 		m_fly;
	private  bool 		m_jump;
	private bool 		m_flyStart;

	#endregion

	#region Start
	// Use this for initialization
	void Start () 
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
				m_fly -= GRAVITATION * Time.deltaTime * 0.25f;
		} 
		else if (m_flyStart && m_jump && jumpKey) 
		{
			if(m_fly > 0)
				m_fly -= GRAVITATION * Time.deltaTime;
			else
				m_fly -= GRAVITATION * Time.deltaTime * 0.25f;
		} 
		else if (m_jump) 
		{
			m_fly -= GRAVITATION * Time.deltaTime;
		} 
		// nothing?
		else 
		{
		}

		// set new position
		this.transform.position += new Vector3 (m_speed, m_fly, 0) * Time.deltaTime;

		// jump/fly finished?
		if (this.transform.position.y < 0) 
		{
			m_jump = false;
			m_flyStart = false;
			m_fly = 0;
			this.transform.position = new Vector3 (this.transform.position.x, 0, this.transform.position.z);
		}

	}
	#endregion
}
