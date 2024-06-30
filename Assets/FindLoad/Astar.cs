using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;
using System.Threading.Tasks;
using System;
using Unity.VisualScripting;

public class Astar : MonoBehaviour
{
    public int Atype;
    public Vector3Int StrPos;
    public Vector3Int EndPos;
    public CellData CurrentData;
    public Queue<CellData> Cells = new();
    public HashSet<Vector3Int> WallPoses = new();
    public HashSet<Vector3Int> CheckedCells = new();
    Vector3Int[] dir = new Vector3Int[8]
    {
        new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0), new Vector3Int(1, -1, 0), new Vector3Int(-1, 1, 0), new Vector3Int(1, 1, 0), new Vector3Int(-1, -1, 0) }
    ;

    public IEnumerator FindTarget()
    {
        FirstSet();
        while (Cells.Count > 0)
        {
            CurrentData = Cells.Dequeue();
            if (CurrentData.CurrentPos == EndPos)
            {
                for (int i = 1; i < CurrentData.CellRoad.Count - 1; i++)
                {
                    Map.Instance.FillColor(CurrentData.CellRoad[i], Color.green);
                }
                yield break;
            }
            FindDir(CurrentData);
            yield return null;
        }
    }

    private void FirstSet()
    {
        List<Vector3Int> pos = new();
        pos.Add(StrPos);
        CheckedCells.Add(StrPos);
        CurrentData = new CellData(StrPos, pos);
        Cells.Enqueue(CurrentData);
    }

    public void FindDir(CellData cell)
    {
        Vector3Int pos;
        for (int i = 0; i < Atype; i++)
        {
            pos = cell.CurrentPos + dir[i];
            if (!WallPoses.Contains(pos) && !CheckedCells.Contains(pos))
            {
                CellData currentCell = new CellData(pos, cell.CellRoad);
                currentCell.CellRoad.Add(pos);
                currentCell.G = currentCell.CellRoad.Count - 1;
                currentCell.H = (Atype == 4) ? Mathf.Abs(currentCell.CurrentPos.x - EndPos.x) + Mathf.Abs(currentCell.CurrentPos.y - EndPos.y) : (int)(Mathf.Sqrt(currentCell.CurrentPos.x - EndPos.x))+ (int)(Mathf.Sqrt(currentCell.CurrentPos.y - EndPos.y));
                currentCell.F = currentCell.G + currentCell.H;
                Cells.Enqueue((currentCell));
                if (Map.Instance.Tilemap.GetColor(currentCell.CurrentPos) != Color.red)
                {
                    Map.Instance.FillColor(currentCell.CurrentPos, Color.yellow);
                }
                CheckedCells.Add(pos);
            }
        }
    }
    public class CellData
    {
        public int F;
        public int G;
        public int H;
        public Vector3Int CurrentPos;
        public List<Vector3Int> CellRoad;

        public CellData(Vector3Int currentPos, List<Vector3Int> pos)
        {
            this.CurrentPos = currentPos;
            this.CellRoad = pos.ToList();
        }
    }
}

