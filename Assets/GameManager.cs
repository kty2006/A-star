using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleTone<GameManager>
{
    [HideInInspector] public Astar Astar;

    public void Start()
    {
        Astar = new Astar();
    }
    public void StartButton()
    {
        StartCoroutine(Astar.FindTarget());
    }

    public void Manhattan()
    {
        Astar.Atype = 4;
        Debug.Log("맨허튼거리");
    }

    public void Euclidean()
    {
        Astar.Atype = 8;
    }
}
