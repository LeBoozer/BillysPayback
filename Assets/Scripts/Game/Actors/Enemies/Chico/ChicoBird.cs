using UnityEngine;
using System.Collections;

public class ChicoBird : Enemy 
{
    private Transform m_player;

	// Use this for initialization
	void Start () 
    {
        // init
        m_canFall = true;
        m_canFly = true;
        m_allowToMove = true;

        // find player
        GameObject _p = GameObject.FindGameObjectWithTag(Tags.TAG_PLAYER);
        if (_p == null)
            Debug.LogWarning("ChicoBird: Player dont found!");
        else
            m_player = _p.transform;

        // base.start()
        base.Start();
        m_groundFlyValue *= 2f;
	}

    void FixedUpdate()
    {
        base.FixedUpdate();
    }

	// Update is called once per frame
	void Update () 
    {
        if (m_player != null && m_direction != Mathf.Sign(-this.transform.position.x + m_player.position.x))
                turnAround();
        base.Update();
	}
}
