/*
 * Project:	Billy's Payback
 * File:	TakeScreenshot.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Handles console commands
 */
public class TakeScreenshot : MonoBehaviour
{
    // The ID counter
    private int m_nameIDCounter = 0;
	
	// Update is called once per frame
	void Update ()
    {
        // F10 pressed?
        if (Input.GetKeyDown(KeyCode.F10) == true)
        {
            Application.CaptureScreenshot("screenshot_" + m_nameIDCounter + ".png");
            ++m_nameIDCounter;
        }
	}
}
