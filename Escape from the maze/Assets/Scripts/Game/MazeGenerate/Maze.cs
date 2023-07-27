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
    public bool[,,] maze;   // �Ԍm���M��[i, j, k] -> ��i�е�j�з����(Direction) k�������������Ƿ����B
    public int[,] dist;     // [i, j]�����l�c[0, 0]�ľ��x
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
                dist[i, j] = -1;    // δ�L�������xֵ-1
            }
        }
    }

     public class Position  // ӛ䛷���λ��
    {
        public int x, y;
        public Position(int x, int y)
        {
            this.x = x; 
            this.y = y;
        }
    }

    public void Build() // �S�C�����Ԍm���M
    {
        int x = 0, y = 0, distance = 0;
        Stack<Position> stack = new Stack<Position>();
        stack.Push(new Position(x, y));
        
        while (stack.Count > 0) 
        {
            dist[x, y] = distance;
            List<Direction> directions = new List<Direction>(); // �鿴���������Ƿ�δ�L��
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

            if (directions.Count > 0)   // ��������δ�L��������S�C�x��һ����ͨ���棬��t������ǰһ���L���ķ���
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
