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
	const float 		JUMP_START_SPEED	= 5f;
	const float 		GRAVITATION 		= 9.81f;
	
	private float 		m_direction;
	private float 		m_fly;
	
	public  int			m_lifepoints 		= 1;
	public 	bool		m_canFly			= false;

	private PlayerData			m_playerData;
	private CharacterController m_controller;
	#endregion

	// Use this for initialization
	void Start () 
	{
		m_direction = 1;
		m_fly = 0;

		// Get player data
		m_playerData = Game.Instance.PlayerData;

		// get controller
		m_controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		// at ground and can jump?
		if (m_canFly && m_controller.isGrounded)
			m_fly = JUMP_START_SPEED;

		// save last x-coordination
		float x = this.transform.position.x;

		// 
		if(!m_controller.isGrounded)
			m_fly -= GRAVITATION * Time.deltaTime;
		else
			m_fly = 0;

		// set new position
		m_controller.Move (new Vector3 (m_direction * MAX_SPEED, m_fly, 0) * Time.deltaTime);
		
		// x-coordination haven't change? -> running again some wall
		if (x == this.transform.position.x)
			m_direction *= -1;
	}

	/*
	 * 
	 */
	public void OnControllerColliderHit(ControllerColliderHit _hit)
	{
		// get controller
		CharacterController controller = _hit.controller;

		if (_hit.collider.transform.tag == Tags.TAG_PLAYER)
		{		
			Player _p = (Player) _hit.transform.GetComponent<Player>();
			if((_hit.controller.collisionFlags & CollisionFlags.Below) != 0
			   && (controller.collisionFlags & CollisionFlags.Above) != 0)
			{
				_p.jumpingFromAnEnemy ();
				hit();
			}
			else if(m_playerData.isPowerUpAvailable(PlayerData.PowerUpType.PUT_KIWANO))
			{
				m_playerData.decreaseStockSizeByValue(PlayerData.PowerUpType.PUT_KIWANO, 1);
				hit();
			}
			else
				m_playerData.LifePoints--;
		}
	}


	private void hit ()
	{
		--m_lifepoints;
	}
}
