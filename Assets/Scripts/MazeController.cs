﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeController : MonoBehaviour
{
    [SerializeField]
    private GameObject mazeObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InstantiateMaze(MazeCell[,] maze)
    {
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                float cellx = 6* j - 57;
                float celly = 2.0f;
                float cellz = 57 - 6 * i;

                if (maze[i, j].northWall)
                {
                    Instantiate(mazeObject, new Vector3(cellx, celly, cellz + 3f), Quaternion.Euler(270, 180, 0));
                }

                if (maze[i, j].southWall)
                {
                    Instantiate(mazeObject, new Vector3(cellx, celly, cellz - 3f), Quaternion.Euler(270, 0, 0));
                }

                if (maze[i, j].eastWall)
                {
                    Instantiate(mazeObject, new Vector3(cellx+3f, celly, cellz), Quaternion.Euler(270, 270, 0));
                }

                if (maze[i, j].westWall)
                {
                    Instantiate(mazeObject, new Vector3(cellx-3f, celly, cellz ), Quaternion.Euler(270, 90, 0));
                }
            }
        }
    }
}
