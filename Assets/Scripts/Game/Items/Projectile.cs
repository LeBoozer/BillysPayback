/*
 * Project:	Billy's Payback
 * File:	Projectile.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * TODO
 */
public class Projectile : Hitable
{
    private bool            m_alive = true;
    private float           m_birthTime;
    private readonly float  MAXIMAL_LIFETIME = 10f;

    // Override: MonoBehaviour::OnStart()
    void Start()
    {
        m_birthTime = Time.time;
    }

    void Update()
    {
        if (m_birthTime + MAXIMAL_LIFETIME < Time.time)
            Destroy(this.gameObject);
    }

    // Override: MonoBehaviour::OnCollisionEnter()
    void OnCollisionEnter(Collision _c)
    {
        // Local variables
        Hitable hitable = null;

        // Destroy game-object
        GameObject.Destroy(gameObject);
        m_alive = false;

        // Hitable?
        hitable = _c.gameObject.GetComponent<Hitable>();
        if (hitable == null)
            return;

        // Do damage
        hitable.onHit(this);

        // TODO Particle effect
    }

    // Override: Hitable::onHit()
    public override void onHit(Hitable _source)
    {
        // not alive?
        if (!m_alive)
            return;

        // die
        m_alive = false;

        // Do damage
        _source.onHit(this);

        // Destroy game-object
        GameObject.Destroy(gameObject);
    }

}