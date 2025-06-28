using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject roomPrefab;
    public GameObject hallPrefab;
    public GameObject noHallPrefab;
    public int roomSize = 15;
    public int hallSize = 10;

    public int level = 0;
    private int heightNow = 0;
    private int levelWidth = 5;
    private int levelHeight = 3;

    Dictionary<Vector2Int, GameObject> roomObject = new Dictionary<Vector2Int, GameObject>();
    Dictionary<Vector3Int, GameObject> hallObject = new Dictionary<Vector3Int, GameObject>();
    Dictionary<Vector2Int, List<Vector2Int>> roomGraph = new Dictionary<Vector2Int, List<Vector2Int>>();
    Dictionary<Vector2Int, Vector2Int> dsuPar = new Dictionary<Vector2Int, Vector2Int>();

    void Start()
    {
        GenerateLevel();
    }

    void Update()
    {
        
    }

    public void GenerateLevel()
    {
        level++;
        dsuPar.Clear();
        for(int i = -levelWidth/2; i <= levelWidth / 2; i++)
        {
            for (int j = heightNow; j < heightNow + levelHeight; j++)
            {
                dsuPar[new Vector2Int(i, j)] = new Vector2Int(i, j);
                roomObject[new Vector2Int(i, j)] = Instantiate(roomPrefab, RoomPosition(new Vector2Int(i, j)), Quaternion.identity);
            }
        }
        List<Vector3Int> hallIdxList = new List<Vector3Int>();
        for (int i = -levelWidth / 2; i <= levelWidth / 2; i++)
        {
            for (int j = heightNow; j < heightNow + levelHeight; j++)
            {
                if (i > -levelWidth / 2) hallIdxList.Add(new Vector3Int(i, j, 0));
                if (j > heightNow) hallIdxList.Add(new Vector3Int(i, j, 1));
            }
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
            }
        }
        int hallLeft = levelWidth * levelHeight / 5;
        foreach (Vector3Int hallIdx in hallIdxList)
        {
            if (!hallObject.ContainsKey(hallIdx))
            {
                if (hallLeft >= 0)
                {
                    if (hallIdx.z == 0) hallObject[hallIdx] = Instantiate(hallPrefab, HallPosition(hallIdx), Quaternion.identity);
                    else if (hallIdx.z == 1) hallObject[hallIdx] = Instantiate(hallPrefab, HallPosition(hallIdx), Quaternion.Euler(0, 0, 90));
                    hallLeft--;
                }
                else
                {
                    if (hallIdx.z == 0) hallObject[hallIdx] = Instantiate(noHallPrefab, HallPosition(hallIdx), Quaternion.identity);
                    else if (hallIdx.z == 1) hallObject[hallIdx] = Instantiate(noHallPrefab, HallPosition(hallIdx), Quaternion.Euler(0, 0, 90));
                    hallLeft--;
                }
            }
        }

        heightNow += levelHeight;
    }

    Vector3 RoomPosition(Vector2Int idx)
    {
        return new Vector3(idx.x * (roomSize+hallSize), idx.y * (roomSize + hallSize), 0f);
    }

    Vector3 HallPosition(Vector3Int idx)
    {
        if(idx.z == 0) return new Vector3(idx.x * (roomSize + hallSize) - 12.5f, idx.y * (roomSize + hallSize), 0f);
        else return new Vector3(idx.x * (roomSize + hallSize), idx.y * (roomSize + hallSize) - 12.5f, 0f);
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
}
