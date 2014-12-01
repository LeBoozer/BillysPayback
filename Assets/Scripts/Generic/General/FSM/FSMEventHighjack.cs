/*
 * Project:	Billy's Payback
 * File:	FSMEventHighjack.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Helper class for highjacking an object declared as target in transitions
 */
public class FSMEventHighjack : MonoBehaviour
{
	// List of all supported delegate functions
	public delegate void Delegate_OnTriggerEnter(Collider _other);
	public delegate void Delegate_OnTriggerStay(Collider _other);
	public delegate void Delegate_OnTriggerLeave(Collider _other);
	
	// List of all supported events
	public event Delegate_OnTriggerEnter 	FSMOnTriggerEnter 	= delegate{};
	public event Delegate_OnTriggerEnter 	FSMOnTriggerStay	= delegate{};
	public event Delegate_OnTriggerEnter 	FSMOnTriggerLeave	= delegate{};
	
	// Override: MonoBehaviour::OnTriggerEnter
	private void OnTriggerEnter(Collider _other)
	{
		// Notify listener
		FSMOnTriggerEnter(_other);
	}
	
	// Override: MonoBehaviour::OnTriggerStay
	private void OnTriggerStay(Collider _other)
	{
		// Notify listener
		FSMOnTriggerStay(_other);
	}	
	
	// Override: MonoBehaviour::OnTriggerLeave
	private void OnTriggerLeave(Collider _other)
	{
		// Notify listener
		FSMOnTriggerLeave(_other);
	}	
}
