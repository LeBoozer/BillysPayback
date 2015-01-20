/*
 * Project:	Billy's Payback
 * File:	OnAreaKeyEvent.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/*
 * Triggers an event if one target-object in within the bounds of the trigger object and the specified button is pressed
 */
public class OnAreaKeyEvent : MonoBehaviour
{
    // Delegate function for the notification event
    public delegate void Delegate_OnKeyEventTriggered();

    // The event for the notification
    public event Delegate_OnKeyEventTriggered OnKeyEventTriggered = delegate { };

    // True to auto-detect the player (billy) as target object
    public bool                     m_autoDetectPlayer = true;

    // True to re-use the trigger-area. If false, at least one target object has to enter the area again!
    public bool                     m_reuseArea = false;

    // Name of the button
    public string                   m_buttonName = "Interaction";

    // Help text displayed by the HUD
    public string                   m_helpText = "Press \"e\" to interact...";

    // List with all target objects
    public GameObject[]             m_targetObjects = null;

    // List with the information about the target objects
    private List<GameObject>        m_holders = new List<GameObject>();

    // Greater than 0 if the key event can be triggered
    private int                     m_canBeTriggered = 0;

    // The text for displaying the help text
    private Text                    m_simpleTextDisplay = null;
    
    // Override: MonoBehaviour::Awake
	void Awake ()
    {
        // Local variables
        Collider col = null;

	    // Check for colliders in game object
        col = this.GetComponent<Collider>();
        if(col == null || col.isTrigger == false)
        {
            Debug.LogError("Parent game-object has to contain at least one collider declared as trigger!");
            return;
        }
	}

    // Override: MonoBehaviour::Start
    void Start()
    {
        // Local variables
        GameObject obj = null;

        // Button defined?
        if (m_buttonName == null || m_buttonName.Length <= 0)
        {
            Debug.LogError("No valid button has been defined!");
            return;
        }

        // Try to get the game-object for displaying the help text
        obj = GameObject.Find("SimpleTextDisplay");
        if (obj == null)
            Debug.LogWarning("The game-object for displaying the help text couldn't be found!");
        else
        {
            // Get text component
            m_simpleTextDisplay = obj.GetComponent<Text>();
            if(m_simpleTextDisplay == null)
                Debug.LogWarning("The component for displaying the help text couldn't be found!");
        }

        // Auto detect player?
        if (m_autoDetectPlayer == true)
        {
            obj = GameObject.FindGameObjectWithTag(Tags.TAG_PLAYER);
            if (obj != null)
            {
                // Create holder
                m_holders.Add(obj);
            }
        }

        // Add defined target objects to list
        if (m_targetObjects != null && m_targetObjects.Length > 0)
        {
            // Loop through all entries
            foreach (GameObject g in m_targetObjects)
            {
                // Create holder
                m_holders.Add(g);
            }
        }
        if (m_holders.Count <= 0)
            Debug.LogWarning("At least one target object is required!");

        // Clean-up
        m_targetObjects = null;
    }

    // Override: MonoBehaviour::OnTriggerEnter
    void OnTriggerEnter(Collider _other)
    {
        // Enlisted?
        if (isObjectEnlisted(_other.gameObject) == true)
        {
            ++m_canBeTriggered;

            // Show help text
            if (m_simpleTextDisplay != null)
                m_simpleTextDisplay.text = m_helpText;
        }
    }

    // Override: MonoBehaviour::OnTriggerExit
    void OnTriggerExit(Collider _other)
    {
        // Enlisted?
        if (isObjectEnlisted(_other.gameObject) == true)
        {
            --m_canBeTriggered;
            if (m_canBeTriggered <= 0)
            {
                // Reset
                m_canBeTriggered = 0;

                // Hide help text
                if (m_simpleTextDisplay != null)
                    m_simpleTextDisplay.text = "";
            }
        }
    }

    // Override: MonoBehaviour::Update
    void Update()
    {
        // Local variables
        bool wasPressed = Input.GetButton(m_buttonName);

        // Trigger can be used?
        if (m_canBeTriggered <= 0)
            return;

        // Key event fired?
        if(wasPressed == true)
        {
            // Notify event
            OnKeyEventTriggered();

            // Re-use?
            if (m_reuseArea == false)
                m_canBeTriggered = 0;
        }
    }

    // Returns whether the requested game object is enlisted
    private bool isObjectEnlisted(GameObject _obj)
    {
        // Local variables

        // Loop through all defined taret objects
        foreach(GameObject g in m_holders)
        {
            // Compare
            if (g.Equals(_obj) == true)
                return true;
        }

        return false;
    }
}
