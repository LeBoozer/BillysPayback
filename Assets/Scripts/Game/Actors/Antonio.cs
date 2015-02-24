/*
 * Project:	Billy's Payback
 * File:	Antonio.cs
 * Authors:	Raik Dankworth
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


/*
 * Represent the Antonio
 * Controll the Movement and so on
 */
public class Antonio : MonoBehaviour
{
    #region Structs

    enum MovementDirection
    {
        eNothing    = 0,
        eRight      = (1 << 0),
        eLeft       = (1 << 1)
    }
    
    struct PlayerValues
    {
        public int     m_lifePoints;
        public int     m_lifeNumbers;
        public bool    m_kiwanos;
        public bool    m_raspberry;
        public bool    m_orange;
    }

    #endregion

    #region Variables

    // variables
    public bool                         m_simpleMovement            = true;
    public bool                         m_chase                     = true;
    private GameObject                  m_way;
    private float                       m_realAntonioHeight;
    private bool                        m_waiting;
    private int                         m_ignoreLayerMask;
    private GameObject                  m_sphere                    = null;
    private float                       m_lastSpeakingTimeStamp;
    private Queue<string>               m_speakingQueue;
    private Transform                   m_currentSpokenText;

    // movement
    private float                       m_velocityX;
    private Vector3                     m_oldPosition;
    private float                       m_velocityY;
    private float                       m_brakeFactor               = 1;
    private float                       m_accelerationFactor        = 1;

    // chase variables
    private int                         m_iteration;
    private int                         m_samplingRate              = 10;
    public float                        m_targetDistance;
    private Vector3                     m_nextWayPoint;
    
    // throw variable
    private PlayerValues                m_lastPlayerValues;
    private Queue<double>               m_playerHitTimeStamp;
    private double                      m_lastGiftTimeStamp;
    private GameObject                  Kiwano                      = null;
    private GameObject                  Raspberry                   = null;
    private GameObject                  Life                        = null;

    // external objects
    private GameObject                  m_player;
	private PlayerData		            m_playerData;
    private CharacterController         m_controller;
    private Vector3                     m_worldScale;
    private DialogueWindowScript        m_textDisplayScript;
    private Camera                      m_mainCamera;

    // constants
    public static readonly string       GAME_OBJECT_WAY_NAME        = "Way";
    private const double                IGNORE_HIT_TIME_DIFFERENCE  = 2;
    private const int                   NUMBER_OF_NEEDED_HITS       = 2;
    private const double                MIN_GIFT_TIME_DIFFERENCE    = 2;
    private const double                GIFT_TIME_DIFFERENCE        = 10;
    private const int                   MAXIMAL_POWER_UPS_NUMBER    = 3;
    private const float                 MAXIMAL_SPEAKING_TIME       = 2;

    private readonly string[]           ANTONIO_GIFT_SENTENCES = { "Halt durch!",
                                                                     "Hier bitte sehr!",
                                                                     "Ich hoffe, das hilft dir!",
                                                                     "Pass auf dich auf!"
                                                                 };
    #endregion

