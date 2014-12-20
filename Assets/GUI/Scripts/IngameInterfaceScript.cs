using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IngameInterfaceScript : MonoBehaviour {

	private PlayerData m_playerData;

	//count display values
	private int m_heartCount;
	public Text m_lifesCount;
	public Text m_diamondCount;
	public Text m_kiwanoCount;
	public Text m_raspberryCount;

	//the heart prefab and the wished parent for this object
	public Transform m_heartPrefab;
	public Transform m_parent;

	//the position of the first heart from which all the other heart positions are derived (position which is saved in the prefab)
	public Vector3 m_firstHeartPos;

	//maximum number of life points
	private int m_maximumHearts;

	//array which holds all the heart objects
	private GameObject[] hearts;

	// Use this for initialization
	void Start () {
		//gets the full number of hearts at the beginning of the game
		m_maximumHearts = GameConfig.BILLY_LIFE_POINT;

		//initializes the array of hearts
		hearts = new GameObject [m_maximumHearts];

		//creates the first life point/heart
		Transform lastHeart = createHeart(m_firstHeartPos);

		//adds lately created heart to the list of hearts
		hearts[0] = lastHeart.gameObject;
		
		Vector3 heartPosition;
		float heartScaleX;
		float heartWidth;
		float scaleOffset = 0.05f; //offset to keep some distance between the hearts

		//initializes the rest of the life points/hearts
		for (int i = 1; i < m_maximumHearts; i++) {
			//calculates the position of the new heart
			heartScaleX = ((RectTransform)lastHeart).localScale.x;
			heartWidth = ((RectTransform)lastHeart).rect.width * (heartScaleX + scaleOffset);
			heartPosition = m_firstHeartPos + i * new Vector3(heartWidth, 0, 0); //can't take the real position of the lastHeart --> for some reason not right

			lastHeart = createHeart(heartPosition);

			//adds lately created heart to the list of hearts
			hearts[i] = lastHeart.gameObject;
		}
	}
	
	// Update is called once per frame
	void Update () {

		//gets the current player data
		m_playerData = Game.Instance.PlayerData;

		//updates the interface values
		m_lifesCount.text = "x " + m_playerData.LifeNumber.ToString();
		m_diamondCount.text = m_playerData.CollectedDiamonds.ToString() + " x";
		m_kiwanoCount.text = m_playerData.getPowerUpStockSize (PlayerData.PowerUpType.PUT_KIWANO).ToString() + " x";
		m_raspberryCount.text = m_playerData.getPowerUpStockSize (PlayerData.PowerUpType.PUT_RASPBERRY).ToString() + " x";

		updateHeartCount ();
	}

	//updates the number of life points/hearts
	void updateHeartCount() {
		//gets number of hearts
		int heartNum = m_playerData.LifePoints;

		//tests if number of hearts is within the valid limit
		if (heartNum > m_maximumHearts) {
			heartNum = m_maximumHearts;

			//corrects the life points
			m_playerData.LifePoints = m_maximumHearts;
		}

		//enables/disables the hearts to display the right number of hearts
		for (int i = 1; i <= m_maximumHearts; i++) {
			//checks if current number of hearts is reached
			if (i <= heartNum) {
				hearts[i-1].SetActive(true);
			} else {
				hearts[i-1].SetActive(false);
			}
		}
	}

	//creates new heart prefab instance at the given position
	Transform createHeart(Vector3 position) {
		//creates a new heart object
		Transform newHeart = Instantiate (m_heartPrefab, position, Quaternion.identity) as Transform;
		
		//set the parent of the new heart
		newHeart.SetParent (m_parent, false);

		return newHeart;
	}
}
