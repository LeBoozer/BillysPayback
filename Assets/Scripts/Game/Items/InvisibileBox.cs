using UnityEngine;
using System.Collections;

public class InvisibileBox : MonoBehaviour
{

    int m_enter = 0;
    Vector3 m_enterPosition = Vector3.zero;

    // Use this for initialization
    void Start()
    {
        foreach (Transform t in transform)
            t.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider _other)
    {
        // not the player?
        if (!_other.gameObject.tag.Equals(Tags.TAG_PLAYER))
            return;


        m_enterPosition = _other.gameObject.transform.position;
    }

    public void OnTriggerStay(Collider _other)
    {
        // not the player?
        if (!_other.gameObject.tag.Equals(Tags.TAG_PLAYER))
            return;

        ++m_enter;

        if(m_enter != 1)
            return;

        Vector3 moveDirection = _other.gameObject.transform.position - m_enterPosition;

        // move direction from Player up?
        if (Vector3.Dot(moveDirection.normalized, Vector3.up) >= GameConfig.ENEMY_PLAYER_ABOVE_FACTOR)
        {
            // active the model
            foreach (Transform t in transform)
                t.gameObject.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider _other)
    {
        // not the player?
        if (!_other.gameObject.tag.Equals(Tags.TAG_PLAYER))
            return;

        m_enter = 0;
    }
}

