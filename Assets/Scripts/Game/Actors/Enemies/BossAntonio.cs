/*
 * Project:	Billy's Payback
 * File:	BossAntonio.cs
 * Authors:	Raik Dankworth
 */
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*
 * Represent the Antonio
 * Controll the Movement and so on in the boss fight again the player
 */
public class BossAntonio : Hitable, Boss 
{
    // movement
    private float                   m_fly;
    private float                   m_groundedStartFlyValue;
    private Vector3                 m_lastPlayerPosition;
    private float                   m_currentMovementDirection;
    private float                   m_neededDistanceToPlayerForJumping;
    private float                   m_velocityX;

    // die value
    private bool                    m_active;
    private LinkedList<Action>      m_deathEvents;
    private LinkedList<Action>      m_breakEvents;
    private int                     m_lifepoints;
    private float                   m_lastHit;

    // Kiwano
    private readonly float          m_kiwanoRadius = 1.5f;
    private readonly float          m_kiwanosRotationSpeed = 180;

    // Components
    private CharacterController     m_controller;
    private Vector3                 m_worldScale;

    // external objects
    private GameObject              m_player;
    private PlayerData              m_playerData;

    // prefabs
    private GameObject              Kiwano;
    private GameObject              Raspberry;
	private ArrayList				m_kiwanos;

    void Awake()
    {
        // init
        m_active = false;
        m_deathEvents = new LinkedList<Action>();
        m_breakEvents = new LinkedList<Action>();
        m_lifepoints = GameConfig.ANTONIO_LIFE_POINTS;

        // get componente character controller
        m_controller = this.gameObject.GetComponent<CharacterController>();

        // Load Kiwano prefabs
        Kiwano      = Resources.Load<GameObject>("Items/Kiwano");
        m_kiwanos   = null;

        // Load Raspberry prefabs
        Raspberry   = Resources.Load<GameObject>("Items/Raspberry");
        Raspberry.layer = Layer.getLayerIDByName(Layer.LAYER_PROJECTILE_ENEMY);
        Raspberry.tag = Tags.TAG_PROJECTILE_ENEMY;
        
        // get world scale
        m_worldScale = HelperFunctions.getWorldScale(gameObject);

        // init last hit value
        m_lastHit = Time.time - 10f;

        // init movement
        m_fly = 0;
        m_groundedStartFlyValue = 2 * Mathf.Sqrt(GameConfig.ANTONIO_JUMP_HEIGHT * m_controller.height * m_worldScale.y * Mathf.Abs(Physics.gravity.y));
        m_currentMovementDirection = 1;
        m_neededDistanceToPlayerForJumping = Mathf.Abs( m_groundedStartFlyValue / (4 * Physics.gravity.y) * GameConfig.ANTONIO_MAX_SPEED);
        m_velocityX = 0;

        // init tag and layer
        this.gameObject.tag = Tags.TAG_ENEMY;
        this.gameObject.layer = Layer.getLayerIDByName(Layer.LAYER_ENEMY);
    }

	// Use this for initialization
	void Start ()
    {
        // seek player
        m_player = GameObject.FindGameObjectWithTag(Tags.TAG_PLAYER);

        // get player data
        m_playerData = Game.Instance.PlayerData;

        // init kiwanos if possible
        initKiwanos();

        // save last position of the player
        m_lastPlayerPosition = m_player.transform.position;
	}

    private void initKiwanos()
    {
        // cannot use kiwanos?
        if (!m_playerData.isPowerUpAvailable(PlayerData.PowerUpType.PUT_KIWANO) || Kiwano == null)
            return;

        // init arraylist for the kiwanos
        m_kiwanos = new ArrayList();

        // init helper variables
        float height = m_controller.height / 4 * m_worldScale.y;
        float cos = 1, sin = 1;
        Vector3 newPos;

        // create new kiwanos
        while (m_kiwanos.Count < 6)
        {
            Transform newKiwano = ((GameObject)Instantiate(Kiwano)).transform;
            newKiwano.localScale = new Vector3(newKiwano.localScale.x * m_worldScale.x,
                                                newKiwano.localScale.y * m_worldScale.y,
                                                newKiwano.localScale.z * m_worldScale.z);
            newKiwano.parent = this.transform;
            newKiwano.LookAt(this.transform);
            m_kiwanos.Add(newKiwano);
        }

        // set start position for the kiwanos
        cos = Mathf.Cos(2 * Mathf.PI / m_kiwanos.Count);
        sin = Mathf.Sin(2 * Mathf.PI / m_kiwanos.Count);

        newPos = new Vector3(m_kiwanoRadius * m_controller.radius * m_worldScale.x, height, 0);
        foreach (Transform kiwa in m_kiwanos)
        {
            kiwa.position = newPos + this.transform.position;
            newPos = new Vector3(cos * newPos.x - sin * newPos.z,
                                    height,
                                    sin * newPos.x + cos * newPos.z);
        }
    }


