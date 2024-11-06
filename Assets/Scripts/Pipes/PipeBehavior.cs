using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeBehavior : MonoBehaviour
{
    public PipeInfo pipeInfo;

    private bool mouseDown;
    private bool movable;

    // variables to keep track of main game state
    public PipeInfo[][] gameState;
    public int row;
    public int col;

    private Vector3 rotate90 = new Vector3(0, 0, 90);
    private Vector3 rotateTurnPipe = new Vector3(0, 0, 270);

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

    // Start is called before the first frame update
    void Start()
    {
        // pipeInfo is populated in pipeGenerator
        mouseDown = false;
        movable = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseOver()
    {
        movable = true;
    }

    void OnMouseExit()
    {
        movable = false;
    }

    void OnMouseDown()
    {
        mouseDown = true;
    }

    void OnMouseUp()
    {
        if (!mouseDown || !movable)
        {
            return;
        }
        mouseDown = false;
        RotatePipe();
    }

    void RotatePipe()
    {
        // goes to next clockwise direction in a cycle
        int dir = (((int)pipeInfo.direction) + 1) % 4;
        Direction newDir = ((Direction)dir);

        if (pipeInfo.type == PipeType.turn)
        {
            // when turning turnpipes, the direction they rotate has to go in the other way with how we defined Direction
            // The position changes handle the animations so that they stay in the same spot even after rotating
            switch (newDir)
            {
                case Direction.up:
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 1f, gameObject.transform.position.z);
                    gameObject.transform.Rotate(rotateTurnPipe);
                    break;
                case Direction.right:
                    gameObject.transform.Rotate(rotateTurnPipe);
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x - 1f, gameObject.transform.position.y, gameObject.transform.position.z);
                    break;
                case Direction.down:
                    gameObject.transform.Rotate(rotateTurnPipe);
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1f, gameObject.transform.position.z);
                    break;
                case Direction.left:
                    gameObject.transform.Rotate(rotateTurnPipe);
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x + 1f, gameObject.transform.position.y, gameObject.transform.position.z);
                    break;
            }
        }
        else
        {
            gameObject.transform.Rotate(rotate90);
        }

        Debug.Log($"going from direction {(int) pipeInfo.direction} to {dir}");
        pipeInfo.direction = newDir;
        gameState[row][col].direction = newDir;

        Debug.Log(gameState[row][col].direction);
    }
}
