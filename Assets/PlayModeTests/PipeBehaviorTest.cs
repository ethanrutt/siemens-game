using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class PipeBehaviorTest : MonoBehaviour
{
    private string turnPipePrefabPath = "Assets/Prefabs/Pipes/TurnPipe.prefab";
    private string straightPipePrefabPath = "Assets/Prefabs/Pipes/StraightPipe.prefab";
    private string sourcePipePrefabPath = "Assets/Prefabs/Pipes/Source.prefab";
    private string sinkPipePrefabPath = "Assets/Prefabs/Pipes/Sink.prefab";

    private GameObject turnPipe;
    private GameObject straightPipe;
    private GameObject sourcePipe;
    private GameObject sinkPipe;

    private GameObject turn;
    private GameObject straight;
    private GameObject source;
    private GameObject sink;

    private void SimulateClick(GameObject target)
    {
        target.SendMessage("OnMouseOver", SendMessageOptions.DontRequireReceiver);
        target.SendMessage("OnMouseDown", SendMessageOptions.DontRequireReceiver);
    }

    private void SimulateRelease(GameObject target)
    {
        target.SendMessage("OnMouseUp", SendMessageOptions.DontRequireReceiver);
        target.SendMessage("OnMouseExit", SendMessageOptions.DontRequireReceiver);
    }

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        Debug.Log("setting up");
        SceneManager.LoadScene("PipeGame");
        yield return null;

        PipeInfo[][] sampleGameState = new PipeInfo[][] {
            new PipeInfo[] {new PipeInfo(Direction.right, PipeType.turn), new PipeInfo(Direction.right, PipeType.straight)}
        };

        turnPipe = AssetDatabase.LoadAssetAtPath<GameObject>(turnPipePrefabPath);
        straightPipe = AssetDatabase.LoadAssetAtPath<GameObject>(straightPipePrefabPath);
        sourcePipe = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePipePrefabPath);
        sinkPipe = AssetDatabase.LoadAssetAtPath<GameObject>(sinkPipePrefabPath);

        Assert.IsNotNull(turnPipe, "turnPipe was not able to be loaded");
        Assert.IsNotNull(straightPipe, "straightPipe was not able to be loaded");
        Assert.IsNotNull(sourcePipe, "sourcePipe was not able to be loaded");
        Assert.IsNotNull(sinkPipe, "sinkPipe was not able to be loaded");

        turn = GameObject.Instantiate(
                turnPipe,
                new Vector3(-5, 5, 0),
                turnPipe.transform.rotation
        );
        PipeBehavior turnBehavior = turn.GetComponent<PipeBehavior>();
        turnBehavior.pipeInfo = new PipeInfo(Direction.up, PipeType.turn);
        turnBehavior.gameState = sampleGameState;
        turnBehavior.row = 0;
        turnBehavior.col = 0;

        straight = GameObject.Instantiate(
                straightPipe,
                new Vector3(5, 5, 0),
                straightPipe.transform.rotation
        );
        PipeBehavior straightBehavior = straight.GetComponent<PipeBehavior>();
        straightBehavior.pipeInfo = new PipeInfo(Direction.up, PipeType.straight);
        straightBehavior.gameState = sampleGameState;
        straightBehavior.row = 0;
        straightBehavior.col = 1;

        source = GameObject.Instantiate(
                sourcePipe,
                new Vector3(5, 5, 0),
                sourcePipe.transform.rotation
        );

        sink = GameObject.Instantiate(
                sinkPipe,
                new Vector3(5, 5, 0),
                sinkPipe.transform.rotation
        );

        Assert.IsNotNull(turn, "turn was not able to be instantiated");
        Assert.IsNotNull(straight, "straight was not able to be instantiated");
        Assert.IsNotNull(source, "source was not able to be instantiated");
        Assert.IsNotNull(sink, "sink was not able to be instantiated");

        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Debug.Log("tearing down");
        if (turn != null) GameObject.Destroy(turn);
        if (straight != null) GameObject.Destroy(straight);
        if (source != null) GameObject.Destroy(source);
        if (sink != null) GameObject.Destroy(sink);

        yield return null;
    }

    [UnityTest]
    public IEnumerator StraightPipeClick()
    {
        if (straight != null)
        {
            Quaternion initialRotation = straight.transform.rotation;

            SimulateClick(straight);

            yield return null;

            SimulateRelease(straight);

            yield return null;

            Assert.AreNotEqual(straight.transform.rotation, initialRotation);

        }
        else
        {
            Assert.Fail("straight is null");
        }
    }

    [UnityTest]
    public IEnumerator TurnPipeClick()
    {
        if (turn != null)
        {
            Quaternion initialRotation = turn.transform.rotation;

            SimulateClick(turn);

            yield return null;

            SimulateRelease(turn);

            yield return null;

            Assert.AreNotEqual(turn.transform.rotation, initialRotation);

        }
        else
        {
            Assert.Fail("turn is null");
        }
    }
}
