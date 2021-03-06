﻿using UnityEngine;
using System.Collections;

public class Egg : MonoBehaviour {

	public enum EggType
	{
		KIWANO,
		RASPBERRY,
		DIAMOND,
		NOTHING
	}

	public  EggType 			m_eggType = EggType.NOTHING;
	private GameObject			m_nextPowerUp;

    // Helper class for on destroy/collect effects
    private PowerUpsOnDestroy m_onDestroyHelper = null;

	// Use this for initialization
	void Start () 
	{
		// load rigidbody
		Object m_nextPowerUp1;
		switch (m_eggType) 
		{
		case EggType.DIAMOND:
                m_nextPowerUp1 = Resources.Load("Items/Diamond");
			break;
		case EggType.KIWANO:
			m_nextPowerUp1 = Resources.Load("Items/KiwanoPowerUp");
			break;
		case EggType.RASPBERRY:
			m_nextPowerUp1 = Resources.Load("Items/RaspberryPowerUp");
			break;
		case EggType.NOTHING:
			m_nextPowerUp1 = null;
			break;
		default:
			m_nextPowerUp1 = null;
			break;
		}
		m_nextPowerUp = m_nextPowerUp1 as GameObject;

        // Try to get on destroy helper
        m_onDestroyHelper = GetComponentInChildren<PowerUpsOnDestroy>();
	}

    /**
     * 
     */
    public void OnCharacterControllerHit(object _hitInfo)
    {
        // Local variables
        ControllerColliderHit hit = _hitInfo as ControllerColliderHit;

        // Useless message?
        if (hit == null)
            return;

        // Hit from beneath?
        if (Vector3.Dot(hit.normal, Vector3.down) >= GameConfig.EGG_BENEATH_FACTOR)
        {
            // Create power up
            if (m_nextPowerUp != null)
            {
                GameObject obj = Instantiate(m_nextPowerUp, this.transform.position, this.transform.rotation) as GameObject;
                obj.transform.parent = transform.parent;
                obj.transform.localScale = m_nextPowerUp.transform.localScale;
            }

            // On destroy/collect helper available?
            if (m_onDestroyHelper != null)
            {
                // Change parent
                m_onDestroyHelper.transform.parent = transform.parent;

                // Play
                m_onDestroyHelper.onAction();
            }

            // Destroy
            GameObject.Destroy(gameObject);
        }
    }
}