    // Use this for initialization
	void Start () 
    {
        // init 
        m_iteration = 0;

        // no way point
        m_nextWayPoint = Vector3.zero;

        // dont move
        m_velocityX = 0;
        m_velocityY = 0;

        
        // ignore everything except the enviroment
        m_ignoreLayerMask = ~(1 << LayerMask.NameToLayer(Layer.LAYER_COLLECTABLE)
                                | 1 << LayerMask.NameToLayer(Layer.LAYER_ENEMY)
                                | 1 << LayerMask.NameToLayer(Layer.LAYER_PLAYER)
                                | 1 << LayerMask.NameToLayer(Layer.LAYER_PROJECTILE_ENEMY)
                                | 1 << LayerMask.NameToLayer(Layer.LAYER_PROJECTILE_PLAYER));

        // seek player
        m_player = GameObject.FindGameObjectWithTag(Tags.TAG_PLAYER);
        if (m_player == null)
        {
            Debug.LogError("Antonio: Player not found!");
            m_accelerationFactor = 1;
            m_brakeFactor = 1;
        }
        else
        {
            Player _player = m_player.GetComponent<Player>();
            m_accelerationFactor = _player.m_accelerationFactor;
            m_brakeFactor = _player.m_brakeFactor;
        }

        // get player data
        m_playerData = Game.Instance.PlayerData;

        // throw values
        m_lastPlayerValues = new PlayerValues();
        m_lastPlayerValues.m_lifePoints     = m_playerData.LifePoints;
        m_lastPlayerValues.m_lifeNumbers    = m_playerData.LifeNumber;
        m_lastPlayerValues.m_kiwanos        = m_playerData.isPowerUpAvailable(PlayerData.PowerUpType.PUT_KIWANO);
        m_lastPlayerValues.m_raspberry      = m_playerData.isPowerUpAvailable(PlayerData.PowerUpType.PUT_RASPBERRY);
        m_playerHitTimeStamp = new Queue<double>();
        m_lastGiftTimeStamp = Time.time - GIFT_TIME_DIFFERENCE;

        // load prefabs 
        if (Kiwano == null)
            Kiwano      = Resources.Load<GameObject>("Items/KiwanoPowerUp");
        if (Raspberry == null)
            Raspberry   = Resources.Load<GameObject>("Items/RaspberryPowerUp");
        if (Life == null)
            Life        = Resources.Load<GameObject>("Items/LifePowerUp");

        // Get character controller
        m_controller = GetComponent<CharacterController>();

        // get or create the way
        m_way = getWay();

        // Calculate world scale
        m_worldScale = HelperFunctions.getWorldScale(gameObject);

        // calculate the height of antonio
        m_realAntonioHeight = m_worldScale.y * m_controller.height * transform.localScale.y;

        // calculate the next target distance to the player and go in the waiting state
        m_waiting = false;
        m_targetDistance = 0;
        calculateNextTargetDistance();

        // save last position
        m_oldPosition = transform.position;

        
        // The text for displaying the help text
        GameObject tempGO = GameObject.Find("Dialog");
        if (tempGO == null)
            Debug.LogWarning("Antonio: GameObject SimpleTextDisplay not found!");
        else
        {
            // get component text
            m_textDisplayScript = tempGO.GetComponent<DialogueWindowScript>();
            if (m_textDisplayScript == null)
                Debug.LogWarning("Antonio: Speaking Text Display not found!");
        }

        // init speaking queue
        m_speakingQueue = new Queue<string>();
        m_currentSpokenText = null;

        // get camera
        tempGO = GameObject.FindGameObjectWithTag(Tags.TAG_MAIN_CAMERA);
        if (tempGO == null)
            Debug.LogWarning("Antonio: Main camera dont found!");
        else
            m_mainCamera = tempGO.GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        // haven't the player?
        if (m_player == null)
        {
            // seek the player
            m_player = GameObject.FindGameObjectWithTag(Tags.TAG_PLAYER);

            // dont find the player?
            if (m_player == null)
            {
                Debug.LogError("Antonio: Player not found!");
                return;
            }
        }
        
        // update antonio
        if (m_simpleMovement && m_chase)
            this.transform.position = m_player.transform.position + new Vector3(-m_controller.radius * 2 * m_worldScale.x * transform.localScale.x, 0, m_controller.radius * 2 * m_worldScale.z * transform.localScale.z);
        else if (m_simpleMovement)
            runAheadUpdateSimple();
        // complex movement
        else if (m_chase)
            chaseUpdate();
        else
            runAheadUpdate();

        // threw power ups
        throwPowerUps();

        // speak
        updateSpeaking();
	}

    /**
     * let antonio run ahead 
     */
    private void runAheadUpdateSimple()
    {
        // get the next way point if necessary
        float height = m_controller.stepOffset;
        bool newWayPoint = m_nextWayPoint == Vector3.zero                                                  // default way point?
                            || (m_nextWayPoint - transform.position).sqrMagnitude < height * height;           // way point achieved?

        // fetch the next way point
        if (newWayPoint)
            getNextWayPointSimple();    

        // waiting?
        if (m_nextWayPoint == Vector3.zero
             || (transform.position - m_player.transform.position).sqrMagnitude > m_targetDistance * m_targetDistance)
            return;

        // calcualte distance between position and way point
        float distance = (m_nextWayPoint - transform.position).magnitude;
        if(distance > GameConfig.BILLY_MAX_SPEED * Time.deltaTime)
            distance = GameConfig.BILLY_MAX_SPEED * Time.deltaTime / distance;
        else
            distance = 1;

        // move Antonio
        m_controller.Move( distance * (m_nextWayPoint - transform.position));
    }

