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
public class Player : Hitable
{

	#region Variable
    // Movement
	private bool					m_allowToMove;
    // vertical
	private float 					m_velocityX;
	private float 					m_oldPositionX 			= 0.0f;
	public  float					m_brakeFactor			= 1;
	public  float					m_accelerationFactor	= 1;

    // horizontal
	private float 					m_velocityY;
	private bool 					m_jump;
	private float					m_startJumpTime;
    private float                   m_jumpImpulse;
    private float                   m_maximalJumpTime;
    public  float                   m_minJumpHeight         = 1;
	public  float					m_maxJumpHeight 		= 6;

    // hit control
	private float 					m_lastHit;
	private	bool					m_gameOver;
	private	bool					m_loseLife;
    public  bool                    IGNORE_CHECK_POINTS     = false;
	private Vector3 				m_lastCheckPoint;

    // external variables
	private PlayerData				m_playerData;
    private GameObject              m_playerCamera;
	private CharacterController 	m_controller;
    private Vector3                 m_worldScale = Vector3.zero;

    // power up variables
	private ArrayList				m_kiwanos;
    private GameObject              Kiwano = null;
	private GameObject				Raspberry = null;
	public	float					m_kiwanoDistance = 2;
	public 	float					m_kiwanosRotationSpeed = 180;

	#endregion

	#region Start
    private void Awake()
    {
        m_velocityX = 0;
        m_velocityY = 0;
        m_jump = false;
        m_lastHit = Time.time - 2;
        m_gameOver = false;
        m_kiwanos = new ArrayList();
        m_allowToMove = true;
        m_startJumpTime = 0;

        // Get character controller
        m_controller = GetComponent<CharacterController>();
        m_lastCheckPoint = this.transform.position;

        if (Kiwano == null)
            Kiwano = Resources.Load<GameObject>("Items/Kiwano");

        if (Raspberry == null)
            Raspberry = Resources.Load<GameObject>("Items/Raspberry");

        // Calculate world scale
        m_worldScale = HelperFunctions.getWorldScale(gameObject);

        // Get player data
        m_playerData = Game.Instance.PlayerData;

        // Get Player Camera
        m_playerCamera = GameObject.FindGameObjectWithTag(Tags.TAG_MAIN_CAMERA);

        // Set player instance in game class
        Game.Instance.Player = this;

        // calculate jumpValues
        m_jumpImpulse = 2 * Mathf.Sqrt(m_maxJumpHeight * m_worldScale.y * m_controller.height * transform.localScale.y * Mathf.Abs(Physics.gravity.y));
        m_maximalJumpTime = (m_worldScale.y * m_controller.height * transform.localScale.y * (m_maxJumpHeight - m_minJumpHeight)) / (m_jumpImpulse);
    }

	#endregion

	#region Update
	// Update is called once per frame
	void Update () 
	{
        if (!alive())
            return;

		if(m_allowToMove)
			updateMovement ();

		updateKiwanos ();

		if(m_allowToMove)
			shooting ();
	}

    private bool alive()
    {
        // game over?
        if (m_gameOver)
            return false;

        // get a hit or is under the map?
        if(this.transform.position.y < -50 || m_loseLife)
        {
            // reduce the life points
            --m_playerData.m_lifePoints;
            m_loseLife = false;

            // game over?
            if (m_playerData.m_lifePoints == 0)
            {
                --m_playerData.m_LifeNumber;
                this.transform.position = Vector3.zero;
                m_gameOver = true;
                return false;
            }

            if (this.transform.position.y > -50 && IGNORE_CHECK_POINTS)
                return true;

            // move player to the last check point
			this.transform.position = m_lastCheckPoint;
            
            PlayerCamera camera = m_playerCamera.GetComponent<PlayerCamera>();

            // player is the target object in the camera script?
            if (camera != null && camera.m_object.Equals(transform))
                // set the camera to a new position
                m_playerCamera.transform.position = new Vector3(m_lastCheckPoint.x,
                                                                    m_lastCheckPoint.y + camera.m_YDistance,
                                                                    m_lastCheckPoint.z - camera.m_distance);
        }

        return true;
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
				m_velocityX += GameConfig.BILLY_MAX_SPEED * m_accelerationFactor * Time.deltaTime;
				if(m_velocityX > GameConfig.BILLY_MAX_SPEED)
					m_velocityX = GameConfig.BILLY_MAX_SPEED;
			} 
			else if(keyA)
			{
				m_velocityX -= GameConfig.BILLY_MAX_SPEED * m_accelerationFactor * Time.deltaTime;
				if(m_velocityX <  -GameConfig.BILLY_MAX_SPEED)
					m_velocityX = -GameConfig.BILLY_MAX_SPEED;
			}
			else
			{
				if(Mathf.Abs(m_velocityX - (m_velocityX * m_brakeFactor * Time.deltaTime)) > GameConfig.BILLY_MAX_SPEED * m_accelerationFactor * Time.deltaTime)
						m_velocityX -= GameConfig.BILLY_MAX_SPEED * m_accelerationFactor* Time.deltaTime * (m_velocityX / Mathf.Abs(m_velocityX));
				else
					m_velocityX *= m_brakeFactor * Time.deltaTime;

				if(m_velocityX < 0.1f && m_velocityX > -0.1f)
					m_velocityX = 0;
			}
		#endregion
		// jump and fly
		// movement high&down
		//float lastVeloY = m_velocityY;
		#region horizontal
        // start to jump
		if ((jumpKeyDown || jumpKey) && !m_jump) 
		{
			m_jump = true;
			m_velocityY = m_jumpImpulse;
			m_startJumpTime = 0;
		}
        else if (m_velocityY > 0)
        {
            // update 
            m_velocityY += Physics.gravity.y * Time.deltaTime;

            // like to jump higher?
            if (false && jumpKey && m_startJumpTime < m_maximalJumpTime)
                m_velocityY = m_jumpImpulse;
        }
        // like start to fly?
        else if ((jumpKeyDown || jumpKey) && m_jump && m_playerData.isPowerUpAvailable(PlayerData.PowerUpType.PUT_ORANGE))
            m_velocityY += Physics.gravity.y * Time.deltaTime * GameConfig.BILLY_FLYING_FACTOR;
        // falling
        else
            m_velocityY += Physics.gravity.y * Time.deltaTime;
		#endregion
		// set new position
        float y = m_velocityY + ((m_velocityY == m_jumpImpulse) ? 0 : m_startJumpTime * Physics.gravity.y);
		m_controller.Move (new Vector3 (m_velocityX, y, -this.transform.position.z / Time.deltaTime) * Time.deltaTime);
		m_startJumpTime += Time.deltaTime;
		
