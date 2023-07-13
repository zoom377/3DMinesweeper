using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float _moveSens, _aimSens, _aimSmooth;
    [SerializeField] bool _smoothingEnabled;

    Vector2 _aim, _targetAim;

    Collider _lastCollider;

    void Update()
    {

        if (UnityEngine.Cursor.lockState == CursorLockMode.None)
        {
            if (Input.GetMouseButtonDown(0))
            {
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false;
            }

            return;
        }


        //Movement
        var moveX = Input.GetAxis("Horizontal");
        var moveY = Input.GetAxis("Vertical");
        var moveZ = Input.GetAxis("UpDown");

        Vector3 move = new Vector3(moveX, moveZ, moveY);
        move *= Time.deltaTime * _moveSens;
        transform.Translate(move);

        //Aiming
        _targetAim.x -= Input.GetAxis("Mouse Y") * _aimSens;
        _targetAim.y += Input.GetAxis("Mouse X") * _aimSens;
        _targetAim.x = Mathf.Clamp(_targetAim.x, -90, 90);

        _aim = Vector2.Lerp(_aim, _targetAim, _aimSmooth * Time.deltaTime);

        transform.rotation = Quaternion.Euler(_aim.x, _aim.y, 0);

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0))
            {
                FindObjectOfType<GameManager>().OnTileClicked(hit.transform.parent.gameObject);
            }

            if (Input.GetMouseButtonDown(1))
            {
                FindObjectOfType<GameManager>().OnTileFlagged(hit.transform.parent.gameObject);
            }

        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
        }
    }

}
