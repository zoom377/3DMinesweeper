using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSens, aimSens;

    float aimX, aimY;
    Collider _lastCollider;

    void Start()
    {
        //UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        //UnityEngine.Cursor.visible = false;
    }

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
        move *= Time.deltaTime * moveSens;
        transform.Translate(move);

        //Aiming
        aimX -= Input.GetAxis("Mouse Y") * aimSens;
        aimY += Input.GetAxis("Mouse X") * aimSens;
        aimX = Mathf.Clamp(aimX, -90, 90);

        transform.rotation = Quaternion.Euler(aimX, aimY, 0);

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

    private void FixedUpdate()
    {
    }
}
