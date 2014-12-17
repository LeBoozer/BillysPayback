using UnityEngine;
using System.Collections;

public class Egg : MonoBehaviour {

	public enum EggType
	{
		KIWANO,
		RASPBERRY,
		DIAMOND,
		NOTHING
	}
	private CapsuleCollider 	m_capsule;
	public  EggType 			m_eggType = EggType.NOTHING;
	private GameObject			m_nextPowerUp;



	// Use this for initialization
	void Start () 
	{
		// get needed components
		m_capsule 		= GetComponent<CapsuleCollider> ();

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

		if ( m_nextPowerUp1 == null)
			Debug.Log("Ladefehler");
		else 
			Debug.Log(m_nextPowerUp1);
		m_nextPowerUp = m_nextPowerUp1 as GameObject;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	
	/**
	 * processed the collision with the player
	 * must called with SendMessage in the Player-Class
	 */
	public void PlayerCollision (Object _obj)
	{
		// Transform from player
		Transform player = _obj as Transform;
		
		// useless message?
		if (player == null)
			return;
		
		// player under the box?
		if(player.position.y < this.transform.position.y - m_capsule.height / 2)
		{
			if(m_nextPowerUp != null)
				Instantiate(m_nextPowerUp, this.transform.position, this.transform.rotation);

			// TO DO: create partical effekt
			Destroy(this.gameObject);
		}
	}
}
