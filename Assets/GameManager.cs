using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject _tilePrefab;
    [SerializeField] Texture[] _numbers;
    [SerializeField] Texture _flag;
    [SerializeField] bool _debug;

    const int WIDTH = 10, HEIGHT = 10, DEPTH = 10, BOMB_COUNT = 250;

    Tile[] _tileArray;

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
        PlaceBombsRandomly();
        UpdateAdjacentBombCounts();
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                for (int z = 0; z < DEPTH; z++)
                {
                    UpdateTileVisual(new Vector3Int(x, y, z));
                }
            }
        }
    }

    void InitTileArray()
    {
        _tileArray = new Tile[WIDTH * HEIGHT * DEPTH];
        for (int i = 0; i < _tileArray.Length; i++)
        {
            if (_tileArray[i] != null && !_tileArray[i].Visual)
            {
                Destroy(_tileArray[i].Visual);
            }

            _tileArray[i] = new Tile { };
        }
    }

    void PlaceBombsRandomly()
    {
        var bombsPlaced = 0;

        while (bombsPlaced < BOMB_COUNT)
        {
            bool spaceFound = false;
            while (!spaceFound)
            {
                var pos = new Vector3Int(
                    UnityEngine.Random.Range(0, WIDTH),
                    UnityEngine.Random.Range(0, HEIGHT),
                    UnityEngine.Random.Range(0, DEPTH));

                Tile tile = GetTile(pos);
                if (!tile.IsBomb)
                {
                    spaceFound = true;
                    tile.IsBomb = true;
                    bombsPlaced++;
                }
            }
        }
    }

    void UpdateAdjacentBombCounts()
    {
        foreach (var pos in AllTilePositions())
        {
            var tile = GetTile(pos);
            foreach (var adjPos in TileAdjacentPositions(pos))
            {
                var adjTile = GetTile(adjPos);
                if (adjTile.IsBomb)
                {
                    tile.AdjacentBombCount++;
                }
            }
        }

    }



    void UpdateTileVisual(Vector3Int pos)
    {
        //Create visual if not existing
        var tile = GetTile(pos);
        if (!tile.Visual)
        {
            tile.Visual = Instantiate(_tilePrefab, pos, Quaternion.identity);
        }

        //Update
        if (_debug)
        {
            if (tile.IsBomb)
            {
                tile.Visual.GetComponent<Renderer>().material.color = Color.yellow;
            }
        }

        //Set a texture that displays the number of adjacent bombs.
        //tile.Visual.GetComponent<MeshRenderer>().materials[1].SetTexture("Albedo" ,_numbers[tile.AdjacentBombCount]);
        tile.Visual.GetComponent<MeshRenderer>().materials[1].mainTexture = _numbers[tile.AdjacentBombCount];
    }

    IEnumerable<Vector3Int> TileAdjacentPositions(Vector3Int pos)
    {
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                for (int z = -1; z < 2; z++)
                {
                    if (x == 0 && y == 0 && z == 0)
                        continue;

                    var adjPos = pos + new Vector3Int(x, y, z);
                    if (IsInBounds(adjPos))
                    {
                        yield return adjPos;
                    }
                }
            }
        }
    }

    IEnumerable<Vector3Int> AllTilePositions()
    {
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                for (int z = 0; z < DEPTH; z++)
                {
                    yield return new Vector3Int(x, y, z);
                }
            }
        }
    }

    Tile GetTile(Vector3Int pos)
    {
        return _tileArray[Array3DToIndex(pos)];
    }

    int Array3DToIndex(Vector3Int pos)
    {
        return pos.x * HEIGHT * DEPTH
            + pos.y * DEPTH
            + pos.z;
    }

    Vector3Int ArrayIndexTo3D(int index)
    {
        int z = index % DEPTH;
        z /= DEPTH;

        int y = index % HEIGHT;
        y /= HEIGHT;

        int x = index & WIDTH;
        x /= WIDTH;

        return new Vector3Int(x, y, z);
    }

    bool IsInBounds(Vector3Int pos)
    {
        if (pos.x >= 0 &&
            pos.y >= 0 &&
            pos.z >= 0 &&
            pos.x < WIDTH &&
            pos.y < HEIGHT &&
            pos.z < DEPTH)
            return true;

        return false;
    }



    class Tile
    {
        public GameObject Visual;
        public bool IsBomb, IsFlagged, IsDiscovered;
        public int AdjacentBombCount;
    }
}
