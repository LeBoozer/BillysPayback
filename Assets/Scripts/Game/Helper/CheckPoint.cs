using UnityEngine;
using System.Collections;

public class CheckPoint : MonoBehaviour 
{

    private Player  m_player;

	// Use this for initialization
	void Start () 
    {
        m_player = GameObject.FindGameObjectWithTag(Tags.TAG_PLAYER).GetComponent<Player>();
	}


    public void OnTriggerEnter(Collider _obj)
    {
        if (!_obj.gameObject.tag.Equals(Tags.TAG_PLAYER))
            return;

        m_player.setChechPoint(transform.position);
        this.gameObject.SetActive(false);
    }


}