    /**
     * pull the next way point for Antonio
     */
    private void getNextWayPointSimple()
    {
        // if way points found
        if (0 != m_way.transform.childCount)
        {
            // set the next possible way point
            m_nextWayPoint = m_way.transform.GetChild(0).transform.position;
            GameObject.Destroy(m_way.transform.GetChild(0).gameObject);
        }
        else
            m_nextWayPoint = Vector3.zero;
    }

    #region Complex Movement

    /**
     * antonio chase billy
     */
    private void chaseUpdate()
    {
        // save the actual position of the player
        if (m_iteration % m_samplingRate == 0)
            createAWayPoint();
        ++m_iteration;

        // get the next way point if necessary
        float height = m_controller.height * m_worldScale.y * transform.localScale.y;

        bool newWayPoint = (m_nextWayPoint == Vector3.zero                                                  // default way point?
                            || (m_nextWayPoint - transform.position).magnitude < height * height            // way point achieved?
                            || !usefulWayPoint());                                                          // generally now an useless way point?
        Vector3 lastWayPoint = m_nextWayPoint;
        // check the usefulness of the current way point
        if (newWayPoint)
        {
            //Debug.Log("calculate new way point");
            // fetch the next way point
            getNextWayPoint();

            // --- debugging beginn ---
            if (m_sphere != null)
                GameObject.Destroy(m_sphere);
            m_sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            m_sphere.GetComponent<SphereCollider>().enabled = false;
            m_sphere.transform.parent = transform.parent;
            m_sphere.transform.position = m_nextWayPoint;
            // --- debugging end ---
        }

        // find not an usefull way point -> dont move
        if (m_nextWayPoint == Vector3.zero || (newWayPoint && lastWayPoint == m_nextWayPoint))
        {
            Debug.Log("return: " + (!usefulWayPoint()) + "\t" + (m_nextWayPoint == Vector3.zero));
            move(MovementDirection.eNothing, 0);
            return;
        }
        // movement in the chase state
        MovementDirection nextMovement = MovementDirection.eNothing;
        float jumpHeight = 0; // GameConfig.BILLY_JUMP_MAXIMAL_HEIGHT;

        // calculate the next target distance if necessary
        float distanceToPlayer = (m_player.transform.position - transform.position).sqrMagnitude;
        if (m_waiting ^ (distanceToPlayer < m_targetDistance ))
            calculateNextTargetDistance();
        // dont waiting?
        if (!m_waiting)
        {

            // make a raycast downward in front of Antonio
            RaycastHit hitInfo;
            Vector3 rayOrigin, rayDirection;
            rayOrigin = transform.position + new Vector3(m_controller.radius * Mathf.Sign(m_velocityX) * m_worldScale.x * transform.localScale.x, 0, 0);
            rayDirection = Vector3.down;
            bool foundGround = Physics.Raycast(rayOrigin, rayDirection, out hitInfo, float.MaxValue, m_ignoreLayerMask);

            // make a ray cast between the current position and the next way point
            rayOrigin += new Vector3(0, m_realAntonioHeight / 2 , 0);
            rayDirection = m_nextWayPoint - rayOrigin + new Vector3(0, m_controller.stepOffset, 0);
            bool somethingBetween = Physics.Raycast(rayOrigin, rayDirection, out hitInfo, rayDirection.magnitude, m_ignoreLayerMask);
            // have a canyon in front of Antonio?
            if (!foundGround || somethingBetween)
                jumpHeight = GameConfig.BILLY_JUMP_MAXIMAL_HEIGHT;

            // calculate move direction
            float currentMoveDirection = Mathf.Sign(m_velocityX);
            float targetMoveDirection = Mathf.Sign(m_nextWayPoint.x - transform.position.x);
            //Debug.Log("direction: " + currentMoveDirection);
            if (currentMoveDirection == targetMoveDirection && currentMoveDirection * (transform.position.x + m_velocityX * Time.deltaTime) < currentMoveDirection * m_nextWayPoint.x)
            {
                if (currentMoveDirection > 0)
                    nextMovement = MovementDirection.eRight;
                else
                    nextMovement = MovementDirection.eLeft;
            }
            else if (currentMoveDirection == targetMoveDirection)
                nextMovement = MovementDirection.eNothing;
            else if (currentMoveDirection == 1)
                nextMovement = MovementDirection.eLeft;
            else
                nextMovement = MovementDirection.eRight;
            Debug.Log(nextMovement + "\t" + jumpHeight);
        }
        else
            Debug.Log("waiting: " + nextMovement + "\t" + jumpHeight);

        // move
        move(nextMovement, jumpHeight);
    }