    #region Update 

	// Update is called once per frame
	void Update ()
    {
        if (!m_active)
            return;

	    if(m_kiwanos != null)
            updateKiwano();

        if(m_playerData.isPowerUpAvailable(PlayerData.PowerUpType.PUT_RASPBERRY))
            updateRaspberry();

        updateMovement();
    }


    /**
     */
    private void updateKiwano()
    {
        float height = m_controller.height / 4 * m_worldScale.y;
        float cos = 1, sin = 1;
        Vector3 newPos;

        // move the kiwanos
        cos = Mathf.Cos(m_kiwanosRotationSpeed * Mathf.PI / 180f * Time.deltaTime);
        sin = Mathf.Sin(m_kiwanosRotationSpeed * Mathf.PI / 180f * Time.deltaTime);

        foreach (Transform kiwa in m_kiwanos)
        {
            newPos = kiwa.position - this.transform.position;
            newPos = new Vector3(cos * newPos.x - sin * newPos.z,
                                    height,
                                    sin * newPos.x + cos * newPos.z);
            kiwa.position = newPos + this.transform.position;
        }
    }

    private void updateRaspberry()
    {
    }

    private void updateMovement()
    {
        if (m_player == null)
            return;

        float movedirection = Mathf.Sign(m_player.transform.position.x - this.transform.position.x);

        #region vertical

        float playerVelocityY = m_player.transform.position.y - m_lastPlayerPosition.y;
        if (Mathf.Abs(playerVelocityY) > 0.01f && this.transform.position.y < m_player.transform.position.y)
            movedirection *= -1;

        m_velocityX += movedirection * GameConfig.ANTONIO_MAX_SPEED * Time.deltaTime;
        if (Mathf.Abs(m_velocityX) > GameConfig.ANTONIO_MAX_SPEED)
            m_velocityX = movedirection * GameConfig.ANTONIO_MAX_SPEED;


        #endregion

        #region horizontal

        float playDistance = Mathf.Abs(m_player.transform.position.x - this.transform.position.x);
        if (m_controller.isGrounded && playDistance < m_neededDistanceToPlayerForJumping && this.transform.position.y >= m_player.transform.position.y - 0.01f)
            m_fly = m_groundedStartFlyValue;
        else if (m_controller.isGrounded)
            m_fly = 0;
        else
            m_fly += 2 * Physics.gravity.y * Time.deltaTime;

        #endregion

        // move controller
        m_controller.Move(Time.deltaTime * new Vector3(m_velocityX, m_fly, -transform.position.z / Time.deltaTime));

        if (m_currentMovementDirection != movedirection)
            turnAround();

        // save last position of the player
        m_lastPlayerPosition = m_player.transform.position;
    }

    private void turnAround()
    {
        m_currentMovementDirection *= -1;
        this.transform.Rotate(Vector3.up, 180);

    }

    #endregion

    #region Boss Method
    public void StartBossFight()
    {
        m_active = true;
    }

    public void EndBossFight(Action _event)
    {
        m_deathEvents.AddLast(_event);
    }

    public void BreakBossFight()
    {
        foreach(Action e in m_breakEvents)
        {
            if (e != null)
                e();
        }
        m_active = false;
    }

    public void OnBreakBossFight(Action _event)
    {
        m_breakEvents.AddLast(_event);
    }

    #endregion

    #region Public Method
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
            die();
    }

    private void die()
    {
        Debug.Log("antonio::die");
        foreach (Action _a in m_deathEvents)
            _a();
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

    #endregion 
}
