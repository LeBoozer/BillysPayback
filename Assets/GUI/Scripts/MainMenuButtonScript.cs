using UnityEngine;
using System.Collections;

public class MainMenuButtonScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StartButtonClicked() {
		Debug.Log ("Startbutton was clicked!");
	}

	public void OptionsButtonClicked() {
		Debug.Log ("Optionsbutton was clicked!");
	}

	public void CloseButtonClicked() {
		Debug.Log ("Closebutton was clicked!");

		Application.Quit ();
	}
}
