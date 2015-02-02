using UnityEngine;
using System.Collections;

public class Antonio : MonoBehaviour
{
    #region 

    enum MovementDirection
    {
        eNothing    = 0,
        eRight      = (1 << 1),
        eLeft       = (1 << 2)
    }

    // variables
    public bool                         m_chase = true;
    private GameObject                  m_way;
    private float                       m_realAntonioHeight;
    private bool                        m_waiting;
    private int                         m_ignoreLayerMask;
    GameObject                          m_sphere = null;

    // movement
    private float                       m_velocityX;
    private Vector3                     m_oldPosition;
    private float                       m_velocityY;
    private float                       m_brakeFactor           = 1;
    private float                       m_accelerationFactor    = 1;


    // chase variables
    private int                         m_iteration;
    private int                         m_samplingRate          = 10;
    private float                       m_targetDistance;
    private Vector3                     m_nextWayPoint;
    
    // external objects
    private GameObject                  m_player;
	private PlayerData		            m_playerData;
    private CharacterController         m_controller;
    private Vector3                     m_worldScale;

    // constants
    private const string                GAME_OBJECT_WAY_NAME    = "Way";
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
        if (m_chase)
            chaseUpdate();
        else 
            runAheadUpdate();
	}

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
                            || (m_nextWayPoint - transform.position).magnitude < height * height        // way point achieved?
                            || !usefulWayPoint());                                                       // generally now an useless way point?
        Vector3 lastWayPoint = m_nextWayPoint;
        // check the usefulness of the current way point
        if (newWayPoint)
        {
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
        }

        // move
        //Debug.Log(nextMovement + "\t" + jumpHeight);
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
        Vector3 playerPos = m_player.transform.position;
        Vector3 ownPos = transform.position - new Vector3(0, m_realAntonioHeight / 2, 0);
        return (Mathf.Min(ownPos.x, playerPos.x) <= m_nextWayPoint.x && Mathf.Max(ownPos.x, playerPos.x) >= m_nextWayPoint.x)               // for x-value?
                        && (Mathf.Min(ownPos.y, playerPos.y) <= m_nextWayPoint.y && Mathf.Max(ownPos.y, playerPos.y) >= m_nextWayPoint.y);  // for y -value?
    }

}