    /**
     * controll the movement
     */
    private void move(MovementDirection nextMovement, float _jumpHeight)
    {
        // check the input jump height
        if (_jumpHeight <= 0)
            _jumpHeight = 0;
        else if (_jumpHeight > GameConfig.BILLY_JUMP_MAXIMAL_HEIGHT)
            _jumpHeight = GameConfig.BILLY_JUMP_MAXIMAL_HEIGHT;
        else if (_jumpHeight < GameConfig.BILLY_JUMP_MINIMAL_HEIGHT)
            _jumpHeight = GameConfig.BILLY_JUMP_MINIMAL_HEIGHT;

        #region vertical
        // didn't/couldn't move in x-direction?
        /*if (m_oldPosition.x == transform.position.x)
            m_velocityX = 0;
        */
        // move
        switch (nextMovement)
        {
            case MovementDirection.eRight:
            {
                m_velocityX += GameConfig.BILLY_MAX_SPEED * m_accelerationFactor * Time.deltaTime;
                if (m_velocityX > GameConfig.BILLY_MAX_SPEED)
                    m_velocityX = GameConfig.BILLY_MAX_SPEED;
                break;
            }
            case MovementDirection.eLeft:
            {
                m_velocityX -= GameConfig.BILLY_MAX_SPEED * m_accelerationFactor * Time.deltaTime;
                if (m_velocityX < -GameConfig.BILLY_MAX_SPEED)
                    m_velocityX = -GameConfig.BILLY_MAX_SPEED;
                break;
            }
            default: // case MovementDirection.eNothing:
            {
                if (Mathf.Abs(m_velocityX - (m_velocityX * m_brakeFactor * Time.deltaTime)) > GameConfig.BILLY_MAX_SPEED * m_accelerationFactor * Time.deltaTime)
                    m_velocityX -= GameConfig.BILLY_MAX_SPEED * m_accelerationFactor * Time.deltaTime * (m_velocityX / Mathf.Abs(m_velocityX));
                else
                    m_velocityX *= m_brakeFactor * Time.deltaTime;

                if (m_velocityX < 0.1f && m_velocityX > -0.1f)
                    m_velocityX = 0;
                break;
            }
        }
        #endregion

        #region horizontal

        // didn't/couldn't move in y-direction?
        /*if (m_oldPosition.y == transform.position.y)
            m_velocityY = 0;*/
        // jump
        if (_jumpHeight != 0 && m_controller.isGrounded)
            m_velocityY = calculateJumpImpulse(_jumpHeight);
        else if(m_controller.isGrounded)
            m_velocityY = 0;
        else 
            m_velocityY += 2 * Time.deltaTime * Physics.gravity.y;
        #endregion
        //Debug.Log("velocity: " + m_velocityY);
        m_controller.Move(Time.deltaTime * new Vector3(m_velocityX, m_velocityY, -(transform.position.z - m_oldPosition.z) / Time.deltaTime));

        m_oldPosition = transform.position;

    }

    /**
     * antonio run ahead and billy must chase
     */
    private void runAheadUpdate()
    {
        if (m_nextWayPoint == Vector3.zero)
            getNextWayPoint();

        // TODO: moveToCurrentWayPoint
    }

    /**
     * calcualte the next target distance to billy
     */
    private void calculateNextTargetDistance()
    {
        m_waiting ^= true;
        m_targetDistance = GameConfig.ANTONIO_DISTANCE_MEAN_VALUE + Mathf.Pow(2 * Random.value - 1, 3) * GameConfig.ANTONIO_DISTANCE_VARIANCE;
        //m_targetDistance *= m_worldScale.x * transform.localScale.x * m_controller.radius * 2;
    }

    /**
     * calculate the jump impulse for a target height for the player
     */
    private float calculateJumpImpulse(float _jumpHeight)
    {
        return 2 * Mathf.Sqrt(_jumpHeight * m_realAntonioHeight * Mathf.Abs(Physics.gravity.y));
    }

    /**
     * return the game object with the way
     * if the way doesnt exists, it is generated
     */
    private GameObject getWay()
    {
        // exists a way?
        Transform transOfWay = transform.parent.FindChild(GAME_OBJECT_WAY_NAME);
        if (transOfWay != null)
            return transOfWay.gameObject;

        // create the way as empty game object
        GameObject gameObjectOfWay = new GameObject(GAME_OBJECT_WAY_NAME);
        gameObjectOfWay.transform.parent = transform.parent;
        return gameObjectOfWay;
    }

