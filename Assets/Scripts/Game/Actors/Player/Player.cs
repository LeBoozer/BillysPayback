﻿/*
 * Project:	Billy's Payback
 * File:	Player.cs
 * Authors:	Raik Dankworth
 * Editors:	Byron Worms
 */
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


/*
 * Represent the player
 * Controll the Movement and so on
 */
public class Player : Hitable
{
    #region Structs

    private struct OwnMaterial
    {
        public Material     m_material;
        public Color        m_startColor;
    }

    #endregion


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
    private bool                    m_jumpKeyPressed;
    private bool                    m_jumpFromEnenmy;
	private float					m_startJumpTime;
    private float                   m_jumpImpulse;
    private float                   m_maximalJumpTime;
    private float                   m_maxFallingVelocity;
    private float                   m_startJumpHeight;
    private float                   m_realPlayerHeight;
    public  float                   m_gravityMultiply       = 1;


    // hit control
	private float 					m_lastHit;
	private	bool					m_gameOver;
    private float                   m_gameOverTime;
    private GameObject              m_gameOverGUI;
	private	bool					m_loseLife;
	private Vector3 				m_lastCheckPoint;
    private Action                  m_checkPointAction      = null;
    private List<OwnMaterial>       m_material;
    private float                   m_time;

    // impediments values
    private bool                    m_blockJumping;
    private float                   m_velocityFactor;
    private Vector2                 m_slipDirection;

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
        m_jumpFromEnenmy = false;
        m_lastHit = Time.time - GameConfig.BILLY_TIME_BETWEEN_TWO_ACCEPT_HITS;
        m_gameOver = false;
        m_gameOverTime = 0;
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

        // init jump values
        m_realPlayerHeight = m_worldScale.y * m_controller.height * transform.localScale.y;
        m_startJumpHeight = 0;
        m_jumpKeyPressed = false;

