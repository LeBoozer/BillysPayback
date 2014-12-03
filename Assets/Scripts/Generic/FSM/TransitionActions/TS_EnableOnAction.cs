/*
 * Project:	Billy's Payback
 * File:	TS_EnableOnAction.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * TODO
 */
public class TS_EnableOnAction : FSMAction
{	
	// True to enable the objects on action
	public bool 				m_enableOnAction = true;

	// List of objects which should be disabled/enabled
	public List<GameObject> 	m_objectList = new List<GameObject>();
	
	// Override: FSMAction::OnAction()
	override public void onAction()
	{
		// Enable/disable
		foreach(GameObject obj in m_objectList)
		{
			if(obj != null)
				obj.SetActive(m_enableOnAction);
		}
	}
}