    /**
     * create a new way point for antonio 
     */
    private void createAWayPoint()
    {
        // create a new way point for antonio
        GameObject newWayPoint = new GameObject("way point");

        // attach to the way
        newWayPoint.transform.parent = m_way.transform;

        // set the way point at the current position of the player
        newWayPoint.transform.position = m_player.transform.position;
    }

    /**
     * pull the next way point for antonio
     */
    private void getNextWayPoint()
    {
        // save current way point
        Vector3 lastWayPoint = m_nextWayPoint;

        // local variable 
        int index = 0;
        RaycastHit hitInfo;

        // until a useful way point found
        while(index < m_way.transform.childCount)
        {
            // set the next possible way point
            m_nextWayPoint = m_way.transform.GetChild(index).transform.position;

            // find a gameobject?
            if (Physics.Raycast(m_nextWayPoint + Vector3.up, Vector3.down, out hitInfo, float.MaxValue, m_ignoreLayerMask))
            {
                // set ground as way point
                m_nextWayPoint = hitInfo.point;

                // run ahead?
                if (!m_chase)
                    break;

                // chase 
                // check usefulness of the potential way point
                if (usefulWayPoint())
                    break;
            }
            // useless way point
            ++index;
        }
        // only useless way points found? -> last way point is next way point
        if (index == m_way.transform.childCount)
        {
            m_nextWayPoint = lastWayPoint;
            --index;
        }

        // remove all now useless way points from the way
        while (index >= 0 && m_way.transform.childCount != 0)
            GameObject.Destroy(m_way.transform.GetChild(index--).gameObject);
        
    }

    /**
     * check whether the current way point is useful
     * between player and antonio in at least one component? --> usefull way point
     */
    private bool usefulWayPoint()
    {
        float dir = Mathf.Sign(m_player.transform.position.y - transform.position.y);
        Vector3 playerPos = m_player.transform.position + dir * new Vector3(0, m_controller.stepOffset, 0);
        Vector3 ownPos = transform.position - dir * new Vector3(0, m_controller.stepOffset , 0);
        return (Mathf.Min(ownPos.x, playerPos.x) <= m_nextWayPoint.x && Mathf.Max(ownPos.x, playerPos.x) >= m_nextWayPoint.x)               // for x-value?
                        && (Mathf.Min(ownPos.y, playerPos.y) <= m_nextWayPoint.y && Mathf.Max(ownPos.y, playerPos.y) >= m_nextWayPoint.y);  // for y -value?
    }

    #endregion

    /**
     * let Anwtonio throw power ups for the player if necessary
     */ 
    private void throwPowerUps()
    {
        // useless time hits?
        if (m_playerHitTimeStamp == null)
            Debug.Log("Queue: " + m_playerHitTimeStamp);
        if(m_playerHitTimeStamp.Count > 0)
            Debug.Log("peek: " + m_playerHitTimeStamp.Peek());
        while (m_playerHitTimeStamp.Count > 0 && m_playerHitTimeStamp.Peek() + IGNORE_HIT_TIME_DIFFERENCE < Time.time)
            m_playerHitTimeStamp.Dequeue();

        // in last tick a hit by player?
        if (m_playerData.LifePoints < m_lastPlayerValues.m_lifePoints)
            m_playerHitTimeStamp.Enqueue(Time.time);

        // a lot of hits in last time?
        if (m_playerHitTimeStamp.Count > NUMBER_OF_NEEDED_HITS && m_lastGiftTimeStamp + MIN_GIFT_TIME_DIFFERENCE < Time.time)
        {
            if (Random.value < 0.5)
                createGift(Raspberry);
            else
                createGift(Kiwano);
        }

        // create sometimes a gift is possible to need
        if (m_lastGiftTimeStamp + GIFT_TIME_DIFFERENCE < Time.time)
        { 
            // create a gift with a probability 5%
            float randomValue = Random.value * 20 ;
            if (randomValue < 1)
            {
                float probabilityLife, probabilityKiwano, probabilityRasp;

                calcualteProbability(out probabilityLife, out probabilityKiwano, out probabilityRasp);

                if (randomValue < probabilityLife)
                    createGift(Life);
                else if (randomValue < probabilityKiwano)
                    createGift(Kiwano);
                else if (randomValue < probabilityRasp)
                    createGift(Raspberry);
            }

        }

        // save current life points/ -numbers
        m_lastPlayerValues.m_lifePoints = m_playerData.LifePoints;
        m_lastPlayerValues.m_lifeNumbers = m_playerData.LifeNumber;
    }

