using UnityEngine;
using System.Collections;

public class AntonioChange : MonoBehaviour
{

    void OnTriggerEnter(Collider _other)
    {
        // ignore all other game objects
        if (_other.tag != Tags.TAG_COMPANION)
            return;

        //
        GameObject _antonio = _other.gameObject;
        _antonio.GetComponent<Antonio>().enabled = false;
        _antonio.AddComponent("BossAntonio");
    }

}
