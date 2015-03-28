/*
 * Project:	Billy's Payback
 * File:	BridgeScript.cs
 * Authors:	Dominique Kasper
 * Editors:	Raik Dankworth
 */
using UnityEngine;
using System.Collections;

public class BridgeScript : MonoBehaviour {

	private Transform m_billy = null;

	// Use this for initialization
	void Start () 
    {
        //gets the player
        GameObject player = GameObject.FindGameObjectWithTag(Tags.TAG_PLAYER);
        if (player == null)
        {
            Debug.Log("BrigdeScript: Player dont found!");
            return;
        }
        m_billy = player.transform;
	}
	
	void OnTriggerEnter(Collider other)
    {
        setTrigger(m_billy.position.y < this.transform.position.y);
	}

	void OnTriggerExit(Collider other) 
    {
        setTrigger(m_billy.position.y < this.transform.position.y);
	}

	//sets the bridge to solid (false) or not solid (true)
	public void setTrigger(bool isTrigger) {

		//gets the whole bridge
		Transform parent = this.transform.parent;
		
		Transform bridgeSegment = null;
		Transform bridgeSegmentModell = null;
		
		//walks through the bridge segments
		for (int i = 0; i < parent.childCount; i++) 
        {
			bridgeSegment = parent.GetChild(i);
			bridgeSegmentModell = bridgeSegment.GetChild(0);
			
			//sets the collider of the bridge segment to no trigger (to make it solid)
            bridgeSegmentModell.GetComponent<BrigdeScriptHelper>().setIsTrigger(isTrigger);		
        }
	}
}
