using UnityEngine;
using System.Collections;

public class BrigdeScriptHelper : MonoBehaviour {

    private bool        m_isTrigger = true;
    private bool        m_onTrigger = false;
    private Collider    m_ownCollider;

    void Awake()
    {
        m_ownCollider = this.GetComponent<Collider>();
    }

	
    void OnTriggerEnter(Collider other)
    {
        m_onTrigger = true;
    }

    void OnTriggerExit(Collider other)
    {
        // set trigger value
        m_onTrigger = false;
        if(m_ownCollider != null)
            m_ownCollider.isTrigger = m_isTrigger;
    }

    public void setIsTrigger(bool _newIsTriggerValue)
    {
        // the some value?
        if (m_isTrigger == _newIsTriggerValue)
            return;
        m_isTrigger = _newIsTriggerValue;

        // dont trigger at this time?
        if (!m_onTrigger && m_ownCollider != null)
            m_ownCollider.isTrigger = m_isTrigger;
    }
}
