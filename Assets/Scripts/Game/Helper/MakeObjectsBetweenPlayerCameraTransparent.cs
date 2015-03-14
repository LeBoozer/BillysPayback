/*
 * Project:	Billy's Payback
 * File:	MakeObjectsBetweenPlayerCameraTransparent.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * ToDo
 */
[RequireComponent(typeof(Camera))]
public class MakeObjectsBetweenPlayerCameraTransparent : MonoBehaviour
{
    // True if valid
    private bool m_isValid = false;

    // The player instance
    public Player m_player = null;

    // True to auto find the player
    public bool m_autoFindPlayer = true;

    // Radius for the auto transparency
    public float m_transparenceRadius = 10.0f;

    // The test distance
    public float m_testDistance = 10.0f;

    // Override: MonoBehaviour::Awake()
    void Awake()
    {
        // Local variables
        GameObject obj = null;

        // Auto find player?
        if (m_autoFindPlayer == true)
        {
            obj = GameObject.FindGameObjectWithTag(Tags.TAG_PLAYER);
            if (obj != null)
                m_player = obj.GetComponent<Player>();
        }
        if(m_player == null)
        {
            Debug.LogError("No player object has been defined!");
            return;
        }

        // Set flag
        m_isValid = true;
    }

    // Override: MonoBehaviour::Update()
    void Update()
    {
        // Local variables
        RaycastHit[] hits = null;
        Renderer[] renderers = null;
        AutoTransparent autoTrans = null;
        int layerMask = 0;

        // Check flag
        if(m_isValid == false)
            return;

        // Create ignore layer
        layerMask = (1 << Layer.getLayerIDByName(Layer.LAYER_ENVIROMENT));

        // Cast capsule
        hits = Physics.SphereCastAll(this.transform.position - this.transform.forward * m_transparenceRadius * 2, m_transparenceRadius, this.transform.forward, m_testDistance * m_transparenceRadius * 2, layerMask);
        if (hits == null || hits.Length == 0)
            return;

        // Loop through all hits
        foreach(RaycastHit hit in hits)
        {
            // Try to get the renderer instance
            renderers = hit.collider.GetComponentsInChildren<Renderer>();
            if (renderers == null || renderers.Length == 0)
                continue;
            foreach (Renderer renderer in renderers)
            {
                // Check
                if (renderer == null)
                    continue;

                // Get component
                autoTrans = renderer.GetComponent<AutoTransparent>();
                if (autoTrans == null)
                    autoTrans = renderer.gameObject.AddComponent<AutoTransparent>();
                autoTrans.makeTransparent();
            }
        }
    }
}
