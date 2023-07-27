using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public enum Direction
{
    Left, Right, Up, Down
}
public class Maze
{
    public bool[,,] maze;   // 迷宮數組：[i, j, k] -> 第i行第j列方格跟(Direction) k方向相鄰方格是否相連
    public int[,] dist;     // [i, j]跟出發點[0, 0]的距離
    private int row, col;

    public Maze(int row, int col)
    {
        this.row = row;
        this.col = col;
        maze = new bool[row, col, 4];
        dist = new int[row, col];
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                dist[i, j] = -1;    // 未訪問方格賦值-1
            }
        }
    }

     public class Position  // 記錄方格位置
    {
        public int x, y;
        public Position(int x, int y)
        {
            this.x = x; 
            this.y = y;
        }
    }

    public void Build() // 隨機生成迷宮數組
    {
        int x = 0, y = 0, distance = 0;
        Stack<Position> stack = new Stack<Position>();
        stack.Push(new Position(x, y));
        
        while (stack.Count > 0) 
        {
            dist[x, y] = distance;
            List<Direction> directions = new List<Direction>(); // 查看附近方格是否未訪問
            if (y > 0 && dist[x, y - 1] == -1)
            {
                directions.Add(Direction.Left);
            }
            if (y < col - 1 && dist[x, y + 1] == -1)
            {
                directions.Add(Direction.Right);
            }
            if (x > 0 && dist[x - 1, y] == -1)
            {
                directions.Add(Direction.Up);
            }
            if (x < row - 1 && dist[x + 1, y] == -1)
            {
                directions.Add(Direction.Down);
            }

            if (directions.Count > 0)   // 附近存在未訪問方格就隨機選擇一個打通墻面，否則回溯至前一次訪問的方格
            {
                distance++;
                stack.Push(new Position(x, y));
                switch (directions[Random.Range(0, directions.Count)])
                {
                    case Direction.Left:
                        maze[x, y, 0] = true;
                        maze[x, --y, 1] = true;
                        break;
                    case Direction.Right:
                        maze[x, y, 1] = true;
                        maze[x, ++y, 0] = true;
                        break;
                    case Direction.Up:
                        maze[x, y, 2] = true;
                        maze[--x, y, 3] = true;
                        break;
                    case Direction.Down:
                        maze[x, y, 3] = true;
                        maze[++x, y, 2] = true;
                        break;
                }
            }
            else
            {
                distance--;
                Position position = stack.Pop();
                x = position.x; 
                y = position.y;
            }
        }
        Debug.Log("Destination distance = " + dist[row - 1, col - 1]);
    }
}
