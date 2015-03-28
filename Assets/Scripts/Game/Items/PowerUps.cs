/*
 * Project:	Billy's Payback
 * File:	PowerUps.cs
 * Authors:	Byron Worms
 * Editors:	Raik Dankworth
 */
using UnityEngine;
using System.Collections;

/*
 * ???
 */
public class PowerUps : MonoBehaviour
{
    // The player data
    private PlayerData              m_playerData    = null;

    // The player instance
    private GameObject              m_player        = null;

    // True to skip the update
    private bool                    m_skipUpdate    = true;

    // Power-up type
    public PlayerData.PowerUpType   m_type          = PlayerData.PowerUpType.PUT_NONE;

    // True if this power-up is a diamond
    public bool                     m_isDiamond     = false;

    // set from extern
    // True if the power-up must fly to the player
    public bool                     m_moveToPlayer  = false;

    // Offset for flying to the player
    private Vector3                 m_offset        = Vector3.zero;

    // Helper class for on destroy/collect effects
    private PowerUpsOnDestroy       m_onDestroyHelper = null;

    // Override: MonoBehaviour::Awake()
    void Awake()
    {
        // dont rotate the heart
        if (m_type == PlayerData.PowerUpType.PUT_LIFE)
            return;

        // Randomly rotate
        transform.Rotate(Vector3.up * Random.value * 100 * GameConfig.DIAMONG_ROTATION_SPEED);

        // Try to get on destroy helper
        m_onDestroyHelper = GetComponentInChildren<PowerUpsOnDestroy>();
    }

    // Override: MonoBehaviour::Start()
    void Start()
    {
        // Get player data
        m_playerData = Game.Instance.PlayerData;

        // Get player
        m_player = GameObject.FindGameObjectWithTag(Tags.TAG_PLAYER);

        // Player dont find?
        if (m_player == null)
            return;

        // Get the character controller from the player
        CharacterController controller = m_player.GetComponent<CharacterController>();

        // Character controller find?
        if (controller == null)
            return;

        // Get the center of the controller
        m_offset = controller.center;

        // Get the scale of the player 
        Vector3 playerWorldScale = HelperFunctions.getWorldScale(m_player);

        // Calculate the scale of the player to the offset
        m_offset = new Vector3(m_offset.x * playerWorldScale.x,
                                m_offset.y * playerWorldScale.y,
                                m_offset.z * playerWorldScale.z);

        // Attach script
        MeshRendererOnVisible.attachScriptToRenderer(gameObject,
             (Camera _camera) =>
             {
                 // Player camera?
                 if (Camera.current.name.Equals(GameConfig.CAMERA_NAME_PLAYER) == true)
                     m_skipUpdate = false;
             });
    }

    // Override: MonoBehaviour::Update()
    void Update()
    {
        // like to move to player?
        if (m_moveToPlayer && m_player != null)
            this.transform.position += (m_player.transform.position + m_offset - this.transform.position).normalized * GameConfig.BILLY_MAX_SPEED * 1.5f * Time.deltaTime;

        // dont rotate the heart
        if (m_type == PlayerData.PowerUpType.PUT_LIFE)
            return;

        // Skip update?
        if (m_skipUpdate == true)
            return;

        // Rotate
        transform.Rotate(Vector3.up * Time.deltaTime * GameConfig.DIAMONG_ROTATION_SPEED);

        // Set flag
        m_skipUpdate = true;
    }

    // Override: MonoBehaviour::OnTriggerEnter()
    void OnTriggerEnter(Collider _other)
    {
        // Player?
        if (_other.gameObject.tag.Equals(Tags.TAG_PLAYER) == true)
        {
            // Diamond?
            if (m_isDiamond == true)
                ++m_playerData.CollectedDiamonds;

            // Heart?
            else if (m_type == PlayerData.PowerUpType.PUT_LIFE)
                ++m_playerData.m_lifePoints;
            else
                m_playerData.increaseStockSizeByValue(m_type, 1);

            // On destroy/collect helper available?
            if (m_onDestroyHelper != null)
            {
                // Change parent
                m_onDestroyHelper.transform.parent = transform.parent;

                // Play
                m_onDestroyHelper.onAction();
            }

            // Destory object
            GameObject.Destroy(gameObject);
        }
    }
}
