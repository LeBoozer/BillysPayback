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
	private float 					m_speed;
	private float 					m_fly;
	private bool 					m_jump;
	private bool 					m_flyStart;
	private float 					m_oldPositionX 			= 0.0f;
	private float 					m_lastHit;
	private	bool					m_gameOver;
	private bool					m_allowToMove;
	private float					m_startJumpTime;
	public  float					m_jumpHeight 			= 5;

	private PlayerData				m_playerData;
	private CharacterController 	m_controller;
	private ArrayList				m_kiwanos;
	
	public  GameObject				Kiwano = null;
	public  GameObject				Raspberry = null;
	public	float					m_kiwanoDistance = 2;
	public 	float					m_kiwanosRotationSpeed = 180;

	#endregion

	#region Start
	// Use this for initialization
	private void Start () 
	{
		m_speed 		= 0;
		m_fly 			= 0;
		m_jump 			= false;
		m_flyStart 		= false;
		m_lastHit 		= Time.time - 2;
		m_gameOver 		= false;
		m_kiwanos 		= new ArrayList ();
		m_allowToMove 	= true;
		m_startJumpTime = 0;

		if (Kiwano == null)
			Kiwano 		= (GameObject) Resources.Load ("Items/KiwanoPowerUp");

		if (Raspberry == null)
			Raspberry 	= (GameObject) Resources.Load ("Raspberry");
		
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

		if(m_allowToMove)
			updateMovement ();

		updateKiwanos ();

		if(m_allowToMove)
			shooting ();
	}

	/**
	 * update the movement of the player
	 */
	private void updateMovement ()
	{
		// pressed keys
		bool keyD 			= Input.GetButton 		(KeyMapping.KEY_ACTION_MOVE_RIGHT);
		bool keyA 			= Input.GetButton 		(KeyMapping.KEY_ACTION_MOVE_LEFT);
		bool jumpKeyDown 	= Input.GetButtonDown 	(KeyMapping.KEY_ACTION_JUMP);
		bool jumpKey 		= Input.GetButton 		(KeyMapping.KEY_ACTION_JUMP);
		
		// falling?
		if(!m_controller.isGrounded)
			m_jump = true;
		
		// movement right&left
		#region vertical
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
		#endregion
		// jump and fly
		// movement high&down
		#region horizontal
		if ((jumpKeyDown || jumpKey) && !m_jump) 
		{
			m_jump = true;
			m_fly = 2 * Mathf.Sqrt(m_jumpHeight * m_controller.height * Mathf.Abs(Physics.gravity.y));
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
		#endregion
		// set new position
		m_controller.Move (new Vector3 (m_speed, m_fly + m_startJumpTime * Physics.gravity.y, 0) * Time.deltaTime);
		m_startJumpTime += Time.deltaTime;
		
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
			m_startJumpTime = 0;
		}
	}

	/**
	 * update the rotated kiwanos
	 */
	private void updateKiwanos ()
	{
		// can do anything?
		if(Kiwano == null || !m_playerData.isPowerUpAvailable(PlayerData.PowerUpType.PUT_KIWANO))
			return;
	
		// have some kiwanos
		if(m_playerData.getPowerUpStockSize(PlayerData.PowerUpType.PUT_KIWANO) > 0)
		{
			float cos = 1 , sin = 1;
			Vector3 newPos;

			int newKiwanoNumber = m_playerData.getPowerUpStockSize(PlayerData.PowerUpType.PUT_KIWANO);
			newKiwanoNumber = (7 < newKiwanoNumber)? 7 : newKiwanoNumber;
			
			// must create or destroy kiwanos?
			if(newKiwanoNumber != m_kiwanos.Count)
			{
				// create new kiwanos
				while(m_kiwanos.Count < newKiwanoNumber)
				{
					Transform newKiwano = ((GameObject) Instantiate(Kiwano)).transform;
					newKiwano.parent = this.transform;
					newKiwano.localScale = new Vector3(2, 2, 2);
					newKiwano.LookAt(this.transform);
					m_kiwanos.Add(newKiwano);
				}
				
				// destroy kiwanos
				while(m_kiwanos.Count > newKiwanoNumber)
				{
					for(int i = newKiwanoNumber; i < m_kiwanos.Count; ++i)
						Destroy(((Transform) m_kiwanos[i]).gameObject);
					m_kiwanos.RemoveRange(newKiwanoNumber, m_kiwanos.Count - newKiwanoNumber);
				}
				
				// set new start position for the kiwanos
				cos = Mathf.Cos(2 * Mathf.PI / m_kiwanos.Count);
				sin = Mathf.Sin(2 * Mathf.PI / m_kiwanos.Count);

				newPos = new Vector3(m_kiwanoDistance * m_controller.radius, 0, 0);
				foreach(Transform kiwa in m_kiwanos)
				{
					kiwa.position = newPos + this.transform.position;
					newPos = new Vector3(cos * newPos.x - sin * newPos.z,
					                     0, 
					                     sin * newPos.x + cos * newPos.z);
				}
			}
			
			// move the kiwanos
			cos = Mathf.Cos(m_kiwanosRotationSpeed * Mathf.PI / 180f * Time.deltaTime);
			sin = Mathf.Sin(m_kiwanosRotationSpeed * Mathf.PI / 180f * Time.deltaTime);

			foreach(Transform kiwa in m_kiwanos)
			{
				newPos = kiwa.position - this.transform.position;
				newPos = new Vector3( cos * newPos.x - sin * newPos.z,
			                       	  0, 
				                      sin * newPos.x + cos * newPos.z);
				kiwa.position = newPos + this.transform.position;
			}
		}

		// haven't kiwanos
		else if(m_kiwanos.Count > 0)
		{
			foreach(Transform t in m_kiwanos)
				Destroy(t.gameObject);
			m_kiwanos.Clear();
		}
	}

	/**
	 * update the raspberry
	 */
	private void shooting ()
	{
		// can shooting?
		if(Raspberry == null || !m_playerData.isPowerUpAvailable(PlayerData.PowerUpType.PUT_RASPBERRY))
			return;

		bool shoot = Input.GetButtonDown	(KeyMapping.KEY_ACTION_SHOOT);
		shoot &= m_playerData.getPowerUpStockSize (PlayerData.PowerUpType.PUT_RASPBERRY) > 0;

		// like and can shooting?
		if (!shoot)
			return;

		// create projectile
		GameObject pro = Instantiate (Raspberry, this.transform.position, this.transform.rotation) as GameObject;

		// add force to projectile
		Rigidbody rb = pro.GetComponent<Rigidbody> ();
		rb.useGravity = false;
		if(m_speed != 0)
			rb.AddForce (m_speed / Mathf.Abs (m_speed) * new Vector3 (100, 0, 0));
		else 
			rb.AddForce (new Vector3 (100, 0, 0));

		// decrease the raspberry power up
		m_playerData.decreaseStockSizeByValue (PlayerData.PowerUpType.PUT_RASPBERRY, 1);

	}

	#endregion

	#region public methoden

	/*
	 * after colliding with an enemy
	 */
	public void jumpingFromAnEnemy()
	{
		m_jump = true;
		//m_fly = GameConfig.BILLY_JUMP_START_SPEED_ENEMY;
		m_fly = Mathf.Sqrt(2 * m_jumpHeight * m_controller.height / Mathf.Abs(Physics.gravity.y));
	}

	/*
	 * if the play get a hit
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

	/**
	 * block the movement of the player
	 */
	public void blockMovement(bool _k)
	{
		m_allowToMove = !_k;
	}

	#endregion


}
