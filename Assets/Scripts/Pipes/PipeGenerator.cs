using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        {new PipeInfo(Direction.up, PipeType.turn), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight), new PipeInfo(Direction.up, PipeType.straight)}
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
            case PipeType.straight:
                return spawnLocations[i, j];
        }

        return Vector3.zero;
    }

    void GenerateLevel(PipeInfo[,] currLevel)
    {
        for (int i = 0; i < currLevel.GetLength(0); i++)
        {
            for (int j = 0; j < currLevel.GetLength(1); j++)
            {
                Quaternion currRotation = possibleRotations[rand.Next(3)];

                switch (currLevel[i,j].type)
                {
                    case PipeType.straight:
                        Instantiate(straightPipe, getSpawnLocation(PipeType.straight, i, j), currRotation);
                        break;
                    case PipeType.turn:
                        Instantiate(turnPipe, getSpawnLocation(PipeType.turn, i, j), currRotation);
                        break;
                    case PipeType.source:
                        Instantiate(source, getSpawnLocation(PipeType.source, i, j), source.transform.rotation);
                        break;
                    case PipeType.sink:
                        Instantiate(sink, getSpawnLocation(PipeType.sink, i, j), sink.transform.rotation);
                        break;
                }
            }
        }

    }
}
