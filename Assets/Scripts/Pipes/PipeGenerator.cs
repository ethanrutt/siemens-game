using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PipeGenerator : MonoBehaviour
{
    public System.Random rand = new System.Random();

    public GameObject sourcePrefab;
    public GameObject sinkPrefab;
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
        {new PipeInfo(Direction.up, PipeType.turn), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight)}
    };

    // Start is called before the first frame update
    void Start()
    {
        // have 3 categories of levels, easy, medium, hard
        // maybe have 3 levels for each, so 3 easy, 3 medium, 3 hard
        // randomly pick an easy level, then randomly pick a medium level, then randomly pick a hard level
        // each level is a nxn matrix with - meaning straight pipe, l meaning turnpipe(starting position of these is in an l), s meaning source (start), and e meaning sink (end)
        // we will generate the level, and then when use colliders to see when 2 pipes are connected
        GenerateLevel();
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
                return new Vector3(spawnLocations[i, j].x + 0.5f, spawnLocations[i, j].y, spawnLocations[i, j].z);
            case PipeType.straight:
                return spawnLocations[i, j];
        }

        return Vector3.zero;
    }

    void GenerateLevel()
    {
        for (int i = 0; i < level.GetLength(0); i++)
        {
            for (int j = 0; j < level.GetLength(1); j++)
            {
                Quaternion currRotation = possibleRotations[rand.Next(3)];

                switch (level[i,j].type)
                {
                    case PipeType.straight:
                        Instantiate(straightPipe, getSpawnLocation(PipeType.straight, i, j), currRotation);
                        break;
                    case PipeType.turn:
                        Instantiate(turnPipe, getSpawnLocation(PipeType.turn, i, j), currRotation);
                        break;
                }
            }
        }

    }
}
