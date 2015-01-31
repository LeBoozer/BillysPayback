using UnityEngine;
using System.Collections;

public class Antonio : MonoBehaviour
{
    #region 
    // public variables
    public bool             m_chase = true;
    private GameObject      m_way;

    // chase variables
    private int             m_iteration;
    private int             m_samplingRate          = 10;
    private float           m_targetDistance;
    private Vector3         m_nextWayPoint;


    
    // external objects
    private GameObject      m_player;
	private PlayerData		m_playerData;

    // constants
    private const string    GAME_OBJECT_WAY_NAME    = "Way";
    #endregion

    // Use this for initialization
	void Start () 
    {
        // seek player
        m_player = GameObject.FindGameObjectWithTag(Tags.TAG_PLAYER);
        if(m_player == null)
            Debug.LogError("Antonio: Player not found!");

        // get player data
        m_playerData = Game.Instance.PlayerData;
        if (m_chase)
        { 
        }
        m_iteration = 0;
        m_nextWayPoint = Vector3.zero;
        m_way = getWay();

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
                return;
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
        if (m_nextWayPoint == Vector3.zero)
            getNextWayPoint();

        // TODO: moveToCurrentWayPoint
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
        m_targetDistance = GameConfig.ANTONIO_DISTANCE_MEAN_VALUE + Mathf.Pow(2 * Random.value - 1, 3) * GameConfig.ANTONIO_DISTANCE_VARIANCE;
    }


    /**
     * return the game object with the way
     * if the way doesnt exists, it is generated
     */
    private GameObject getWay()
    {
        // exists a way?
        Transform transOfWay = transform.FindChild(GAME_OBJECT_WAY_NAME);
        if (transOfWay != null)
            return transOfWay.gameObject;

        // create the way as empty game object
        GameObject gameObjectOfWay = new GameObject(GAME_OBJECT_WAY_NAME);
        gameObjectOfWay.transform.parent = transform;
        return gameObjectOfWay;
    }

    /**
     * create a new way point for antonio 
     */
    private void createAWayPoint()
    {
        // create a new way point for antonio
        GameObject newWayPoint = new GameObject();
        newWayPoint.transform.parent = m_way.transform;
        newWayPoint.transform.position = m_player.transform.position;
    }

    /**
     * pull the next way point for antonio
     */
    private void getNextWayPoint()
    {
        // local variable 
        int index = 0;
        int ignoreLayerMask;
        RaycastHit hitInfo;
        Vector3 playerPos = m_player.transform.position;
        Vector3 ownPos = transform.position;

        // until a useful way point found
        while(index < m_way.transform.childCount)
        {
            // set the next possible way point
            m_nextWayPoint = m_way.transform.GetChild(index).transform.position;


            // ignore everything except the enviroment
            ignoreLayerMask = ~(LayerMask.NameToLayer(Layer.LAYER_COLLECTABLE) 
                                    | LayerMask.NameToLayer(Layer.LAYER_ENEMY) 
                                    | LayerMask.NameToLayer(Layer.LAYER_PLAYER)
                                    | LayerMask.NameToLayer(Layer.LAYER_PROJECTILE_ENEMY)
                                    | LayerMask.NameToLayer(Layer.LAYER_PROJECTILE_PLAYER));

            // find a gameobject?
            if (Physics.Raycast(m_nextWayPoint + Vector3.up, Vector3.down, out hitInfo, float.MaxValue, ignoreLayerMask))
            {
                // set ground as way point
                m_nextWayPoint = hitInfo.point;

                // run ahead?
                if (!m_chase)
                    break;

                // chase 
                // between player and antonio in at least one component? --> usefull way point
                if ((Mathf.Min(ownPos.x, playerPos.x) <= m_nextWayPoint.x && Mathf.Max(ownPos.x, playerPos.x) >= m_nextWayPoint.x)          // for x-value?
                        || (Mathf.Min(ownPos.y, playerPos.y) <= m_nextWayPoint.y && Mathf.Max(ownPos.y, playerPos.y) >= m_nextWayPoint.y))  // for y -value?
                    break;
            }

            // useless way point
            ++index;
        }

        // remove all now useless way points from the way
        do
            GameObject.Destroy(m_way.transform.GetChild(index).gameObject);
        while (--index >= 0);
    }

}