    /**
     * calcualte the probability to throw the power up
     */ 
    private void calcualteProbability(out float _probabilityLife, out float _probabilityKiwano, out float _probabilityRasp)
    {
        // life
        _probabilityLife = 0;
        if (m_playerData.LifePoints == 1)
            _probabilityLife = 1;
        else if (m_playerData.LifePoints < GameConfig.BILLY_LIFE_POINT)
            _probabilityLife = 0.5f;
        int kiw = 1, rasp = 1;

        // precalculations
        //kiwano
        if (!m_playerData.isPowerUpAvailable(PlayerData.PowerUpType.PUT_KIWANO) || m_playerData.getPowerUpStockSize(PlayerData.PowerUpType.PUT_KIWANO) >= MAXIMAL_POWER_UPS_NUMBER)
            kiw = 0;
        else if (m_playerData.getPowerUpStockSize(PlayerData.PowerUpType.PUT_KIWANO) < MAXIMAL_POWER_UPS_NUMBER)
            kiw += MAXIMAL_POWER_UPS_NUMBER - m_playerData.getPowerUpStockSize(PlayerData.PowerUpType.PUT_KIWANO);

        // raspberry
        if (!m_playerData.isPowerUpAvailable(PlayerData.PowerUpType.PUT_RASPBERRY) || m_playerData.getPowerUpStockSize(PlayerData.PowerUpType.PUT_RASPBERRY) >= MAXIMAL_POWER_UPS_NUMBER)
            rasp = 0;
        else if (m_playerData.getPowerUpStockSize(PlayerData.PowerUpType.PUT_RASPBERRY) < MAXIMAL_POWER_UPS_NUMBER)
            rasp += MAXIMAL_POWER_UPS_NUMBER - m_playerData.getPowerUpStockSize(PlayerData.PowerUpType.PUT_RASPBERRY);


        _probabilityKiwano = _probabilityLife;
        _probabilityRasp = _probabilityKiwano;

        // no more power up needed?
        if (rasp == 0 && kiw == 0)
            return;

        _probabilityKiwano += (1 - _probabilityLife) * kiw / (rasp + kiw);
        _probabilityRasp = 1;
    }

    /**
     * create a gift for the player
     */
    private void createGift(GameObject _prefab)
    {
        // create the instance
        GameObject instance = (GameObject) Instantiate(_prefab);
        instance.transform.position = transform.position;
        instance.transform.parent = transform.parent;
        instance.transform.localScale = _prefab.transform.localScale;

        // let move to the player
        PowerUps p = instance.GetComponent<PowerUps>();
        if (p != null)
            p.m_moveToPlayer = true;
        else
            Debug.LogError(_prefab + " is not a Power Up!");

        // save the last gift time
        m_lastGiftTimeStamp = Time.time;

        // speak something
        speak(ANTONIO_GIFT_SENTENCES[Random.Range(0, ANTONIO_GIFT_SENTENCES.Length)]);
    }

    /**
     * let Antonio speak
     */
    public void speak(string _text)
    {
        m_speakingQueue.Enqueue(_text);
    }

    /**
     * update the speaking function
     */
    private void updateSpeaking()
    {
        // nothing to speak or cannot speak?
        if((m_currentSpokenText == null && m_speakingQueue.Count == 0) || m_textDisplayScript == null)
            return;
        
        // update the position from the current 
        if (m_lastSpeakingTimeStamp + MAXIMAL_SPEAKING_TIME > Time.time)
        {
            if(m_mainCamera != null && m_currentSpokenText != null)
                m_currentSpokenText.position = m_mainCamera.WorldToScreenPoint(this.transform.position + new Vector3(0, m_realAntonioHeight, 0));
            return;
        }

        // destroy the current spoken text if necessary?
        if (m_currentSpokenText != null)
        {
            Destroy(m_currentSpokenText.gameObject);
            m_currentSpokenText = null;
        }

        // must speak something?
        if (m_speakingQueue.Count != 0)
        {
            m_currentSpokenText = m_textDisplayScript.createNewSpokenText(m_speakingQueue.Dequeue(), this.transform.position);
            m_lastSpeakingTimeStamp = Time.time;
        }

    }

}
