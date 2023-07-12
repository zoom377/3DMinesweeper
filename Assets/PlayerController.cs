using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSens, aimSens;

    float aimX, aimY;

    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    void Update()
    {
        //Movement
        var moveX = Input.GetAxis("Horizontal");
        var moveY = Input.GetAxis("Vertical");
        var moveZ = Input.GetAxis("UpDown");

        Vector3 move = new Vector3(moveX, moveZ, moveY);
        move *= Time.deltaTime * moveSens;
        transform.Translate(move);

        //Aiming
        aimX -= Input.GetAxis("Mouse Y") * Time.deltaTime * aimSens;
        aimY += Input.GetAxis("Mouse X") * Time.deltaTime * aimSens;
        aimX = Mathf.Clamp(aimX, -90, 90);

        transform.rotation = Quaternion.Euler(aimX, aimY, 0);


        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                FindObjectOfType<GameManager>().OnTileClicked(hit.transform.gameObject);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                FindObjectOfType<GameManager>().OnTileFlagged(hit.transform.gameObject);
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
