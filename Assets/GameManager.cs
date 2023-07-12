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

    const int WIDTH = 10, HEIGHT = 10, DEPTH = 10, BOMB_COUNT = 250, SAFE_AREA_SIZE = 1;

    Tile[] _tileArray;
    bool _firstClick;
    Vector3Int _firstClickPosition;

    public void OnTileClicked(GameObject tileVisual)
    {
        var clickedTilePos = GetTilePosFromVisual(tileVisual);
        var tile = GetTile(clickedTilePos);

        if (_firstClick)
        {
            _firstClickPosition = clickedTilePos;
            _firstClick = false;
            OnFirstClick();
        }

        CascadeDiscover(tile);
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
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
    }

    private void Reset()
    {
        _firstClick = true;
        InitTileArray();
        UpdateAllTileVisuals();

    }

    void OnFirstClick()
    {
        PlaceBombsRandomly();
        UpdateAdjacentBombCounts();
        UpdateAllTileVisuals();
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

                bool isInSafeArea = Mathf.Abs(_firstClickPosition.x - pos.x) <= SAFE_AREA_SIZE
                    && Mathf.Abs(_firstClickPosition.y - pos.y) <= SAFE_AREA_SIZE
                    && Mathf.Abs(_firstClickPosition.z - pos.z) <= SAFE_AREA_SIZE;

                if (!tile.IsBomb && !isInSafeArea)
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

    void UpdateAllTileVisuals()
    {
        foreach (var tilePos in AllTilePositions())
        {
            UpdateTileVisual(tilePos);
        }
    }

    void UpdateTileVisual(Vector3Int pos)
    {
        //Create visual if not existing
        var tile = GetTile(pos);
        if (tile.IsDiscovered)
        {
            if (tile.Visual)
            {
                Destroy(tile.Visual);
                tile.Visual = null;
            }
        }
        else
        {
            //Ensure visual exists
            if (!tile.Visual)
            {
                tile.Visual = Instantiate(_tilePrefab, pos, Quaternion.identity);
            }

            if (_debug)
            {
                if (tile.IsBomb)
                {
                    tile.Visual.GetComponent<Renderer>().material.color = Color.yellow;
                }
            }

            //Set a texture that displays the number of adjacent bombs.
            tile.Visual.GetComponent<MeshRenderer>().materials[1].mainTexture = _numbers[tile.AdjacentBombCount];
        }

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

    Vector3Int GetTilePosFromVisual(GameObject visual)
    {
        foreach (var pos in AllTilePositions())
        {
            var tile = GetTile(pos);
            if (tile.Visual == visual)
                return pos;
        }

        return new Vector3Int();
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

    void CascadeDiscover(Tile tile) => CascadeDiscover(tile, new HashSet<Tile>());
    void CascadeDiscover(Tile tile, HashSet<Tile> checkedTiles)
    {
        if (checkedTiles.Contains(tile))
            return;

        checkedTiles.Add(tile);
        tile.IsDiscovered = true;

        if (tile.IsFlagged)
        {
            tile.IsFlagged = false;
        }

        var tilePos = GetTilePosFromVisual(tile.Visual);
        UpdateTileVisual(tilePos);

        if (tile.AdjacentBombCount == 0)
        {
            foreach (var adjPos in TileAdjacentPositions(tilePos))
            {
                CascadeDiscover(GetTile(adjPos), checkedTiles);
            }
        }
    }

    class Tile
    {
        public GameObject Visual;
        public bool IsBomb, IsFlagged, IsDiscovered;
        public int AdjacentBombCount;
    }
}
