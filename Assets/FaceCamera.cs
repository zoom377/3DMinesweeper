using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    Camera _mainCam;

    void Start()
    {
        _mainCam = Camera.main;
    }

    void Update()
    {
        var thisToCam = _mainCam.transform.position - transform.position;
        var rotation = Quaternion.LookRotation(thisToCam, _mainCam.transform.up);
        transform.rotation = rotation;
    }
}
