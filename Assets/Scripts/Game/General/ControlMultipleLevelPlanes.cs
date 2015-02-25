using UnityEngine;
using System.Collections;

public class ControlMultipleLevelPlanes : MonoBehaviour {

	private GameObject[] m_upperPlanes;
	//private GameObject[] m_upperPlaneObjects;

	private GameObject m_billy;
	private float m_billyHeight;

	//heigth which is added to the minimum y value of a plane to display it a little bit earlier than Billy is on its height
	public float m_minYOffset;

	//the upper plane control can be set off or on
	public bool m_isOn = true;

	void Awake() {
		//gets upper plane and its content
		m_upperPlanes = GameObject.FindGameObjectsWithTag ("UpperPlane");
		//m_upperPlaneObjects = GameObject.FindGameObjectsWithTag ("UpperPlaneObject");

		//gets Billy
        m_billy = GameObject.FindGameObjectWithTag(Tags.TAG_PLAYER);
		m_billyHeight = m_billy.transform.position.y;
	}

	// Use this for initialization
	void Start () {
		//controls the upper planes once at the start
		if (m_isOn)
			controlUpperPlanes ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//checks if a control of the upper planes is neccessary
		if (m_billyHeight != m_billy.transform.position.y && m_isOn) {
			//updates the saved height of billy
			m_billyHeight = m_billy.transform.position.y;

			controlUpperPlanes();
		}
	}

	//solves the problem if a level as more than one ground (the upper ground would block the sun from the lower ground in this case)
	//ATTENTION! To a upper plane belonging objects have to be wrapped in a game object named the same than the game object in which the
	//upper plane is wrapped
	private void controlUpperPlanes() {
		string upperPlaneName = "";
        MeshRenderer[] renderers = null;
        bool isHidden = false;

		//checks if there is a upper plane
		if (m_upperPlanes.Length > 0) {
			for (int i = 0; i < m_upperPlanes.Length; i++) {

                // Get renderers
                renderers = m_upperPlanes[i].GetComponentsInChildren<MeshRenderer>();
                if (renderers == null || renderers.Length == 0)
                    continue;

                // Check Billy's position against the current plane
                isHidden = m_billy.transform.position.y < (getMinYPosition(m_upperPlanes[i].transform) - m_minYOffset);

                // Hidden? Disable shadows
                foreach (MeshRenderer r in renderers)
                    r.enabled = !isHidden;

				//checks if Billy is below the current upper plane (so it can be hidden)
                /*if (isHidden)
                {
					upperPlaneName = m_upperPlanes[i].name;

					//deactivates the current upper plane
					m_upperPlanes[i].SetActive(false);*/

					//checks for objects on the current upper plane
					/*for (int j = 0; j < m_upperPlaneObjects.Length; j++) {
						if (m_upperPlaneObjects[j].name == upperPlaneName) {
							//deactivates the current object if it belongs to the current upper plane
							m_upperPlaneObjects[j].SetActive(false);

							Debug.Log (m_upperPlaneObjects[j].name + " is deactivated.");
						}
					}*/
				/*} else { //displays current upper plane if Billy is above it
					//activates the current upper plane
					m_upperPlanes[i].SetActive(true);*/

					//checks for objects on the current upper plane
					/*for (int j = 0; j < m_upperPlaneObjects.Length; j++) {
						if (m_upperPlaneObjects[j].name == upperPlaneName) {
							//activates the current object if it belongs to the current upper plane
							m_upperPlaneObjects[j].SetActive(true);

							Debug.Log (m_upperPlaneObjects[j].name + " is activated.");
						}
					}*/
				//}
			}
		}
	}

	//gets the minimum y value of the objects in a plane
	private float getMinYPosition(Transform plane) {
		float minY = float.MaxValue;

		for (int i = 0; i < plane.childCount; i++) {
			//checks if y value of the current object is smaller than the current minimum y value
			if (plane.GetChild(i).position.y < minY) {
				//updates minimum y value
				minY = plane.GetChild(i).position.y;
			}
		}

		return minY;
	}
}
