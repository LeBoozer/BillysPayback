/*
 * Project:	Billy's Payback
 * File:	CP_Higgins.cs
 * Authors:	Raik Dankworth
 * Editors:	-
 */

using UnityEngine;
using System.Collections;

/*
 * special check point in level after obtained the shield
 */
public class CP_Higgins : CheckPoint 
{

    public GameObject m_higgins = null;

	// Use this for initialization
	void Start () 
    {
        // constructor of the checkpoints
        base.Start();

        // 
        if (m_higgins == null)
            return;

        Vector3 startPositionOfHiggins = m_higgins.transform.position;
        Higgins script = m_higgins.GetComponent<Higgins>();

        m_checkPointAction = () =>
        {
            // let break the boss fight
            script.BreakBossFight();
            
            // reset position of Higgins
            m_higgins.transform.position = startPositionOfHiggins;

            // start the boss fight
            script.StartBossFight();
        };

	}
	
}
