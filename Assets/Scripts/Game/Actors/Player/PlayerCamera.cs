/*
 * Project:	Billy's Payback
 * File:	Camera.cs
 * Authors:	Raik Dankworth
 * Editors:	-
 */

using UnityEngine;
using System.Collections;

/*
 * Control of the camera at a target object
 */
public class PlayerCamera : MonoBehaviour {

	#region Variable
	public 	Transform 	m_object = null;
	public  float		m_distance = 15.0f;
	public  float		m_damping = 0.01f;
	public  float		m_threshold = 0.0001f;
	public  bool		m_lookAt = true;


	#endregion

	#region Start
	// Use this for initialization
	void Start ()
	{
		// no object select --> seek Billy
		if (m_object == null)
		{
			GameObject h = GameObject.FindWithTag(Tags.TAG_PLAYER);

			if(h != null)
				m_object = h.transform;
			else
			{
				Debug.Log ("Kein Objekt gesetzt für die Kamera");
				return;
			}
		}
		// init values
		Vector3 obPos = m_object.transform.position;
		this.transform.position = new Vector3 (obPos.x, obPos.y, obPos.z - m_distance);

	}
	#endregion


	#region Update

	// Update is called once per frame
	void Update () 
	{
		// there are no target object
		if (m_object == null)
			return;
		// Calculate the new position of the camera
		this.transform.position += new Vector3 (customize (m_object.transform.position.x, this.transform.position.x),
			                                     customize (m_object.transform.position.y, this.transform.position.y),
			                                     customize (m_object.transform.position.z, this.transform.position.z + m_distance));
		if (m_lookAt)
			this.transform.LookAt (m_object);
	}

	// Calculate the movement of the camera 
	private float customize(float _obPos, float _ownPos)
	{
		// under the threshold? -> set at the some position in this dimension
		if (Mathf.Abs(m_damping * (_obPos - _ownPos)) < m_threshold)
			return _obPos - _ownPos;

		// damped motion
		return m_damping * (_obPos - _ownPos);
	}

	#endregion
}
