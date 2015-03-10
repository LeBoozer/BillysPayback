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
	public  float		m_distance_Z = 15.0f;
	public  float		m_distance_Y = 3.5f;
	public  float		m_damping = 0.01f;
	public  float		m_threshold = 0.0001f;
	public  bool		m_lookAt = true;

    public bool         m_secondVersion = false;
    private bool        m_useSecondVersion;
    private GameObject  m_lookAtTarget = null;



	#endregion

	#region Start
	// Use this for initialization
	void Start ()
	{
        m_useSecondVersion = m_secondVersion;
        setTheCamera();
	}
	#endregion


	#region Update

	// Update is called once per frame
	void Update () 
	{
		// there are no target object
		if (m_object == null)
		{
			GameObject h = GameObject.FindWithTag(Tags.TAG_PLAYER);
			
			if(h != null)
				m_object = h.transform;
			else
				return;
		}

        // version change?
        if (m_secondVersion ^ m_useSecondVersion)
        {
            m_useSecondVersion = m_secondVersion;
            setTheCamera();
        }

		// Calculate the new position of the camera
        // first version
        if (!m_useSecondVersion)
        {
            this.transform.position += new Vector3(customize(m_object.transform.position.x, this.transform.position.x, 1),
                                                     customize(m_object.transform.position.y, this.transform.position.y - m_distance_Y, 2),
                                                     customize(m_object.transform.position.z, this.transform.position.z + m_distance_Z, 1));
            if (m_lookAt)
                this.transform.LookAt(m_object);
            return;
        }

        // update the second version
        // set position
        this.transform.position += new Vector3(customize(m_object.transform.position.x, this.transform.position.x, 1),
                                                 customize(m_object.transform.position.y, this.transform.position.y, 2),
                                                 customize(m_object.transform.position.z, this.transform.position.z + m_distance_Z, 1));
        // look at?
        if (m_lookAt)
        {
            // set look at target
            m_lookAtTarget.transform.position = m_object.transform.position + new Vector3(0, m_distance_Y, 0);
            // look at 
            this.transform.LookAt(m_lookAtTarget.transform);
        }
	}

	// Calculate the movement of the camera 
	private float customize(float _obPos, float _ownPos, float _localDamping)
	{
		// under the threshold? -> set at the some position in this dimension
        if (Mathf.Abs( 1 / _localDamping *  m_damping * (_obPos - _ownPos)) < m_threshold)
			return _obPos - _ownPos;

		// damped motion
        return m_damping * (_obPos - _ownPos);
	}

    public void setTheCamera()
    {
        // no object select --> seek Billy
        if (m_object == null)
        {
            GameObject h = GameObject.FindWithTag(Tags.TAG_PLAYER);

            if (h != null)
                m_object = h.transform;
            else
            {
                Debug.Log("Kein Objekt gesetzt für die Kamera");
                return;
            }
        }
        // init values
        Vector3 obPos = m_object.transform.position;

        // set poisition
        // first version
        if (!m_useSecondVersion)
        {
            this.transform.position = new Vector3(obPos.x, obPos.y + m_distance_Y, obPos.z - m_distance_Z);
            return;
        }

        // look at target is null? -> create look at target
        if (m_lookAtTarget == null)
        {
            m_lookAtTarget = new GameObject();
            m_lookAtTarget.transform.parent = this.transform.parent;
        }

        // set camera
        this.transform.position = new Vector3(obPos.x, obPos.y, obPos.z - m_distance_Z);
    }

	#endregion
}
