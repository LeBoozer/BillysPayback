/*
 * Project:	Billy's Payback
 * File:	LetAntonioMove.cs
 * Authors:	Raik Dankworth
 */
using UnityEngine;
using System.Collections;

/**
 * Special Trigger for Level 3.2
 * Let Antonio move about the brigde
 */
public class LetAntonioMove : MonoBehaviour {

    public GameObject m_antonioWay;
    public GameObject m_antonioNewWay;

    void OnTriggerEnter(Collider _other)
    {
        // ignore all gameobject except the player
        if (_other.tag != Tags.TAG_PLAYER)
            return;

        // put on all waypoints
        for (int i = 0; i < m_antonioNewWay.transform.childCount; ++i)
            m_antonioNewWay.transform.GetChild(i).parent = m_antonioWay.transform;
    }
}
