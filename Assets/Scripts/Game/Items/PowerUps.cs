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
public class PowerUps : MonoBehaviour {

    private PlayerData              m_playerData    = null;
    public PlayerData.PowerUpType   m_type          = PlayerData.PowerUpType.PUT_NONE;
    public bool                     m_isDiamond     = false;

    // Override: MonoBehaviour::Awake()
    void Awake()
    {
        // Randomly rotate
        transform.Rotate(Vector3.up * Random.value * 100 * GameConfig.DIAMONG_ROTATION_SPEED);
    }

    // Override: MonoBehaviour::Start()
    void Start()
    {
        // Get player data
        m_playerData = Game.Instance.PlayerData;
    }

    // Override: MonoBehaviour::Update()
    void Update()
    {
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
            else
                m_playerData.increaseStockSizeByValue(m_type, 1);
            GameObject.Destroy(gameObject);
        }
    }
}
