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
        // up
        Quaternion.Euler(0, 0, 0),
        // right
        Quaternion.Euler(0, 0, 90),
        // down
        Quaternion.Euler(0, 0, 180),
        // left
        Quaternion.Euler(0, 0, -90)
    };

    private Vector3[][] spawnLocations = new Vector3[][] {
        new Vector3[] {new Vector3(-7, 2, 0), new Vector3(-5, 2, 0), new Vector3(-3, 2, 0), new Vector3(-1, 2, 0), new Vector3(1, 2, 0), new Vector3(3, 2, 0), new Vector3(5, 2, 0), new Vector3(7, 2, 0)},
        new Vector3[] {new Vector3(-7, 0, 0), new Vector3(-5, 0, 0), new Vector3(-3, 0, 0), new Vector3(-1, 0, 0), new Vector3(1, 0, 0), new Vector3(3, 0, 0), new Vector3(5, 0, 0), new Vector3(7, 0, 0)}
    };

    // initialize explicitly
    private PipeInfo[][][] easyLevels = new PipeInfo[][][] {
        // level 1 straight line
        new PipeInfo[][] {
            emptyRow,
            new PipeInfo[] {new PipeInfo(Direction.right, PipeType.source), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.left, PipeType.sink)}
        },
        // level 1 one turn
        new PipeInfo[][] {
            new PipeInfo[] {new PipeInfo(Direction.right, PipeType.source), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.turn), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty)},
            new PipeInfo[] {new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.left, PipeType.sink)}
        }
    };

    private static PipeInfo[] emptyRow = new PipeInfo[] {new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty)};

    private PipeInfo[][] currentLevel;

    // Start is called before the first frame update
    void Start()
    {
        // have 3 categories of levels, easy, medium, hard
        // maybe have 3 levels for each, so 3 easy, 3 medium, 3 hard
        // randomly pick an easy level, then randomly pick a medium level, then randomly pick a hard level
        // probably have a "done" button that checks the solution and moves on to the next level
        GenerateLevel(easyLevels[1]);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /**
     * Since turnPipes have a 0.5 offset for alignment reasons, as well as
     * source and sink might cause issues later, so we will have this function
     * to get spawn position based on type
     *
     * for turn pipes, we also have to do some modifications to the
     * transform.position so that we are always aligned to the grid
     *
     * @see PipeBehavior.RotatePipe
     */
    Vector3 getSpawnLocation(PipeType type, int i, int j, int dir)
    {
        if (type == PipeType.turn) {
            Vector3 res = spawnLocations[i][j];
            res.x += 0.5f;
            switch (dir) {
                case 0:
                    res.y -= 1f;
                    break;
                case 2:
                    res.x -= 1f;
                    break;
                case 3:
                    res.y -= 1f;
                    break;
            }
            return res;
        }
        else {
            return spawnLocations[i][j];
        }
    }

    Quaternion getSpawnRotation(PipeInfo[][] currLevel, int i, int j)
    {
        Direction rotation = currLevel[i][j].direction;
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

    void InstantiatePipe(PipeInfo[][] currLevel, GameObject prefab, Vector3 spawn, Quaternion rotation, int row, int col, int dir, PipeType type)
    {
        Debug.Log($"instantiating pipe at {row}, {col} with direction {(Direction) dir}");
        GameObject pipe = Instantiate(prefab, spawn, rotation);
        PipeBehavior pipeBehavior = pipe.GetComponent<PipeBehavior>();
        pipeBehavior.gameState = currLevel;
        pipeBehavior.row = row;
        pipeBehavior.col = col;
        pipeBehavior.pipeInfo = new PipeInfo((Direction)dir, type);
        currLevel[row][col].direction = (Direction)dir;
    }

    void GenerateLevel(PipeInfo[][] currLevel)
    {
        currentLevel = currLevel;
        for (int i = 0; i < currLevel.Length; i++)
        {
            for (int j = 0; j < currLevel[i].Length; j++)
            {
                int dir = rand.Next(3);
                Quaternion currRotation = possibleRotations[dir];

                switch (currLevel[i][j].type)
                {
                    case PipeType.straight:
                        InstantiatePipe(currLevel, straightPipe, getSpawnLocation(PipeType.straight, i, j, dir), currRotation, i, j, dir, PipeType.straight);
                        break;
                    case PipeType.turn:
                        InstantiatePipe(currLevel, turnPipe, getSpawnLocation(PipeType.turn, i, j, dir), currRotation, i, j, dir, PipeType.turn);
                        break;
                    case PipeType.source:
                        Instantiate(source, getSpawnLocation(PipeType.source, i, j, 0), getSpawnRotation(currLevel, i, j));
                        break;
                    case PipeType.sink:
                        Instantiate(sink, getSpawnLocation(PipeType.sink, i, j, 0), getSpawnRotation(currLevel, i, j));
                        break;
                }
            }
        }
    }

    (int row, int col) GetSource(PipeInfo[][] currLevel)
    {
        for (int row = 0; row < currLevel.Length; row++)
        {
            for (int col = 0; col < currLevel[row].Length; col++)
            {
                if (currLevel[row][col].type == PipeType.source)
                {
                    return (row, col);
                }
            }
        }

        return (0, 0);
    }

    (int i, int j) DirectionToMove(PipeInfo[][] currLevel, int row, int col)
    {
        if (currLevel[row][col].type == PipeType.turn)
        {
            switch(currLevel[row][col].direction)
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
            switch(currLevel[row][col].direction)
            {
                case Direction.up:
                    return (row - 1, col);
                case Direction.right:
                    return (row, col + 1);
                case Direction.down:
                    return (row + 1, col);
                case Direction.left:
                    return (row, col - 1);
            }
        }
        return (0, 0);
    }

    public bool CheckSolution(PipeInfo[][] currLevel)
    {
        var (currRow, currCol) = GetSource(currLevel);
        Debug.Log($"source = ({currRow}, {currCol})");
        for (int i = 0; i < 100; i++)
        {
            Debug.Log($"curr location = ({currRow}, {currCol})");
            if (currLevel[currRow][currCol].type == PipeType.sink)
            {
                return true;
            }
            (currRow, currCol) = DirectionToMove(currLevel, currRow, currCol);
        }

        return false;
    }

    public void CheckSolutionButton()
    {
        if (CheckSolution(currentLevel))
        {
            Debug.Log("success");
        }
        else
        {
            Debug.Log("level failed");
        }
    }
}
