/*
 * Project:	Billy's Payback
 * File:	DeActivateBirds.cs
 * Authors:	Raik Dankworth
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/**
 * Describe a Trigger which acitvate and disable birds
 */
public class DeActivateBirds : MonoBehaviour
{
    public List<GameObject>         m_nextActiveEnemyGroups;
    public List<GameObject>         m_nextInactiveEnemyGroups;


    /**
     * triggers the process of activate/disable
     */
    void OnTriggerEnter(Collider _other)
    { 
        // is the player?
        if(_other.gameObject.tag != Tags.TAG_PLAYER)
            return;

        // disable the enemies
        setAllowToMoveValue(m_nextInactiveEnemyGroups, false);

        // activate the enemies
        setAllowToMoveValue(m_nextActiveEnemyGroups, true);

        this.enabled = false;
    }

    /**
     * set all m_allowToMove values for all enemies in the list to _value
     */
    void setAllowToMoveValue(List<GameObject> _list, bool _value)
    {
        // no list?
        if(_list == null)
            return;
        
        // init needed variables
        Queue<GameObject> queue = new Queue<GameObject>();
        GameObject currentObj;

        // for all gameobjects in the list
        foreach (GameObject _obj in _list)
        {
            // drop current game object in the queue
            queue.Enqueue(_obj);

            // until the queue is empty
            while (queue.Count != 0)
            {
                // fetch next game object
                currentObj = queue.Dequeue();

                // seek an enemy script
                Enemy en = currentObj.GetComponent<Enemy>();

                // is the current game object an enemy?
                if (en != null)
                    en.m_allowToMove = _value;

                // else drop all child to queue
                else
                    for (int i = currentObj.transform.childCount; --i >= 0; )
                        queue.Enqueue(currentObj.transform.GetChild(i).gameObject);
            }
        }
    }
}
