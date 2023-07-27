using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class MazeGenerator : MonoBehaviour
{
    public static MazeGenerator instance;
    [Range(10, 14)]
    public int row, col;        // 迷宮數組的大小
    public int scale, height;   // 迷宮數組每一格對應游戲中的大小 -> [scale, height, scale]

    public GameObject map, keys, enemies;   // 存放相應預製體的集合
    public GameObject[] prefabList;         // MapGenerator類使用

    public int[] mapSize;       // MapGenerator類使用
    public int[,,] mapArray;    // 記錄場景對應坐標上放置的預製體

    public GameObject playerPrefab;
    public Vector3 playerInitPosition;  // 玩家生成位置

    public List<CornerInfo> corners = new List<CornerInfo>();   // 地圖上三面靠墻的區域（角落）
    private int numOfKeys, numOfEnemies;    // 鑰匙和敵人總數
    public GameObject keyPrefab;            
    public GameObject enemyPrefab;

    public Transform miniMapCamera; // 小地圖鏡頭控制

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetParameter(GameObject.Find("GameData").GetComponent<GameData>().difficulty);  // 從GameData物體獲取難度參數
        Maze i_maze = new Maze(row, col);
        i_maze.Build();
        bool[,,] mazeArray = i_maze.maze;
        mapSize = new int[] {scale * row, height, scale * col}; // 地圖大小：[row, 1, col] * [scale, height, scale]
        GenerateMapArray(mazeArray);
        MapGenerator.GenerateMap(map, mapSize, mapArray, prefabList);
        GeneratePlayer();
        GenerateKeyAndEnemy(i_maze);
    }

    private void SetParameter(Difficulty difficulty) // 根據難度設定地圖參數
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                row = 10;
                col = 10;
                numOfKeys = 1;
                numOfEnemies = 3;
                miniMapCamera.position = new Vector3(0f, 55f, 0f);
                break;
            case Difficulty.Normal:
                row = 12;
                col = 12;
                numOfKeys = 2;
                numOfEnemies = 6;
                miniMapCamera.position = new Vector3(0f, 66f, 0f);
                break;
            case Difficulty.Hard:
                row = 14;
                col = 14;
                numOfKeys = 3;
                numOfEnemies = 12;
                miniMapCamera.position = new Vector3(0f, 77f, 0f);
                break;
        }
    }

    public class CornerInfo{
        public Maze.Position position;
        public Vector2 initPosition, destPosition;  // 用於確定該點生成的Enemy類巡邏點

        public CornerInfo (Maze.Position position, Vector2 initPosition, Vector2 destPosition)
        {
            this.position = position;
            this.initPosition = initPosition;
            this.destPosition = destPosition;
        }
    }

    // Generate the map array
    private void GenerateMapArray(bool[,,] mazeArray)
    {
        // Init the map array
        mapArray = new int[mapSize[0], mapSize[1], mapSize[2]];
        // Generate the map array
        for (int x = 0; x < row; x++)
        {
            for (int y = 0; y < col; y++)
            {
                for (int i = 1; i < scale - 1; i++)
                {
                    for (int j = 1; j < scale - 1; j++)
                    {
                        for (int h = 1; h < height; h++)
                        {
                            mapArray[x * scale + i, h, y * scale + j] = 1;  // 中間挖空
                        }
                    }
                }
                if (x == row - 1 && y == col - 1)
                {
                    for (int i = 1; i < scale - 1; i++)
                    {
                        for (int j = 1; j < scale - 1; j++)
                        {
                            mapArray[x * scale + i, 0, y * scale + j] = 2;  // [row - 1, col - 1] 放置終點方塊
                        }
                    }
                }

                // 挖通相連的相鄰方格
                int count = 0;
                Direction direction = Direction.Left;
                if (mazeArray[x, y, 0])
                {
                    count++;
                    direction = Direction.Left;
                    for (int i = 1; i < scale - 1; i++)
                    {
                        for (int h = 1; h < height; h++)
                        {
                            mapArray[x * scale + i, h, y * scale] = 1;
                        }
                    }
                }
                if (mazeArray[x, y, 1])
                {
                    count++;
                    direction = Direction.Right;
                    for (int i = 1; i < scale - 1; i++)
                    {
                        for (int h = 1; h < height; h++)
                        {
                            mapArray[x * scale + i, h, (y + 1) * scale - 1] = 1;
                        }
                    }
                }
                if (mazeArray[x, y, 2])
                {
                    count++;
                    direction = Direction.Up;
                    for (int j = 1; j < scale - 1; j++)
                    {
                        for (int h = 1; h < height; h++)
                        {
                            mapArray[x * scale, h, y * scale + j] = 1;
                        }
                    }
                }
                if (mazeArray[x, y, 3])
                {
                    count++;
                    direction = Direction.Down;
                    for (int j = 1; j < scale - 1; j++)
                    {
                        for (int h = 1; h < height; h++)
                        {
                            mapArray[(x + 1) * scale - 1, h, y * scale + j] = 1;
                        }
                    }
                }
                // 生成cornerInfo
                if (count == 1 && (x != 0 || y != 0) && (x != row - 1 || y != col - 1))
                {
                    int dest_x = x, dest_y = y;
                    switch (direction)
                    {
                        case Direction.Left:
                            while (dest_y > 0 && mazeArray[x, dest_y, 0])
                            {
                                dest_y--;
                            }
                            break;
                        case Direction.Right:
                            while (dest_y < col - 1 && mazeArray[x, dest_y, 1])
                            {
                                dest_y++;
                            }
                            break;
                        case Direction.Up:
                            while (dest_x > 0 && mazeArray[dest_x, y, 2]) 
                            { 
                                dest_x--;
                            }
                            break;
                        case Direction.Down:
                            while (dest_x < row - 1 && mazeArray[dest_x, y, 3])
                            {
                                dest_x++;
                            }
                            break;
                    }
                    corners.Add(new CornerInfo(new Maze.Position(x, y), SetPosition(x, y), SetPosition(dest_x, dest_y)));
                }
            }
        }
        Debug.Log("Number of Corner: " + corners.Count);
    }

    private void GeneratePlayer()   // 生成玩家
    {
        GameObject playerObject = Instantiate(playerPrefab, playerInitPosition, Quaternion.identity);
        playerObject.name = "Player";
        playerObject.transform.tag = "Player";
        playerObject.layer = map.layer;
        playerObject.GetComponent<PlayerMovement>().playerInitPosition = playerInitPosition;
    }

    private void GenerateKeyAndEnemy (Maze i_maze) // 生成鑰匙和敵人
    {
        corners.Sort(delegate (CornerInfo c1, CornerInfo c2)  // 根據角落跟初始點的距離排序
        { 
            return i_maze.dist[c1.position.x, c1.position.y].CompareTo(i_maze.dist[c2.position.x, c2.position.y]); 
        });
        List <CornerInfo> used = new List<CornerInfo>();
        for (int i = 0; i < numOfKeys; i++)
        {
            bool flag = true;
            while (flag)
            {
                CornerInfo i_cornerInfo = corners[Random.Range(corners.Count / 2, corners.Count)];  // 鑰匙生成在較遠的角落
                if (!used.Contains(i_cornerInfo))
                {
                    used.Add(i_cornerInfo);
                    flag = false;

                    GameObject keyObject = Instantiate(keyPrefab, new Vector3(i_cornerInfo.initPosition.x, 1.5f, i_cornerInfo.initPosition.y), keyPrefab.transform.rotation);
                    keyObject.transform.parent = keys.transform;
                    keyObject.layer = map.layer;
                    keyObject.name = "Key_" + i;
                }
            }
        }
        numOfEnemies = Mathf.Min(numOfEnemies, corners.Count - numOfKeys - 1);  // 避免敵人數量超過可生成的位置數量
        Debug.Log("Number of Enemies: " + numOfEnemies);
        for (int j = 0; j < numOfEnemies; j++)
        {
            bool flag = true;
            while (flag)
            {
                CornerInfo i_cornerInfo = corners[Random.Range(1, corners.Count)];  // 敵人不會巡邏到玩家的初始點
                if (!used.Contains(i_cornerInfo))
                {
                    used.Add(i_cornerInfo);
                    flag = false;
                    Vector2 initPosition = new Vector3(i_cornerInfo.initPosition.x, i_cornerInfo.initPosition.y);
                    GameObject enemyObject = Instantiate(enemyPrefab, new Vector3(i_cornerInfo.initPosition.x, 1f, i_cornerInfo.initPosition.y), Quaternion.identity);
                    enemyObject.transform.parent = enemies.transform;
                    enemyObject.layer = map.layer;
                    enemyObject.name = "Enemy_" + j;

                    Enemy enemy = enemyObject.GetComponent<Enemy>();
                    enemy.initPosition = initPosition;
                    enemy.destPosition = new Vector3(i_cornerInfo.destPosition.x, i_cornerInfo.destPosition.y);
                }
            }
        }
    }

    private Vector2 SetPosition(int x, int y)   // 迷宮數組對應場景中的XZ軸方位
    {
        return new Vector2(x * scale + (scale - 1) / 2f, y * scale + (scale - 1) / 2f);
    }
}