		// x-coord haven't changed?
		if(m_oldPositionX == this.transform.position.x)
			m_velocityX = 0.0f;
		
		// save last x-coordination
		m_oldPositionX = this.transform.position.x;
		
		// jump again something?
		if ((m_controller.collisionFlags & CollisionFlags.Above) != 0 && m_velocityY > 0)
			m_velocityY = 0;
		
		// jump/fly finished?
		if(m_controller.isGrounded)
		{
			m_jump = false;
			m_velocityY = 0;
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
		float height = m_controller.height / 2 * m_worldScale.y;
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
                    newKiwano.localScale = new Vector3(newKiwano.localScale.x * m_worldScale.x,
                                                        newKiwano.localScale.y * m_worldScale.y,
                                                        newKiwano.localScale.z * m_worldScale.z);
					newKiwano.parent = this.transform;
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

				newPos = new Vector3(m_kiwanoDistance * m_controller.radius * m_worldScale.x, height, 0);
				foreach(Transform kiwa in m_kiwanos)
				{
					kiwa.position = newPos + this.transform.position;
					newPos = new Vector3(cos * newPos.x - sin * newPos.z,
					                     height, 
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
				                     height, 
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

		bool shoot = Input.GetButtonDown			(KeyMapping.KEY_ACTION_SHOOT);
		shoot &= m_playerData.getPowerUpStockSize 	(PlayerData.PowerUpType.PUT_RASPBERRY) > 0;

		// like and can shooting?
		if (!shoot)
			return;

		// create projectile
        GameObject pro = Instantiate(Raspberry, this.transform.position + Vector3.up * 0.5f * m_controller.height * m_worldScale.y, this.transform.rotation) as GameObject;
        pro.transform.localScale = new Vector3(pro.transform.localScale.x * m_worldScale.x,
                                                        pro.transform.localScale.y * m_worldScale.y,
                                                        pro.transform.localScale.z * m_worldScale.z);
		// add force to projectile
		Rigidbody rb = pro.GetComponent<Rigidbody> ();
		rb.useGravity = false;
		Vector3 force = new Vector3 (50 * GameConfig.BILLY_MAX_SPEED, 0, 0);
		if(m_velocityX != 0)
			force *= m_velocityX / Mathf.Abs (m_velocityX);
		rb.AddForce (force);

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
		m_velocityY = Mathf.Sqrt(2 * m_maxJumpHeight * m_controller.height / Mathf.Abs(Physics.gravity.y));
	}

	/**
	 * block the movement of the player
	 */
	public void blockMovement(bool _k)
	{
		m_allowToMove = !_k;
	}

    // Override: Hitable::onHit()
    public override void onHit(Hitable _source)
    {
        // distribute a hit
        if (m_playerData.getPowerUpStockSize(PlayerData.PowerUpType.PUT_KIWANO) > 0)
        {
            _source.onHit(this);
            m_playerData.decreaseStockSizeByValue(PlayerData.PowerUpType.PUT_KIWANO, 1);
            return;
        }

        // Take a hit
        if (m_lastHit + 2 < Time.time)
        {
            m_lastHit = Time.time;
            m_loseLife = true;
        }
    }

    // Override: Monobehaviour::OnControllerColliderHit()
	public void OnControllerColliderHit(ControllerColliderHit _hit)
	{
        // Send notification
        _hit.gameObject.SendMessageUpwards("OnCharacterControllerHit", _hit, SendMessageOptions.DontRequireReceiver);
	}


    /**
     * set a new CheckPoint
     */
    public void setChechPoint(Vector3 _newCheckPoint)
    {
        m_lastCheckPoint = _newCheckPoint;
    }

	#endregion
}
