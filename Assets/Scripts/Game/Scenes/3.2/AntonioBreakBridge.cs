/*
 * Project:	Billy's Payback
 * File:	AntonioBreakBridge.cs
 * Authors:	Raik Dankworth
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Special Trigger for Level 3.2 
 * let break the brigde behind antonio
 */ 
public class AntonioBreakBridge : MonoBehaviour {

    private class TransformComparer : IComparer
    {
        public Transform m_containedTransform;
        public float m_x;

        public TransformComparer(Transform _t)
        {
            m_containedTransform = _t;
            m_x = _t.position.x;
        }

        int IComparer.Compare(System.Object _first, System.Object _second)
        {
            TransformComparer f = _first as TransformComparer;
            TransformComparer s = _second as TransformComparer;

            if (f == null && s == null)
                return 0;

            if (f == null)
                return -1;

            if (s == null)
                return 1;

            return m_x.CompareTo(s.m_x);
        }
    };


    private Queue<Transform> m_bridgeSegments;
    private int counter;

	// Use this for initialization
	void Start () 
    {
        // init
        m_bridgeSegments = new Queue<Transform>();
        List<TransformComparer> unsortedList = new List<TransformComparer>();

        for (int i = this.transform.childCount; i > 0; )
        {
            Transform currentChild = this.transform.GetChild(--i);
            unsortedList.Add(new TransformComparer(currentChild));
        }

        unsortedList.Sort(delegate(TransformComparer f, TransformComparer s)
        {
            return f.m_x.CompareTo(s.m_x);
        });
        
        for (int i = 0; i < unsortedList.Count; ++i)
            m_bridgeSegments.Enqueue(unsortedList[i].m_containedTransform);

        counter = unsortedList.Count;

	}

    void OnTriggerStay(Collider _other)
    {
        // ignore all other game objects
        if (_other.tag != Tags.TAG_COMPANION || counter == 0 )
            return;

        // the remove of a bridge segment is not necessary?
        if (_other.gameObject.transform.position.x < m_bridgeSegments.Peek().position.x)
            return;

        // get current bridge segment
        Transform current = m_bridgeSegments.Dequeue();

        // remove it
        Destroy(current.gameObject);
        --counter;
    }
	
}
