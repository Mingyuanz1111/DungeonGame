using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject roomPrefab;
    public GameObject hallPrefab;
    public GameObject noHallPrefab;
    public GameObject railPrefab;
    public GameObject checkpointPrefab;
    public int roomSize = 15;
    public int hallSize = 10;

    public int level = 1;
    private int levelWidth = 5;
    private int prvLevelWidth = 1;

    private int totalHeight = 4;
    private int nextHeight = 3;
    private List<int> levelHeight = new List<int> { 4 };
    private int dungeonHeight = 4;

    public Base baseScript;
    public Transform fogTransform;
    public Transform lowerFogTransform;
    private Dictionary<Vector2Int, GameObject> roomObject = new();
    private Dictionary<Vector3Int, GameObject> hallObject = new();
    private Dictionary<Vector2Int, List<Vector2Int>> roomGraph = new();
    private Dictionary<Vector2Int, Vector2Int> dsuPar = new();

    private Vector2Int basePos = new Vector2Int(0, 0);
    private Vector2Int nextBasePos = new Vector2Int(0, 4);
    private Vector2 fogPos;
    private Vector2 lowerFogPos;

    void Awake()
    {
        roomObject[new Vector2Int(0, totalHeight - 1)] = null;
        AddHallToGraph(new Vector2Int(0, 0), new Vector2Int(0, 1));
        AddHallToGraph(new Vector2Int(0, 1), new Vector2Int(0, 2));
        AddHallToGraph(new Vector2Int(0, 2), new Vector2Int(0, 3));
        AddHallToGraph(new Vector2Int(0, 1), new Vector2Int(1, 1));
        AddHallToGraph(new Vector2Int(0, 2), new Vector2Int(-1, 2));
    }
    void Start()
    {
        /*levelWidth = 3;
        levelHeight = 2;
        GenerateLevel();
        levelHeight = 3;
        GenerateLevel();
        levelWidth = 5;
        levelHeight = 2;
        GenerateLevel();
        levelHeight = 3;
        GenerateLevel();*/
    }

    void Update()
    {
        /*if (!baseScript.isMoving)
        {
            GenerateLevel();
        }*/
        fogTransform.position = Vector3.MoveTowards(fogTransform.position, fogPos, Time.deltaTime * 40f);
        lowerFogTransform.position = Vector3.MoveTowards(lowerFogTransform.position, lowerFogPos, Time.deltaTime * 40f);
    }

    public void GenerateLevel()
    {
        level++;

        // Generate map

        dsuPar.Clear();
        for(int i = -levelWidth/2; i <= levelWidth / 2; i++)
        {
            for (int j = totalHeight; j < totalHeight + nextHeight; j++)
            {
                dsuPar[new Vector2Int(i, j)] = new Vector2Int(i, j);
                roomObject[new Vector2Int(i, j)] = Instantiate(roomPrefab, RoomPosition(new Vector2Int(i, j)), Quaternion.identity);
            }
        }
        List<Vector3Int> hallIdxList = new();
        int guaranteeHall = Random.Range(-prvLevelWidth / 2, -prvLevelWidth / 2 + 1);
        for (int i = -levelWidth / 2; i <= levelWidth / 2; i++)
        {
            Vector3Int hallIdx = new Vector3Int(i, totalHeight, 1);
            if (roomObject.ContainsKey(new Vector2Int(i, totalHeight - 1)) && (i == guaranteeHall || Random.Range(0, 3) == 0))
            {
                hallObject[hallIdx] = Instantiate(hallPrefab, HallPosition(hallIdx), Quaternion.Euler(0, 0, 90));
                AddHallToGraph(new Vector2Int(i, totalHeight - 1), new Vector2Int(i, totalHeight));
            }
            else
            {
                Instantiate(noHallPrefab, HallPosition(hallIdx), Quaternion.Euler(0, 0, 90));
            }
            for (int j = totalHeight; j < totalHeight + nextHeight; j++)
            {
                if (i > -levelWidth / 2) hallIdxList.Add(new Vector3Int(i, j, 0));
                if (j > totalHeight) hallIdxList.Add(new Vector3Int(i, j, 1));
            }
        }
        for (int i = totalHeight; i <= totalHeight + nextHeight; i++)
        {
            Instantiate(noHallPrefab, HallPosition(new Vector3Int(-levelWidth / 2, i, 0)), Quaternion.identity);
            Instantiate(noHallPrefab, HallPosition(new Vector3Int(levelWidth / 2 + 1, i, 0)), Quaternion.identity);
        }
        Shuffle(hallIdxList);

        foreach (Vector3Int hallIdx in hallIdxList)
        {
            Vector2Int u = (hallIdx.z == 0) ? new Vector2Int(hallIdx.x - 1, hallIdx.y) : new Vector2Int(hallIdx.x, hallIdx.y - 1);
            Vector2Int v = (hallIdx.z == 0) ? new Vector2Int(hallIdx.x, hallIdx.y) : new Vector2Int(hallIdx.x, hallIdx.y);
            if (DsuJoin(u, v))
            {
                if(hallIdx.z == 0) hallObject[hallIdx] = Instantiate(hallPrefab, HallPosition(hallIdx), Quaternion.identity);
                else if(hallIdx.z == 1) hallObject[hallIdx] = Instantiate(hallPrefab, HallPosition(hallIdx), Quaternion.Euler(0, 0, 90));
                AddHallToGraph(u, v);
            }
        }
        int hallLeft = levelWidth * nextHeight / 7;
        foreach (Vector3Int hallIdx in hallIdxList)
        {
            if (!hallObject.ContainsKey(hallIdx))
            {
                if (hallLeft >= 0)
                {
                    Vector2Int u = (hallIdx.z == 0) ? new Vector2Int(hallIdx.x - 1, hallIdx.y) : new Vector2Int(hallIdx.x, hallIdx.y - 1);
                    Vector2Int v = (hallIdx.z == 0) ? new Vector2Int(hallIdx.x, hallIdx.y) : new Vector2Int(hallIdx.x, hallIdx.y);
                    AddHallToGraph(u, v);
                    if (hallIdx.z == 0) hallObject[hallIdx] = Instantiate(hallPrefab, HallPosition(hallIdx), Quaternion.identity);
                    else if (hallIdx.z == 1) hallObject[hallIdx] = Instantiate(hallPrefab, HallPosition(hallIdx), Quaternion.Euler(0, 0, 90));
                    hallLeft--;
                }
                else
                {
                    if (hallIdx.z == 0) hallObject[hallIdx] = Instantiate(noHallPrefab, HallPosition(hallIdx), Quaternion.identity);
                    else if (hallIdx.z == 1) hallObject[hallIdx] = Instantiate(noHallPrefab, HallPosition(hallIdx), Quaternion.Euler(0, 0, 90));
                }
            }
        }

        // Generate path of base

        if (level > 2)
        {
            basePos = nextBasePos;
            nextBasePos = new Vector2Int(Random.Range(-levelWidth / 2, levelWidth / 2 + 1), Random.Range(totalHeight, totalHeight + nextHeight));
        }
        Instantiate(checkpointPrefab, RoomPosition(nextBasePos), Quaternion.identity);
        Debug.Log($"Level {level} : {nextBasePos}");

        RandomQueue<(Vector2Int,Vector2Int)> rdq = new();
        HashSet<Vector2Int> visitedRooms = new();
        Dictionary<Vector2Int, Vector2Int> previousRoom = new();
        rdq.Push((basePos, basePos));
        while (rdq.Count > 0)
        {
            (Vector2Int tp, Vector2Int prv) = rdq.Pop();
            if (visitedRooms.Contains(tp)) continue;
            visitedRooms.Add(tp);
            previousRoom[tp] = prv;
            if (tp == nextBasePos) break;
            foreach (Vector2Int v in roomGraph[tp])
            {
                if (v.y >= totalHeight - levelHeight[levelHeight.Count-1] && !visitedRooms.Contains(v)) rdq.Push((v, tp));
            }
        }

        baseScript.ResetPath();

        Vector2Int roomNow = nextBasePos;
        while (roomNow != previousRoom[roomNow])
        {
            baseScript.path.Add(RoomPosition(roomNow));
            Vector3Int hallIdx;
            Vector2 diff = roomNow - previousRoom[roomNow];
            if (diff.x == 1) hallIdx = new Vector3Int(roomNow.x, roomNow.y, 0);
            else if (diff.x == -1) hallIdx = new Vector3Int(previousRoom[roomNow].x, previousRoom[roomNow].y, 0);
            else if (diff.y == 1) hallIdx = new Vector3Int(roomNow.x, roomNow.y, 1);
            else  hallIdx = new Vector3Int(previousRoom[roomNow].x, previousRoom[roomNow].y, 1);
            Instantiate(railPrefab, HallPosition(hallIdx), Quaternion.Euler(0, 0, (diff.x != 0)?0:90));
            roomNow = previousRoom[roomNow];
        }
        baseScript.path.Reverse();

        baseScript.StartMove();

        prvLevelWidth = levelWidth;
        levelHeight.Add(nextHeight);
        totalHeight += levelHeight[levelHeight.Count - 1];
        dungeonHeight += levelHeight[levelHeight.Count - 1] - ((levelHeight.Count > 3) ? levelHeight[levelHeight.Count - 4] : 0);

        fogPos = RoomPosition(new Vector2Int(0, totalHeight));
        lowerFogPos = RoomPosition(new Vector2Int(0, totalHeight - dungeonHeight));
    }

    Vector2 RoomPosition(Vector2Int idx)
    {
        return new Vector3(idx.x * (roomSize+hallSize), idx.y * (roomSize + hallSize));
    }

    Vector2 HallPosition(Vector3Int idx)
    {
        if(idx.z == 0) return new Vector3(idx.x * (roomSize + hallSize) - 12.5f, idx.y * (roomSize + hallSize));
        else return new Vector3(idx.x * (roomSize + hallSize), idx.y * (roomSize + hallSize) - 12.5f);
    }

    void Shuffle<T>(List<T> list)
    {
        for(int i=0; i < list.Count; i++)
        {
            int rd = Random.Range(i, list.Count);
            (list[i], list[rd]) = (list[rd], list[i]);
        }
    }

    Vector2Int DsuRoot(Vector2Int nd)
    {
        return dsuPar[nd] = (dsuPar[nd] == nd) ? (nd) : DsuRoot(dsuPar[nd]);
    }
    bool DsuJoin(Vector2Int a, Vector2Int b)
    {
        Vector2Int aRoot = DsuRoot(a);
        Vector2Int bRoot = DsuRoot(b);
        if (aRoot != bRoot) {
            dsuPar[bRoot] = aRoot;
            return true;
        } else return false;
    }

    void AddHallToGraph(Vector2Int a, Vector2Int b) 
    {
        if (!roomGraph.ContainsKey(a)) roomGraph[a] = new List<Vector2Int>();
        if (!roomGraph.ContainsKey(b)) roomGraph[b] = new List<Vector2Int>();
        roomGraph[a].Add(b);
        roomGraph[b].Add(a);
    }

    public class RandomQueue<T>
    {
        private List<T> list = new();
        public int Count => list.Count;
        public void Push(T val)
        {
            list.Add(val);
        }
        public T Pop()
        {
            int rd = Random.Range(0, Count);
            (list[Count-1], list[rd]) = (list[rd], list[Count - 1]);
            T ret = list[Count - 1];
            list.RemoveAt(Count - 1);
            return ret;
        }
    }
}
