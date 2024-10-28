using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class PipeGenerator : MonoBehaviour
{
    public System.Random rand = new System.Random();

    public GameObject source;
    public GameObject sink;
    public GameObject straightPipe;
    public GameObject turnPipe;

    private Quaternion[] possibleRotations = {
        Quaternion.Euler(0, 0, 0),
        Quaternion.Euler(0, 0, 90),
        Quaternion.Euler(0, 0, 180),
        Quaternion.Euler(0, 0, -90)
    };

    private Vector3[,] spawnLocations = {
        {new Vector3(-7, 0, 0), new Vector3(-5, 0, 0), new Vector3(-3, 0, 0), new Vector3(-1, 0, 0), new Vector3(1, 0, 0), new Vector3(3, 0, 0), new Vector3(5, 0, 0), new Vector3(7, 0, 0)}
    };
    // initialize explicitly
    private PipeInfo[,] level = {
        {new PipeInfo(Direction.right, PipeType.source), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.left, PipeType.sink)}
    };

    // Start is called before the first frame update
    void Start()
    {
        // have 3 categories of levels, easy, medium, hard
        // maybe have 3 levels for each, so 3 easy, 3 medium, 3 hard
        // randomly pick an easy level, then randomly pick a medium level, then randomly pick a hard level
        // probably have a "done" button that checks the solution and moves on to the next level
        GenerateLevel(level);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /**
     * Since turnPipes have a 0.5 offset for alignment reasons, as well as
     * source and sink might cause issues later, so we will have this function
     * to get spawn position based on type
     */
    Vector3 getSpawnLocation(PipeType type, int i, int j)
    {
        switch (type)
        {
            case PipeType.turn:
                Vector3 res = spawnLocations[i, j];
                res.x += 0.5f;
                return res;
            default:
                return spawnLocations[i, j];
        }

        return Vector3.zero;
    }

    Quaternion getSpawnRotation(PipeInfo[,] currLevel, int i, int j)
    {
        Direction rotation = currLevel[i, j].direction;
        switch (rotation)
        {
            case Direction.up:
                return possibleRotations[0];
            case Direction.right:
                return possibleRotations[1];
            case Direction.down:
                return possibleRotations[2];
            case Direction.left:
                return possibleRotations[3];
        }
        return possibleRotations[0];
    }

    void InstantiatePipe(PipeInfo[,] currLevel, GameObject prefab, Vector3 spawn, Quaternion rotation, int row, int col, int dir)
    {
        GameObject pipe = Instantiate(prefab, spawn, rotation);
        PipeBehavior pipeInfo = pipe.GetComponent<PipeBehavior>();
        pipeInfo.gameState = currLevel;
        pipeInfo.row = row;
        pipeInfo.col = col;
        currLevel[row, col].direction = (Direction)dir;
    }

    void GenerateLevel(PipeInfo[,] currLevel)
    {
        for (int i = 0; i < currLevel.GetLength(0); i++)
        {
            for (int j = 0; j < currLevel.GetLength(1); j++)
            {
                int dir = rand.Next(3);
                Quaternion currRotation = possibleRotations[dir];

                switch (currLevel[i,j].type)
                {
                    case PipeType.straight:
                        InstantiatePipe(currLevel, straightPipe, getSpawnLocation(PipeType.straight, i, j), currRotation, i, j, dir);
                        break;
                    case PipeType.turn:
                        currLevel[i, j].direction = (Direction) dir;
                        Instantiate(turnPipe, getSpawnLocation(PipeType.turn, i, j), currRotation);
                        break;
                    case PipeType.source:
                        Instantiate(source, getSpawnLocation(PipeType.source, i, j), getSpawnRotation(currLevel, i, j));
                        break;
                    case PipeType.sink:
                        Instantiate(sink, getSpawnLocation(PipeType.sink, i, j), getSpawnRotation(currLevel, i, j));
                        break;
                }
            }
        }
    }

    (int row, int col) GetSource(PipeInfo[,] currLevel)
    {
        for (int row = 0; row < currLevel.GetLength(0); row++)
        {
            for (int col = 0; col < currLevel.GetLength(1); col++)
            {
                if (currLevel[row, col].type == PipeType.source)
                {
                    return (row, col);
                }
            }
        }

        return (0, 0);
    }

    (int i, int j) DirectionToMove(PipeInfo[,] currLevel, int row, int col)
    {
        if (currLevel[row, col].type == PipeType.turn)
        {
            switch(currLevel[row, col].direction)
            {
                case Direction.up:
                    return (row, col + 1);
                case Direction.right:
                    return (row + 1, col);
                case Direction.down:
                    return (row, col - 1);
                case Direction.left:
                    return (row - 1, col);
            }
        }
        else
        {
            Debug.Log("non turn pipe");
            switch(currLevel[row, col].direction)
            {
                case Direction.up:
                    Debug.Log("moving up");
                    return (row - 1, col);
                case Direction.right:
                    Debug.Log("moving right");
                    return (row, col + 1);
                case Direction.down:
                    Debug.Log("moving down");
                    return (row + 1, col);
                case Direction.left:
                    Debug.Log("moving left");
                    return (row, col - 1);
            }
        }
        return (0, 0);
    }

    public bool CheckSolution(PipeInfo[,] currLevel)
    {
        var (currRow, currCol) = GetSource(currLevel);
        Debug.Log($"source = ({currRow}, {currCol})");
        for (int i = 0; i < 100; i++)
        {
            Debug.Log($"curr location = ({currRow}, {currCol})");
            Debug.Log($"curr direction = {currLevel[currRow, currCol].direction}");
            if (currLevel[currRow, currCol].type == PipeType.sink)
            {
                return true;
            }
            (currRow, currCol) = DirectionToMove(currLevel, currRow, currCol);
        }

        return false;
    }

    public void CheckSolutionButton()
    {
        if (CheckSolution(level))
        {
            Debug.Log("success");
        }
        else
        {
            Debug.Log("level failed");
        }
    }
}
