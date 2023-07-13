using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject _tilePrefab;
    [SerializeField] Texture[] _numbers;
    [SerializeField] Texture _flag;
    [SerializeField] Color _hoverPrimary, _hoverSecondary;
    [SerializeField] bool _debug;

    const int SAFE_AREA_SIZE = 1;
    int _width = 10, _height = 10, _depth = 10, _bombCount = 100;

    Tile[] _tileArray;
    bool _firstClick;
    Vector3Int _firstClickPosition;



    void Start()
    {
        var state = FindObjectOfType<State>();
        if (state)
        {
            _width = state.Width;
            _height = state.Height;
            _depth = state.Depth;
            int totalTiles = _width * _height * _depth;
            _bombCount = Mathf.CeilToInt((float)totalTiles * (state.Percentage / 100f));
            Debug.Log($"Field parameters are width:${_width}, height:${_height}, depth:${_depth}, total:${totalTiles}, bombs:${_bombCount}");
        }


        Reset();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
    }

    public void OnTileClicked(GameObject tileVisual)
    {
        var clickedTilePos = GetTilePosFromVisual(tileVisual);
        var tile = GetTile(clickedTilePos);

        if (tile.IsFlagged)
            return;

        if (_firstClick)
        {
            _firstClickPosition = clickedTilePos;
            _firstClick = false;
            OnFirstClick();
        }

        CascadeDiscover(tile);
        UpdateAllTileVisuals();
    }

    public void OnTileFlagged(GameObject tileVisual)
    {
        //tileVisual.GetComponent<Renderer>().material.color = Color.red;
        var tilePos = GetTilePosFromVisual(tileVisual);
        var tile = GetTile(tilePos);

        tile.IsFlagged = !tile.IsFlagged;
        UpdateTileVisual(tilePos);
    }

    //public void OnTileHover

    public void OnTileHoverEnter(GameObject tileVisual)
    {
        var pos = GetTilePosFromVisual(tileVisual);
        var tile = GetTile(pos);

        tileVisual.transform.Find("Tile").GetComponent<ColorLerper>().SetColor(_hoverPrimary);

        foreach (var adjPos in TileAdjacentPositions(pos))
        {
            var adjTile = GetTile(adjPos);
            adjTile.Visual.transform.Find("BombCount").GetComponent<ColorLerper>().SetColor(_hoverSecondary);
        }
    }

    public void OnTileHoverExit(GameObject tileVisual)
    {
        var pos = GetTilePosFromVisual(tileVisual);
        var tile = GetTile(pos);

        tileVisual.transform.Find("Tile").GetComponent<ColorLerper>().SetColorToDefault();

        foreach (var adjPos in TileAdjacentPositions(pos))
        {
            var adjTile = GetTile(adjPos);
            adjTile.Visual.transform.Find("BombCount").GetComponent<ColorLerper>().SetColorToDefault();
        }
    }

    public void OnNumberHoverEnter(GameObject tileVisual)
    {
        //var pos = GetTilePosFromVisual(tileVisual);
        //var tile = GetTile(pos);

        //tileVisual.GetComponent<ColorLerper>().SetColor(_hoverPrimary);

        //foreach (var adjPos in TileAdjacentPositions(pos))
        //{
        //    var adjTile = GetTile(adjPos);
        //    adjTile.Visual.GetComponent<ColorLerper>().SetColor(_hoverSecondary);
        //}
    }

    public void OnNumberHoverExit(GameObject tileVisual)
    {

    }

    private void Reset()
    {
        _firstClick = true;
        InitTileArray();
        UpdateAllTileVisuals();

        Debug.Log(TileAdjacentPositions(new Vector3Int(5, 5, 5)).ToList().Count);
    }

    void OnFirstClick()
    {
        PlaceBombsRandomly();
        UpdateAdjacentBombCounts();
        UpdateAllTileVisuals();
    }

    void InitTileArray()
    {
        _tileArray = new Tile[_width * _height * _depth];
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

        while (bombsPlaced < _bombCount)
        {
            bool spaceFound = false;
            while (!spaceFound)
            {
                var pos = new Vector3Int(
                    UnityEngine.Random.Range(0, _width),
                    UnityEngine.Random.Range(0, _height),
                    UnityEngine.Random.Range(0, _depth));

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
        var tile = GetTile(pos);

        //Ensure visual exists
        if (!tile.Visual)
        {
            tile.Visual = Instantiate(_tilePrefab, pos, Quaternion.identity);
        }

        //var renderer = tile.Visual.GetComponent<MeshRenderer>();

        if (tile.IsDiscovered)
        {
            tile.Visual.transform.Find("Tile").gameObject.SetActive(false);

            if (tile.AdjacentBombCount > 0)
            {
                tile.Visual.transform.Find("BombCount").gameObject.SetActive(true);
                tile.Visual.GetComponentInChildren<TMP_Text>().text = tile.AdjacentBombCount.ToString();
            }
        }
        else
        {
            if (tile.IsFlagged)
                tile.Visual.transform.Find("Tile").GetComponent<MeshRenderer>().material.color = Color.red;
            else
                tile.Visual.transform.Find("Tile").GetComponent<MeshRenderer>().material.color = Color.grey;

            //if (_debug)
            //{
            //    if (tile.IsBomb)
            //    {
            //        tile.Visual.GetComponent<Renderer>().material.color = Color.yellow;
            //    }
            //}

            //renderer.materials[0].color = new Color(1, 1, 1, 1f);
            //renderer.materials[1].color = new Color(1, 1, 1, .0f);
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
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                for (int z = 0; z < _depth; z++)
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
        return pos.x * _height * _depth
            + pos.y * _depth
            + pos.z;
    }

    Vector3Int ArrayIndexTo3D(int index)
    {
        int z = index % _depth;
        z /= _depth;

        int y = index % _height;
        y /= _height;

        int x = index & _width;
        x /= _width;

        return new Vector3Int(x, y, z);
    }

    bool IsInBounds(Vector3Int pos)
    {
        if (pos.x >= 0 &&
            pos.y >= 0 &&
            pos.z >= 0 &&
            pos.x < _width &&
            pos.y < _height &&
            pos.z < _depth)
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
