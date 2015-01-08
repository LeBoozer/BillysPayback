using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;

public class DialogueWindowScript : MonoBehaviour {

	//prefabs needed for the dialog window
	public Transform m_dialogWindowPrefab;
	public Transform m_dialogTextPrefab;

	//the position on which the dialog window is placed
	public Vector3 m_windowPosition;
	public Vector3 m_firstTextPosition;

	//dialog window already open?
	private bool m_windowIsOpen;

	public bool WindowIsOpen {
		get { return m_windowIsOpen; }
	}

	private Transform m_parentGameObject; 	//usually the current game object (the object on which this script lies)
	private Transform m_dialogWindow;		//the dialog window if it's open (else it's null)
	private GameObject m_currentHUD;		//the HUD which displays number of lives, diamonds and so on

	//list of currently possible answers
	private Text[] answers;

	public Text[] Answers {
		get { return answers; }
	}

	// Use this for initialization
	void Start () {
		m_dialogWindow = null;
		m_windowIsOpen = false;

		//sets the current game object as the parent for coming dialog objects
		m_parentGameObject = this.transform;

		//searches the current hud
		m_currentHUD = GameObject.Find("HUD");

		string[] testTexts = {"Wozu braucht ihr diese Pflanzen denn?", 
			"Können wir euch irgendwie helfen?", 
			"Wir könnten versuchen euch die Kurbel wiederzubringen, wenn ihr uns anschließen auf unserem Weg in Black Sparrows Nest helft. Seid ihr dabei?", 
			"Da müsst ihr aber ganz schön sauer auf Black Sparrow und seine Gefolgschaft sein was? Wir sind gerade auf dem Weg zu ihnen, um diesen Federviechern eine Lektion zu erteilen. Wärt ihr dabei?", 
			"Warum holt ihr sie euch nicht einfach zurück?",
			"Klingt … interessant … wir gehen dann mal."};

		openDialogWindow ();

		addAnswers (testTexts);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//removes all answers from the current dialog window --> returns if successful or not
	public bool removeAnswers() {
		if (answers.Length <= 0) {
			Debug.Log("No answers exist which could be removed!");

			return false;
		} else {
			//walks through the answers in the dialog window and destroys them
			for (int i = 0; i < m_dialogWindow.childCount; i++) {
				Destroy(m_dialogWindow.GetChild(i).gameObject);
			}

			//deletes saved answer texts
			answers = null;

			return true;
		}
	}

	//adds new answers with the given texts to the dialog
	public void addAnswers (string[] texts) {

		//variables needed for the calculation of the position of each text object
		Vector3 currentPos;
		float textHeight;
		float textScaleY;

		//new answer object and corresponding text object
		Transform newAnswer = null;

		//initializes the answer array
		answers = new Text[texts.Length];

		//needed to get the scale and height later (can't get this from prefabs)
		Transform firstTextObject = null;

		//float overallHeightText = 0;

		//creates the new answers and adds them to the dialog window
		for (int i = 0; i < texts.Length; i++) {
			if (i <= 0) {
				//creates first answer
				newAnswer = createNewAnswer (m_firstTextPosition, texts [i], i);
				
				//adds answer to answer array
				answers [i] = newAnswer.gameObject.GetComponent<Text> ();
			} else {
				//gets scale and height of the dialog text prefab
				textScaleY = ((RectTransform)newAnswer).localScale.y;
				textHeight = ((RectTransform)newAnswer).rect.height * textScaleY;

				Debug.Log ("Text height: " + textHeight); 
				
				//calculates the position of the current answer
				currentPos = m_firstTextPosition - i * (new Vector3(0, textHeight, 0));

				//creates new answer
				newAnswer = createNewAnswer(currentPos, texts[i], i);

				//adds answer to answer array
				answers[i] = newAnswer.gameObject.GetComponent<Text>();

				//overallHeightText += textHeight * 3/2;
			}
		}

		/*if (overallHeightText > ((RectTransform)m_dialogWindow).rect.height) {
			((RectTransform)m_dialogWindow).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, overallHeightText + 5);
		} --> tries to make the dialog window fit the answer texts but rectangle height seems to be always wrong for bigger texts*/
	}

	//creates a new answer in the dialogue window
	private Transform createNewAnswer(Vector3 position, string text, int answerID) {
		//creates the new answerText from the prefab
		Transform newAnswer = Instantiate (m_dialogTextPrefab, position, Quaternion.identity) as Transform;

		//sets the correct parent of the answer text
		newAnswer.SetParent (m_dialogWindow, false);

		//makes the text fit the dialog window
		RectTransform newAnswerRect = (RectTransform)newAnswer;
		newAnswerRect.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, ((RectTransform)m_dialogWindow).rect.height / answers.Length - 5);

		//sets the answer text
		Text answerText = newAnswer.gameObject.GetComponent<Text> ();
		answerText.text = text;

		//sets the name of the text object
		answerText.name = "" + answerID;

		return newAnswer;
	}

	//opens a new dialog --> returns if opening was successfull or not
	public bool openDialogWindow() {
		if (m_windowIsOpen) {
			Debug.Log("Dialog window already open!");
			
			return false;
		} else {
			//marks the dialog window as open
			m_windowIsOpen = true;
			
			//deactivates the HUD
			if (m_currentHUD.activeSelf) {
				m_currentHUD.SetActive(false);
			}
			
			
			//creates new dialog window
			m_dialogWindow = Instantiate(m_dialogWindowPrefab, m_windowPosition, Quaternion.identity) as Transform;
			
			//sets the correct parent of the dialog window
			m_dialogWindow.SetParent(m_parentGameObject, false);
			
			return true;
		}
	}
	
	//closes the current dialog --> returns if closing was successfull or not
	public bool closeDialogWindow() {
		if (!m_windowIsOpen) {
			Debug.Log("No dialog window open!");
			
			return false;
		} else {
			//destroys the current dialog window
			Destroy(m_dialogWindow.gameObject);
			
			m_dialogWindow = null;
			
			//marks the dialog window as closed
			m_windowIsOpen = false;
			
			//reactivates the HUD
			if (!m_currentHUD.activeSelf) {
				m_currentHUD.SetActive(true);
			}
			
			return true;
		}
	}
}
