using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberEventSender : MonoBehaviour, IRaycastReceiver
{
    GameManager _gameManager;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    public void RaycastEnter()
    {
        _gameManager.OnNumberHoverEnter(transform.parent.gameObject);
    }

    public void RaycastExit()
    {
        _gameManager.OnNumberHoverExit(transform.parent.gameObject);
    }
}
