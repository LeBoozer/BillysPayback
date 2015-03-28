/*
 * Project:	Billy's Payback
 * File:	JumpingLittleBird.cs
 * Authors:	Raik Dankworth
 * Editors:	-
 */

using UnityEngine;
using System.Collections;

/**
 * A script to controll the speciale bird
 * Let tne baby bird jump over the position and bit the player
 */
public class JumpingLittleBird : Hitable
{

    #region Variable
    private float                       m_fly;
    private float                       m_groundFlyValue;
    private float                       m_lastHit;
    private bool                        m_skipUpdate = false;
    public int                          m_lifepoints = 1;

    protected CharacterController       m_controller;
    protected Vector3                   m_worldScale = Vector3.zero;
    #endregion

    // Use this for initialization
	void Start () 
    {
        // init
        m_fly = 0;
        m_lastHit = Time.time - 1;

        // Calculate world scale
        m_worldScale = HelperFunctions.getWorldScale(gameObject);

        // get controller
        m_controller = GetComponent<CharacterController>();

        // calculate the start velocity to jump
        m_groundFlyValue = 2 * Mathf.Sqrt(GameConfig.ENEMY_JUMP_HEIGHT * m_controller.height * m_worldScale.y * this.transform.localScale.y * Mathf.Abs(Physics.gravity.y));

        // Attach script
        MeshRendererOnVisible.attachScriptToRenderer(gameObject,
             (Camera _camera) =>
             {
                 // Player camera?
                 if (Camera.current.name.Equals(GameConfig.CAMERA_NAME_PLAYER) == true)
                     m_skipUpdate = false;
             });
	}

	// Update is called once per frame
	void Update ()
    {
        // Skip?
        if (m_skipUpdate == true)
            return;

        // movement
        if (m_controller.isGrounded)
            m_fly = m_groundFlyValue;
        else
            m_fly += 2 * Physics.gravity.y * Time.deltaTime;

        m_controller.Move( Time.deltaTime * new Vector3(0, m_fly, -transform.position.z / Time.deltaTime));

        // Set flag
        m_skipUpdate = true;
	}

    // Override: Hitable::onHit()
    public override void onHit(Hitable _source)
    {
        // Hit by enemy?
        if (_source is Enemy)
            return;

        // Take a hit
        if (m_lastHit + 1 < Time.time)
        {
            --m_lifepoints;
            m_lastHit = Time.time;
        }
        if (m_lifepoints == 0)
            Destroy(this.gameObject);
    }

    /**
     * 
     */
    public void OnCharacterControllerHit(object _hitInfo)
    {
        // Local variables
        ControllerColliderHit hitInfo = _hitInfo as ControllerColliderHit;
        Hitable hitable = null;

        // Useless message?
        if (hitInfo == null)
            return;

        // Hitable?
        hitable = hitInfo.controller.gameObject.GetComponent<Hitable>();
        if (hitable == null)
            return;

        // Notify
        hitable.onHit(this);
    }
    
    // Override: Monobehaviour::OnControllerColliderHit()
    public void OnControllerColliderHit(ControllerColliderHit _hit)
    {
        // Local variables
        Hitable hitable = _hit.gameObject.GetComponent<Hitable>();

        // Hitable?
        if (hitable == null)
            return;

        // Notify
        hitable.onHit(this);
    }
}