        // calculate maximal falling velocity 
        // if higher -> player die!
        m_maxFallingVelocity = -Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * m_gravityMultiply * GameConfig.BILLY_JUMP_MAXIMAL_HEIGHT * 4 * m_realPlayerHeight);

        // impediments
        m_blockJumping = false;
        m_velocityFactor = 1f;
        m_slipDirection = Vector3.zero;

        // seek all materials 
        MeshRenderer[] meshRenderOfBilly = this.gameObject.GetComponentsInChildren<MeshRenderer>();
        m_material = new List<OwnMaterial>();
        foreach (MeshRenderer renderer in meshRenderOfBilly)
            foreach (Material mat in renderer.materials)
            {
                OwnMaterial currentMat = new OwnMaterial();
                currentMat.m_material = mat;
                currentMat.m_startColor = mat.color;
               // Debug.Log("Color: " + mat.color);
                m_material.Add(currentMat);
            }
        //Debug.Log(m_material.Count);


        m_time = GameConfig.BILLY_TIME_BETWEEN_TWO_ACCEPT_HITS / 4;
    }


    void Start()
    {
        // Load Prefabs
        GameObject GameOverGUI = Resources.Load<GameObject>("GUI/GameOver");
        if (GameOverGUI == null)
        {
            Debug.LogWarning("Player: GameOver Game Object not found!");
            return;
        }

        // instantiate the prefab
        m_gameOverGUI = Instantiate(GameOverGUI) as GameObject;
        m_gameOverGUI.name = GameOverGUI.name;
        m_gameOverGUI.SetActive(false);
        
        // find GUI game object
        GameObject GUI = GameObject.Find("GUI");

        if (GUI == null)
        { 
            Debug.LogWarning("Player: GUI not found!");
            return;
        }

        // set the new parent
        m_gameOverGUI.transform.SetParent(GUI.transform);
        m_gameOverGUI.transform.localScale  = GameOverGUI.transform.localScale;
        m_gameOverGUI.transform.position    = GameOverGUI.transform.position;

    }
	#endregion

	#region Update
	/**
     * Update is called once per frame
     */
	void Update () 
	{
        //updateRendererManipulationByHits();

        if (!alive())
            return;

	    updateMovement ();
		updateKiwanos ();

		if(m_allowToMove)
			shooting ();
	}

    private void updateRendererManipulationByHits()
    {
        // nothing to do?
        if (m_lastHit + GameConfig.BILLY_TIME_BETWEEN_TWO_ACCEPT_HITS + 1 < Time.time)
            return;


        // reset?
        if (m_lastHit + GameConfig.BILLY_TIME_BETWEEN_TWO_ACCEPT_HITS < Time.time)
        {
            foreach (OwnMaterial mat in m_material)
                mat.m_material.SetColor(mat.m_startColor.ToString(), mat.m_startColor);
            return;
        }

        //Debug.Log("do something");

        // update
        float currentTime = (Time.time - m_lastHit) % (2 * m_time) ;
        float value = Mathf.Abs(1 - currentTime / m_time);
        //Debug.Log("value: " + value);
        Color currentColor;
        foreach (OwnMaterial mat in m_material)
        {
            currentColor = mat.m_startColor;
            currentColor = new Color(currentColor.r, currentColor.g, currentColor.b, currentColor.a * value);
            //Debug.Log("alpha: " + currentColor.a);
            mat.m_material.SetColor(mat.m_startColor.ToString(), currentColor);
        }

    }

    /**
     * calculate whether the player lose lifepoints/lifenumbers
     * return whether the player is alive
     */
    private bool alive()
    {
        // game over?
        if (m_gameOver && m_gameOverTime > 0)
        {
            m_gameOverTime -= Time.deltaTime;
            return false;
        }
        else if (m_gameOver)
        {
            m_allowToMove = true;
            m_gameOver = false;
            m_gameOverGUI.SetActive(false);
            m_playerData.LifeNumber = GameConfig.BILLY_LIFE_NUMBER;
            m_playerData.LifePoints = GameConfig.BILLY_LIFE_POINT;
        }

        bool underTheMap = m_velocityY < m_maxFallingVelocity && !Physics.Raycast(this.transform.position, Vector3.down);
        
        // get a hit or is under the map?
        if (m_loseLife || underTheMap)
        {
            // reduce the life points
            --m_playerData.m_lifePoints;
            m_loseLife = false;

            // no lifepoints left?
            if (m_playerData.m_lifePoints == 0 || underTheMap)
            {
                --m_playerData.m_LifeNumber;

                // game over?
                if (m_playerData.m_LifeNumber == 0)
                {
                    setToCheckpoint();
                    m_gameOver = true;
                    m_gameOverGUI.SetActive(true);
                    m_gameOverTime = GameConfig.GAME_OVER_SHOW_TIME_SEC;
                    m_allowToMove = false;
                    return false;
                }

                // reset lifepoints
                m_playerData.m_lifePoints = GameConfig.BILLY_LIFE_POINT;

                // set to the last check point
                setToCheckpoint();
            }
        }

        return true;
    }

    /**
     * set the player back to the last checkpoint
     * if the camera have the player as target, set the camera 
     */
    private void setToCheckpoint()
    {
        // move player to the last check point
        this.transform.position = m_lastCheckPoint;

        PlayerCamera camera = m_playerCamera.GetComponent<PlayerCamera>();

        // player is the target object in the camera script?
        if (camera != null && camera.m_object.Equals(transform))
            // set the camera to a new position
            camera.setTheCamera();

        // remove impediments if available
        setImpedimentJumping(false);
        setImpedimentSlip(Vector2.zero);
        setImpedimentVelocity(1f);

        // haven't a check point action?
        if (m_checkPointAction == null)
            return;

        // process the checkpoint action
        m_checkPointAction();
    }

	/**
	 * update the movement of the player
	 */
	private void updateMovement ()
	{
		// pressed keys
		bool keyD 			= Input.GetButton 		(KeyMapping.KEY_ACTION_MOVE_RIGHT) && m_allowToMove;
		bool keyA 			= Input.GetButton 		(KeyMapping.KEY_ACTION_MOVE_LEFT) && m_allowToMove;
		bool jumpKeyDown 	= Input.GetButtonDown 	(KeyMapping.KEY_ACTION_JUMP) && m_allowToMove;
		bool jumpKey 		= Input.GetButton 		(KeyMapping.KEY_ACTION_JUMP) && m_allowToMove;
		
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
        else if (keyA)
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
        float factor = 1 + Mathf.Abs(m_slipDirection.x);
        m_velocityX += GameConfig.BILLY_MAX_SPEED * m_slipDirection.x /* Time.deltaTime*/;
        if (Mathf.Abs(m_velocityX) > GameConfig.BILLY_MAX_SPEED * factor)
            m_velocityX = Mathf.Sign(m_velocityX) * GameConfig.BILLY_MAX_SPEED * factor;

        // move the player mimimum -> trigger event standing up
        m_velocityX += 0.0001f;
		#endregion

		// jump and fly
        // movement high&down
        #region horizontal
        // jump key still pressed first time?
        m_jumpKeyPressed &= jumpKey;
        // start to jump ?
        if (!m_jump && (jumpKeyDown || jumpKey) && !m_blockJumping)
        {
            m_jump = true;
            m_jumpKeyPressed = true;
            m_velocityY = calculateJumpImpulse(GameConfig.BILLY_JUMP_MINIMAL_HEIGHT);
            m_startJumpHeight = this.transform.position.y;
        }
        // like to jump heigher?
        else if (m_jumpKeyPressed && m_startJumpTime > GameConfig.BILLY_MINIMAL_KEYPRESS_TIME_FOR_JUMPING && m_startJumpTime < GameConfig.BILLY_MAXIMAL_KEYPRESS_TIME_FOR_JUMPING)
        {
            // calculate the target height with the keypress time of the jump key
            float nextHeight = GameConfig.BILLY_JUMP_MINIMAL_HEIGHT
                                + (m_startJumpTime - GameConfig.BILLY_MINIMAL_KEYPRESS_TIME_FOR_JUMPING)
                                    / (GameConfig.BILLY_MAXIMAL_KEYPRESS_TIME_FOR_JUMPING - GameConfig.BILLY_MINIMAL_KEYPRESS_TIME_FOR_JUMPING)
                                    * (GameConfig.BILLY_JUMP_MAXIMAL_HEIGHT - GameConfig.BILLY_JUMP_MINIMAL_HEIGHT);
            // reduce the target height with the until now jumped height
            nextHeight -= (this.transform.position.y - m_startJumpHeight) / m_realPlayerHeight;

            // calcualte the jump impulse for the new target height
            m_velocityY = calculateJumpImpulse(nextHeight);
        }
        // like start to fly?
        else if ((m_velocityY < 0) && (jumpKeyDown || jumpKey) && m_jump && m_playerData.isPowerUpAvailable(PlayerData.PowerUpType.PUT_ORANGE))
            m_velocityY += 2 * Physics.gravity.y * m_gravityMultiply * Time.deltaTime * GameConfig.BILLY_FLYING_FACTOR;
        // falling
        else
            m_velocityY += 2 * Physics.gravity.y * m_gravityMultiply * Time.deltaTime;
        m_velocityY -= Physics.gravity.y * m_gravityMultiply * m_slipDirection.y /* Time.deltaTime*/;

        // intercept and correct error in the velocity calculation
        if (float.IsNaN(m_velocityY))
            m_velocityY = 0;
        #endregion

        // set new position
        m_controller.Move(new Vector3(m_velocityX, m_velocityY, -this.transform.position.z / Time.deltaTime) * Time.deltaTime * m_velocityFactor);

        // update the start jump time  
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
        if (m_controller.isGrounded && m_jumpFromEnenmy)
            m_jumpFromEnenmy = false;
        else if (m_controller.isGrounded)
        {
            m_jump = false;
            m_velocityY = 0;
            m_startJumpTime = 0;
        }
	}

    /**
     * calculate the jump impulse for a target height for the player
     */
    private float calculateJumpImpulse(float _jumpHeight)
    {
        return 2 * Mathf.Sqrt(_jumpHeight * m_realPlayerHeight * Mathf.Abs(Physics.gravity.y) * m_gravityMultiply);
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
		Vector3 force = new Vector3 (Mathf.Sign(m_velocityX) * 50 * GameConfig.BILLY_MAX_SPEED * m_worldScale.x, 0, 0);
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
        m_jumpFromEnenmy = true;
		m_jump = true;
        m_velocityY = calculateJumpImpulse(GameConfig.BILLY_JUMP_HEIGHT_FROM_ENEMY);
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
        // Dont take a hit?
        if (m_lastHit + GameConfig.BILLY_TIME_BETWEEN_TWO_ACCEPT_HITS > Time.time)
            return;

        StartCoroutine("doHitFlash");

        // distribute a hit
        if (m_playerData.isPowerUpAvailable(PlayerData.PowerUpType.PUT_KIWANO) && m_playerData.getPowerUpStockSize(PlayerData.PowerUpType.PUT_KIWANO) > 0)
        {
            _source.onHit(this);
            m_playerData.decreaseStockSizeByValue(PlayerData.PowerUpType.PUT_KIWANO, 1);
            m_lastHit = Time.time;
            return;
        }

        m_lastHit = Time.time;
        m_loseLife = true;
    }

    IEnumerator doHitFlash()
    {
        Renderer[] renderer = null;
        Color[] colors = null;

        renderer = GetComponentsInChildren<Renderer>();
        if (renderer == null || renderer.Length == 0)
            yield break;
        colors = new Color[renderer.Length];
        for (int i = 0; i < renderer.Length; ++i)
        {
            colors[i] = renderer[i].material.color;
            renderer[i].material.color = new Color(colors[i].r + 0.35f, colors[i].g + 0.35f, colors[i].b + 0.35f);
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < renderer.Length; ++i)
            renderer[i].material.color = colors[i];
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
    public void setCheckPoint(Vector3 _newCheckPoint, Action _checkPointAction)
    {
        m_lastCheckPoint = _newCheckPoint;
        m_checkPointAction = _checkPointAction;
    }

    /**
     * block the jumping if necessary
     */
    public void setImpedimentJumping(bool _notAllowToJump)
    {
        m_blockJumping = _notAllowToJump;
    }

    /**
     * set a velocity factor for slower or faster movement
     */
    public void setImpedimentVelocity(float _factor)
    {
        m_velocityFactor = _factor;
    }

    /**
     * set a direction for the slip
     */
    public void setImpedimentSlip(Vector2 _direction)
    {
        m_slipDirection = new Vector3(_direction.x, _direction.y, 0);
    }

    /**
     * 
     */
    public void letAcceptNextHit()
    {
        m_lastHit = Time.time - GameConfig.BILLY_TIME_BETWEEN_TWO_ACCEPT_HITS;
    }

	#endregion
}
