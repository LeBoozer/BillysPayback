using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DialogTextScript : MonoBehaviour {

	public delegate void AnswerClickedEventHandler(Text answer);
	public static event AnswerClickedEventHandler AnswerClicked;

	//color to hightlight currently hovered answer
	public Color m_hoverTextColor;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void onAnswerClicked(BaseEventData data) {
		Debug.Log ("Clicked text: " + this.gameObject.GetComponent<Text>().text);

		//calls the answerClicked Event if a corresponding function was assigned
		if (AnswerClicked != null) {
			AnswerClicked(this.gameObject.GetComponent<Text>());
		}
	}

	public void onAnswerEntered(BaseEventData data) {
		//highlights the answer which is currently under the mouse pointer
		this.gameObject.GetComponent<Text> ().color = m_hoverTextColor;
	}

	public void onAnswerExit(BaseEventData data) {
		//unhighlights the answer after the mouse pointes has left
		this.gameObject.GetComponent<Text> ().color = Color.white;
	}
}
