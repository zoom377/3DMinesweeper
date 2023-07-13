using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastSender : MonoBehaviour
{
    [SerializeField] Vector3 _localDirection, _offset;
    [SerializeField] float _maxDistance;

    Collider _lastHit;

    void Update()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position + _offset, transform.TransformDirection(_localDirection), out hit, _maxDistance);

        if (_lastHit != hit.collider)
        {
            if (_lastHit)
            {
                _lastHit.GetComponent<IRaycastReceiver>()?.RaycastExit();
            }

            if (hit.collider)
            {
                hit.collider.GetComponent<IRaycastReceiver>()?.RaycastEnter();
            }

            _lastHit = hit.collider;
        }
    }
}
