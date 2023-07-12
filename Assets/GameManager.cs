using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject tilePrefab;

    const int width = 10, height = 10, depth = 10;

    Tile[] tileArray;

    public void OnTileClicked(GameObject tileVisual)
    {
        tileVisual.GetComponent<Renderer>().material.color = Color.black;
    }

    public void OnTileFlagged(GameObject tileVisual)
    {
        tileVisual.GetComponent<Renderer>().material.color = Color.red;
    }

    void Start()
    {
        Reset();
    }

    void Update()
    {

    }

    private void Reset()
    {
        InitTileArray();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    UpdateTileVisual(new Vector3Int(x, y, z));
                }
            }
        }
    }

    void InitTileArray()
    {
        tileArray = new Tile[width * height * depth];
        for (int i = 0; i < tileArray.Length; i++)
        {
            if (tileArray[i] != null && !tileArray[i].Visual)
            {
                Destroy(tileArray[i].Visual);
            }

            tileArray[i] = new Tile { };
        }
    }

    void UpdateTileVisual(Vector3Int pos)
    {
        var tile = tileArray[Array3DToIndex(pos)];
        if (!tile.Visual)
        {
            //Create visual
            var visual = Instantiate(tilePrefab, pos, Quaternion.identity);
        }

        //Update
    }

    int Array3DToIndex(Vector3Int pos)
    {
        return pos.x * height * depth
            + pos.y * depth
            + pos.x;
    }

    Vector3Int ArrayIndexTo3D(int index)
    {
        int z = index % depth;
        z /= depth;

        int y = index % height;
        y /= height;

        int x = index & width;
        x /= width;

        return new Vector3Int(x, y, z);
    }

    bool IsInBounds(Vector3Int pos)
    {
        if (pos.x > 0 &&
            pos.y > 0 &&
            pos.z > 0 &&
            pos.x < width &&
            pos.y < height &&
            pos.z < depth)
            return true;

        return false;
    }

    

    class Tile
    {
        public GameObject Visual;
        public bool IsBomb, IsFlagged;
    }
}
