using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class Map : MonoSingleTone<Map>
{
    private Color StartPos = Color.blue, TargetPos = Color.red, WallPos = Color.black;
    public Color selectColor;
    public Tilemap Tilemap;
    Vector2 pos;
    Vector3Int cell;
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cell = Tilemap.WorldToCell(pos);
            FillColor(cell, selectColor);
            if (selectColor == StartPos)
                GameManager.Instance.Astar.StrPos = cell;
            else if (selectColor == TargetPos)
                GameManager.Instance.Astar.EndPos = cell;
            else if (selectColor == WallPos)
                GameManager.Instance.Astar.WallPoses.Add(cell);
        }
    }

    public void FillColor(Vector3Int cellPos,Color color)
    {
        Tilemap.SetTileFlags(cellPos, TileFlags.None);
        Tilemap.SetColor(cellPos, color);
    }

    public void StartButton()
    {
        selectColor = StartPos;
    }
    public void TargetButton()
    {
        selectColor = TargetPos;
    }
    public void WallButton()
    {
        selectColor = WallPos;
    }

    public void ClearMap()
    {
        SceneManager.LoadScene(0);
    }
}
