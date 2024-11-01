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
        // 0 -> 1 means y + 1
        // 1 -> 2 means x - 1
        // 2 -> 3 means y - 1
        // 3 -> 0 means x + 1
        // goes to next clockwise direction in a cycle
        int dir = (((int)pipeInfo.direction) + 1) % 4;

        if (pipeInfo.type == PipeType.turn) {
            switch (dir) {
                case 0:
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x + 1f, gameObject.transform.position.y, gameObject.transform.position.z);
                    break;
                case 1:
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1f, gameObject.transform.position.z);
                    break;
                case 2:
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x - 1f, gameObject.transform.position.y, gameObject.transform.position.z);
                    break;
                case 3:
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 1f, gameObject.transform.position.z);
                    break;
            }
        }

        Debug.Log($"going from direction {(int) pipeInfo.direction} to {dir}");
        pipeInfo.direction = ((Direction) dir);
        gameState[row][col].direction = ((Direction)dir);
        gameObject.transform.Rotate(rotate90);
        Debug.Log(gameState[row][col].direction);
    }
}
