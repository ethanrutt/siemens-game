using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class PipeGenerator : MonoBehaviour
{
    public System.Random rand = new System.Random();

    public System.Diagnostics.Stopwatch gameTime = new System.Diagnostics.Stopwatch();

    public GameObject source;
    public GameObject sink;
    public GameObject straightPipe;
    public GameObject turnPipe;

    int level = 1;

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

    private static PipeInfo[] emptyRow = new PipeInfo[] {new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty)};

    private Vector3[][] spawnLocations = new Vector3[][] {
        new Vector3[] {new Vector3(-7, 4, 0), new Vector3(-5, 4, 0), new Vector3(-3, 4, 0), new Vector3(-1, 4, 0), new Vector3(1, 4, 0), new Vector3(3, 4, 0), new Vector3(5, 4, 0), new Vector3(7, 4, 0)},
        new Vector3[] {new Vector3(-7, 2, 0), new Vector3(-5, 2, 0), new Vector3(-3, 2, 0), new Vector3(-1, 2, 0), new Vector3(1, 2, 0), new Vector3(3, 2, 0), new Vector3(5, 2, 0), new Vector3(7, 2, 0)},
        new Vector3[] {new Vector3(-7, 0, 0), new Vector3(-5, 0, 0), new Vector3(-3, 0, 0), new Vector3(-1, 0, 0), new Vector3(1, 0, 0), new Vector3(3, 0, 0), new Vector3(5, 0, 0), new Vector3(7, 0, 0)},
        new Vector3[] {new Vector3(-7, -3, 0), new Vector3(-5, -3, 0), new Vector3(-3, -3, 0), new Vector3(-1, -3, 0), new Vector3(1, -3, 0), new Vector3(3, -2, 0), new Vector3(5, -3, 0), new Vector3(7, -3, 0)},
        new Vector3[] {new Vector3(-7, -4, 0), new Vector3(-5, -4, 0), new Vector3(-3, -4, 0), new Vector3(-1, -4, 0), new Vector3(1, -4, 0), new Vector3(3, -4, 0), new Vector3(5, -4, 0), new Vector3(7, -4, 0)}
    };

    private PipeInfo[][][] easyLevels = new PipeInfo[][][] {
        // level 1 straight line
        new PipeInfo[][] {
            emptyRow,
            new PipeInfo[] {new PipeInfo(Direction.right, PipeType.source), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.left, PipeType.sink)},
            emptyRow,
            emptyRow,
            emptyRow
        },
        // level 1 one turn
        new PipeInfo[][] {
            emptyRow,
            new PipeInfo[] {new PipeInfo(Direction.right, PipeType.source), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.turn), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty)},
            new PipeInfo[] {new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.left, PipeType.sink)},
            emptyRow,
            emptyRow
        }
    };

    private PipeInfo[][][] mediumLevels = new PipeInfo[][][] {
        // level 2 stairs down and then back
        new PipeInfo[][] {
            new PipeInfo[] {new PipeInfo(Direction.right, PipeType.source), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.turn), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty)},
            new PipeInfo[] {new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.turn), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty)},
            new PipeInfo[] {new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.turn), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty)},
            new PipeInfo[] {new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.right, PipeType.sink), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.turn), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty)},
            emptyRow
        },
        // level 2 sideways u shape
        new PipeInfo[][] {
            emptyRow,
            emptyRow,
            new PipeInfo[] {new PipeInfo(Direction.right, PipeType.source), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.turn), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty)},
            new PipeInfo[] {new PipeInfo(Direction.right, PipeType.sink), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.turn), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty)},
            emptyRow
        }
    };

    private PipeInfo[][][] hardLevels = new PipeInfo[][][] {
        // level 3 down then back
        new PipeInfo[][] {
            new PipeInfo[] {new PipeInfo(Direction.right, PipeType.source), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.turn), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty)},
            new PipeInfo[] {new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.turn), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty)},
            new PipeInfo[] {new PipeInfo(Direction.right, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.turn), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty)},
            new PipeInfo[] {new PipeInfo(Direction.right, PipeType.sink), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.turn), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty)},
            emptyRow
        }
    };

    private PipeInfo[][] currentLevel;

    private ArrayList gameObjects = new ArrayList();

    public PipeGameOverManager pipeGameOverManager;

    public Button checkSolutionButton;

    // Start is called before the first frame update
    void Start()
    {
        gameTime.Start();
        GenerateLevel(easyLevels[rand.Next(2)]);
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
                case 1:
                    res.x -= 1f;
                    res.y -= 1f;
                    break;
                case 2:
                    res.x -= 1f;
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
        GameObject pipe = Instantiate(prefab, spawn, rotation);
        PipeBehavior pipeBehavior = pipe.GetComponent<PipeBehavior>();
        pipeBehavior.gameState = currLevel;
        pipeBehavior.row = row;
        pipeBehavior.col = col;
        pipeBehavior.pipeInfo = new PipeInfo((Direction)dir, type);
        currLevel[row][col].direction = (Direction)dir;
        gameObjects.Add(pipe);

        // the sprites for right / left are flipped on straight pipes, make sure to handle this properly
        if ((dir == 1 || dir == 3) && type != PipeType.turn)
        {
            SpriteRenderer sr = pipe.GetComponent<SpriteRenderer>();
            sr.flipY = !sr.flipY;
        }
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
                        // with turnpipes, right is left and left is right in terms of animations
                        // this is due to the rotate 90 working differently for the shape i've defined
                        // this will handle whether it is left or right, flip it accordingly, and sync the direction in the gamestate
                        if (dir == 1)
                        {
                            InstantiatePipe(currLevel, turnPipe, getSpawnLocation(PipeType.turn, i, j, dir), possibleRotations[3], i, j, dir, PipeType.turn);
                        }
                        else if (dir == 3)
                        {
                            InstantiatePipe(currLevel, turnPipe, getSpawnLocation(PipeType.turn, i, j, dir), possibleRotations[1], i, j, dir, PipeType.turn);

                        }
                        else
                        {
                            InstantiatePipe(currLevel, turnPipe, getSpawnLocation(PipeType.turn, i, j, dir), currRotation, i, j, dir, PipeType.turn);
                        }
                        break;
                    case PipeType.source:
                        Quaternion rot = getSpawnRotation(currLevel, i, j);
                        GameObject bruh1 = Instantiate(source, getSpawnLocation(PipeType.source, i, j, 0), rot);

                        // the source has the same sprites as straight pipe, so we also need to flip if its right or left
                        if (rot == possibleRotations[1] || rot == possibleRotations[3])
                        {
                            SpriteRenderer sr = bruh1.GetComponent<SpriteRenderer>();
                            sr.flipY = !sr.flipY;
                        }

                        gameObjects.Add(bruh1);
                        break;
                    case PipeType.sink:
                        GameObject bruh2 = Instantiate(sink, getSpawnLocation(PipeType.sink, i, j, 0), getSpawnRotation(currLevel, i, j));
                        gameObjects.Add(bruh2);
                        break;
                }
            }
        }
    }

    void ClearLevel()
    {
        foreach (GameObject g in gameObjects)
        {
            Destroy(g);
        }
        gameObjects.Clear();
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
        for (int i = 0; i < 100; i++)
        {
            // sometimes we can go out of bounds, so if we do, just return false
            try
            {
                if (currLevel[currRow][currCol].type == PipeType.sink)
                {
                    return true;
                }
            }
            catch (System.Exception e)
            {
                return false;
            }
            (currRow, currCol) = DirectionToMove(currLevel, currRow, currCol);
        }

        return false;
    }

    public IEnumerator ChangeButtonColorOnFail()
    {
        Image buttonImage = checkSolutionButton.GetComponent<Image>();

        buttonImage.color = Color.red;

        yield return new WaitForSeconds(1);

        buttonImage.color = Color.white;

    }

    public void CheckSolutionButton()
    {
        if (CheckSolution(currentLevel))
        {
            ClearLevel();
            level++;
            if (level == 2)
            {
                GenerateLevel(mediumLevels[rand.Next(2)]);
            }
            else if (level == 3)
            {
                GenerateLevel(hardLevels[0]);
            }
            else
            {
                gameTime.Stop();
                pipeGameOverManager.Setup(gameTime.Elapsed);
            }
        }
        else
        {
            StartCoroutine(ChangeButtonColorOnFail());
        }
    }
}
