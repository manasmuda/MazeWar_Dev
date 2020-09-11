using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MazeConvertor 
{
    public static MazeCell[,] ToMazeArray(List<object> list)
    {
        MazeCell[,] maze = new MazeCell[20, 20];
        for(int x = 0; x<list.Count; x++)
        {
            int i = x / 20;
            int j = x % 20;
            string temp=Convert.ToString(list[x]);
            string[] temparr = temp.Split(',');
            maze[i, j] = new MazeCell(i,j);
            if (temparr[0] == "0")
            {
                maze[i, j].northWall = false;
            }
            if (temparr[1] == "0")
            {
                maze[i, j].southWall = false;
            }
            if (temparr[2] == "0")
            {
                maze[i, j].eastWall = false;
            }
            if (temparr[3] == "0")
            {
                maze[i, j].westWall = false;
            }
        }
        return maze;
    }
}
