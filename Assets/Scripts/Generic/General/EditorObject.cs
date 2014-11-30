/*
 * Project:	Billy's Payback
 * File:	EditorObject.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Represents the blueprint for a editor-object
 */
public interface EditorObject
{
	// Will be called as soon as the editor-object is being initialized
	void onInit ();
	
	// Will be called as soon as the editor-object is being terminated
	void onExit ();
	
	// Will be called as soon as the editor-object is being updated
	void onUpdate (float _deltaTime);
	
	// Will be called as soon as the editor-object is being updated at a fixed rate
	void onUpdateFixed (float _deltaTime);
}