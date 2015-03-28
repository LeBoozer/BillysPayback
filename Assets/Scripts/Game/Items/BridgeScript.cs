﻿using UnityEngine;
using System.Collections;

public class BridgeScript : MonoBehaviour {

	private Transform m_billy;

	void Awake() {

		//gets the player
		m_billy = GameObject.Find("Billy").gameObject.transform;
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//checks if billy is under the bridge by now (make the bridge not solid again)
		if (m_billy.position.y < this.transform.position.y) {
			setTrigger(true);
		} else if (m_billy.position.y > this.transform.position.y) { //makes bridge solid if Billy was above the bridge all the time
			setTrigger (false);
		}
	
	}

	void OnTriggerEnter(Collider other) {
	}

	void OnTriggerExit(Collider other) {
		setTrigger (false);
	}

	//sets the bridge to solid (false) or not solid (true)
	public void setTrigger(bool isTrigger) {
		//gets the whole bridge
		Transform parent = this.transform.parent.transform.parent.transform;
		
		Transform bridgeSegment = null;
		Transform bridgeSegmentModell = null;
		
		//walks through the bridge segments
		for (int i = 0; i < parent.childCount; i++) {
			bridgeSegment = parent.GetChild(i);
			bridgeSegmentModell = bridgeSegment.GetChild(0);
			
			//sets the collider of the bridge segment to no trigger (to make it solid)
			bridgeSegmentModell.GetComponent<Collider>().isTrigger = isTrigger;
		}
	}
}
