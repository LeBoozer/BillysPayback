using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DialogTextScript : MonoBehaviour {

	public delegate void AnswerClickedEventHandler(Text answer);
	public static event AnswerClickedEventHandler AnswerClicked;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void onAnswerClicked(BaseEventData data) {
		Debug.Log ("Event fired!");

		//calls the answerClicked Event if a corresponding function was assigned
		if (AnswerClicked != null) {
			AnswerClicked(this.gameObject.GetComponent<Text>());
		}
	}
}
