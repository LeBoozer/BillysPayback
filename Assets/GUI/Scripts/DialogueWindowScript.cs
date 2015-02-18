using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;

public class DialogueWindowScript : MonoBehaviour {

	//prefabs needed for the dialog window
	public Transform m_dialogWindowPrefab;
	public Transform m_dialogTextPrefab;
	public Transform m_spokenTextPrefab;

	//the position on which the dialog window is placed
	public Vector3 m_windowPosition;
	public Vector3 m_firstTextPosition;

	//the preferred length of a spoken text display
	public int m_spokenTextLength;

	//Problem: the dialog gui in the editor is x times bigger than the level; becaue we don't know the real ratio (we have no
	//object to compare) we have to code it hard for now
	public float m_GUIScaleOffset;

	//currently shown answer
	private Transform m_spokenText;

	public Transform SpokenText {
		get { return m_spokenText; }
	}

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

    void Awake()
    {
        m_dialogWindow = null;
        m_windowIsOpen = false;

        //sets the current game object as the parent for coming dialog objects
        m_parentGameObject = this.transform;

        // searches the current hud
        m_currentHUD = GameObject.Find("HUD");
    }

	// Use this for initialization
	void Start () 
    {

		/*openDialogWindow ();

		addAnswers (testTexts);

		GameObject billy = GameObject.Find ("Billy");

		showAnswer(answers[3].text, billy);

		dropSpokenAnswer ();*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//deletes currently shown answer
	public bool dropSpokenAnswer() {
		if (m_spokenText != null) {
			Destroy(m_spokenText.gameObject);

			return true;
		}

		Debug.Log ("No answer is currently shown!");

		return false;
	}

	//shows the given answer text above the head of the give game object --> returns the shown answer text
	public void showAnswer(string text, GameObject person) {
		//gets the position of the given person
		Vector3 personPosition = person.transform.position;

		//gets the height of the given person
		CapsuleCollider personCollider = person.GetComponent<CapsuleCollider> ();
		float personHeight = personCollider.center.y + personCollider.height;

		//calculates the corresponding height of the spoken text
		Vector3 textPosition = new Vector3 (personPosition.x, personPosition.y + personHeight, personPosition.z);

		//Problem: the dialog gui in the editor is x times bigger than the level; becaue we don't know the real ratio (we have no
		//object to compare) we have to code it hard for now
		textPosition = textPosition * m_GUIScaleOffset;

		//m_spokenText = createNewSpokenText (text, textPosition);

		int charCount = 0;						//counts the number of chars walked through
		int currentStringBegin = 0;				//position in given text from which on the current spoken text is shown
		Transform currentSpokenText = null;		//currently displayed spoken text
		//SimpleTimer timer = new SimpleTimer ();

		//divides the given text in multiple text parts which are shown seperately to save space on the screen
		for (int i = 0; i < text.Length; i++) {
			charCount++;

			//checks if the current substring is long enough to be splitted and/or shown
			if (charCount >= m_spokenTextLength || i == text.Length - 1) {
				//makes sure that single words are not splitted
				if (text[i] == ' ' || i == text.Length - 1) {
					//destroys old spoken text
					if (currentSpokenText != null) {
						Destroy(currentSpokenText.gameObject);
					}

					//creates new spoken text
					currentSpokenText = createNewSpokenText(text.Substring(currentStringBegin, i-currentStringBegin + 1), textPosition);

					//resets counter
					charCount = 0;

					//updates substring begin
					if ( i < text.Length -1) {
						currentStringBegin = i + 1;
					}

					//timer.restart(10 * 1000);

					/*while (!timer.Done)
					{
						Debug.Log("Warten");
					}*/
				}
			}
		}
	}

	public Transform createNewSpokenText(string text, Vector3 position) {
		//creates text object
		Transform spokenText = Instantiate (m_spokenTextPrefab, position, Quaternion.identity) as Transform;
		
		//assigns the correct parent
		spokenText.SetParent (this.gameObject.transform, false);

		//assigns given text to text object
		Text spokenTextContent = spokenText.gameObject.GetComponent<Text> ();
		spokenTextContent.text = text;
		
		return spokenText;
	}

	//removes all answers from the current dialog window --> returns if successful or not
	public bool removeAnswers() {
        if (answers == null || answers.Length <= 0)
        {
			return false;
		} 
        else 
        {
			//walks through the answers in the dialog window and destroys them
			for (int i = 0; m_dialogWindow != null && i < m_dialogWindow.childCount; i++) {
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
