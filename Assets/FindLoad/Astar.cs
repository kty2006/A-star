using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

public class Astar : MonoBehaviour
{
    public int Atype;
    public Vector3Int StrPos;
    public Vector3Int EndPos;

    public List<CellData> openList = new List<CellData>();
    public HashSet<Vector3Int> closedList = new HashSet<Vector3Int>();
    public HashSet<Vector3Int> WallPoses = new HashSet<Vector3Int>();

    public Vector3Int[] dir = new Vector3Int[8]
    {
        new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0),
        new Vector3Int(1, -1, 0), new Vector3Int(-1, 1, 0), new Vector3Int(1, 1, 0), new Vector3Int(-1, -1, 0)
    };

    public IEnumerator FindTarget()
    {
        openList.Clear();
        closedList.Clear();
        openList.Add(new CellData(StrPos, null, 0, CalculateHeuristic(StrPos, EndPos)));

        while (openList.Count > 0)
        {
            openList = openList.OrderBy(x => x.F).ThenByDescending(x => x.G).ToList();
            CellData currentCell = openList.First();
            openList.Remove(currentCell);


            if (currentCell.CurrentPos == EndPos)
            {
                FillRoad(currentCell);
                yield break;
            }

            for (int i = 0; i < Atype; i++)
            {
                Vector3Int neighborPos = currentCell.CurrentPos + dir[i];

                if (WallPoses.Contains(neighborPos) || closedList.Contains(neighborPos) || IsCornerCutting(currentCell.CurrentPos, neighborPos))
                {
                    continue;
                }

                int tentativeG = currentCell.G + ((Mathf.Abs(dir[i].x) + Mathf.Abs(dir[i].y) == 1) ? 10 : 14);
                int h = CalculateHeuristic(neighborPos, EndPos);
                int f = tentativeG + h;

                CellData existingNeighbor = openList.Find(cell => cell.CurrentPos == neighborPos);
                if (existingNeighbor != null && tentativeG >= existingNeighbor.G)
                {
                    continue;
                }

                if (existingNeighbor == null)
                {
                    openList.Add(new CellData(neighborPos, currentCell, tentativeG, h));
                    if (Map.Instance.Tilemap.GetColor(neighborPos) != Color.red)
                    { Map.Instance.FillColor(neighborPos, Color.yellow); }
                    closedList.Add(currentCell.CurrentPos);
                }
                else
                {
                    existingNeighbor.Parent = currentCell;
                    existingNeighbor.G = tentativeG;
                    existingNeighbor.F = f;
                }
            }

            yield return null;
        }
    }

    private void FillRoad(CellData cellData)
    {
        if (cellData.Parent != null)
        {
            if(Map.Instance.Tilemap.GetColor(cellData.CurrentPos) != Color.red)
                Map.Instance.FillColor(cellData.CurrentPos, Color.green);
            FillRoad(cellData.Parent);
        }
    }

    private int CalculateHeuristic(Vector3Int currentPos, Vector3Int endPos)
    {
        int x = Mathf.Abs(endPos.x - currentPos.x);
        int y = Mathf.Abs(endPos.y - currentPos.y);
        int min = Mathf.Min(x, y);
        int max = Mathf.Max(x, y);
        return min * 14 + (max - min) * 10;
    }

    private bool IsCornerCutting(Vector3Int currentPos, Vector3Int nextPos)
    {
        if (Mathf.Abs(currentPos.x - nextPos.x) == 1 && Mathf.Abs(currentPos.y - nextPos.y) == 1)
        {
            Vector3Int neighbor1 = new Vector3Int(currentPos.x, nextPos.y, 0);
            Vector3Int neighbor2 = new Vector3Int(nextPos.x, currentPos.y, 0);
            return WallPoses.Contains(neighbor1) && WallPoses.Contains(neighbor2);
        }
        return false;
    }

    public class CellData
    {
        public Vector3Int CurrentPos;
        public CellData Parent;
        public int G;
        public int H;
        public int F;

        public CellData(Vector3Int currentPos, CellData parent, int g, int h)
        {
            this.CurrentPos = currentPos;
            this.Parent = parent;
            this.G = g;
            this.H = h;
            this.F = g + h;
        }
    }
}
