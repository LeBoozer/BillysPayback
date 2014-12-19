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

	private int m_maximumHearts;

	//heart images
	public Image[] hearts;

	// Use this for initialization
	void Start () {
		m_maximumHearts = hearts.Length;
	}
	
	// Update is called once per frame
	void Update () {

		//gets the current player data
		m_playerData = Game.Instance.PlayerData;

		//updates the interface values
		m_lifesCount.text = "x " + m_playerData.LifePoints.ToString();
		m_diamondCount.text = m_playerData.CollectedDiamonds.ToString() + " x";
		m_kiwanoCount.text = m_playerData.getPowerUpStockSize (PlayerData.PowerUpType.PUT_KIWANO).ToString() + " x";
		m_raspberryCount.text = m_playerData.getPowerUpStockSize (PlayerData.PowerUpType.PUT_RASPBERRY).ToString() + " x";

		updateHeartCount ();
	}

	void updateHeartCount() {
		//gets number of hearts
		int heartNum = m_playerData.LifePoints;

		//tests if number of hearts is within the valid limit
		if (heartNum > m_maximumHearts) {
			heartNum = m_maximumHearts;
		}

		//enables/disables the hearts to display the right number of hearts
		for (int i = 1; i <= m_maximumHearts; i++) {
			//checks if current number of hearts is reached
			if (i <= heartNum) {
				hearts[i-1].enabled = true;
			} else {
				hearts[i-1].enabled = false;
			}
		}
	}
}
