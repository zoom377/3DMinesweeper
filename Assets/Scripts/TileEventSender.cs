using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileEventSender : MonoBehaviour, IRaycastReceiver
{

    GameManager _gameManager;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    public void RaycastEnter()
    {
        _gameManager.OnTileHoverEnter(transform.parent.gameObject);
    }

    public void RaycastExit()
    {
        _gameManager.OnTileHoverExit(transform.parent.gameObject);
    }

}
