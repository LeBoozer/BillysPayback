/*
 * Project:	Billy's Payback
 * File:	PowerUps.cs
 * Authors:	Byron Worms
 * Editors:	-
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

    // Power-up type
    public PlayerData.PowerUpType   m_type          = PlayerData.PowerUpType.PUT_NONE;

    // True if this power-up is a diamond
    public bool                     m_isDiamond     = false;

    public bool                     m_moveToPlayer  = false;

    // The audio clip
    public AudioClip                m_audioClip     = null;

    // The volume scale
    public float                    m_volumeScale   = 1.0f;

    // Override: MonoBehaviour::Awake()
    void Awake()
    {
        // dont rotate the heart
        if (m_type == PlayerData.PowerUpType.PUT_LIFE)
            return;

        // Randomly rotate
        transform.Rotate(Vector3.up * Random.value * 100 * GameConfig.DIAMONG_ROTATION_SPEED);
    }

    // Override: MonoBehaviour::Start()
    void Start()
    {
        // Get player data
        m_playerData = Game.Instance.PlayerData;

        // Get player
        m_player = GameObject.FindGameObjectWithTag(Tags.TAG_PLAYER);
    }

    // Override: MonoBehaviour::Update()
    void Update()
    {
        // like to move to player?
        if (m_moveToPlayer && m_player != null)
            this.transform.position += (m_player.transform.position - this.transform.position) * Time.deltaTime;

        // dont rotate the heart
        if (m_type == PlayerData.PowerUpType.PUT_LIFE)
            return;

        // Rotate
        transform.Rotate(Vector3.up * Time.deltaTime * GameConfig.DIAMONG_ROTATION_SPEED);
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

            // Destory object
            GameObject.Destroy(gameObject);

            // Play sound?
            if (m_audioClip != null)
                AudioSource.PlayClipAtPoint(m_audioClip, transform.position, m_volumeScale);
        }
    }
}
