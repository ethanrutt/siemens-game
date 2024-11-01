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
        Debug.Log($"going from direction {(int) pipeInfo.direction} to {dir}");
        pipeInfo.direction = ((Direction) dir);
        gameState[row][col].direction = ((Direction)dir);
        gameObject.transform.Rotate(rotate90);
        Debug.Log(gameState[row][col].direction);
    }
}
