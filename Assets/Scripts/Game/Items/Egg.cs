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
	private MeshCollider 		m_collider;
	public  EggType 			m_eggType = EggType.NOTHING;
	private GameObject			m_nextPowerUp;



	// Use this for initialization
	void Start () 
	{
		// get needed components
		m_collider 			= GetComponent<MeshCollider> ();

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
		/*/ Debug
		if ( m_nextPowerUp1 == null && m_eggType != EggType.NOTHING)
			Debug.Log("Ladefehler");
		else if(m_eggType != EggType.NOTHING)
			Debug.Log(m_nextPowerUp1);
		*/
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
		GameObject player = _obj as GameObject;
		CharacterController character = player.GetComponent<CharacterController> ();
		
		// useless message?
		if (player == null)
			return;
		
		// player under the box?
		if(player.transform.position.y + character.height < this.transform.position.y)
		{
			if(m_nextPowerUp != null)
				Instantiate(m_nextPowerUp, this.transform.position, this.transform.rotation);

			// TO DO: create partical effekt
			Destroy(this.gameObject);
		}
	}
}
