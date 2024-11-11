using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class PipePrefabsTest : MonoBehaviour
{
    private string sinkPrefabPath = "Assets/Prefabs/Pipes/Sink.prefab";
    private string sourcePrefabPath = "Assets/Prefabs/Pipes/Source.prefab";
    private string straightPipePrefabPath = "Assets/Prefabs/Pipes/StraightPipe.prefab";
    private string turnPipePrefabPath = "Assets/Prefabs/Pipes/TurnPipe.prefab";

    [Test]
    public void PipePrefabSimplePasses()
    {
        GameObject sinkPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(sinkPrefabPath);
        GameObject sourcePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePrefabPath);
        GameObject straightPipePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(straightPipePrefabPath);
        GameObject turnPipePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(turnPipePrefabPath);

        Assert.IsNotNull(sinkPrefab, "sinkPrefab is not at " + sinkPrefabPath);
        Assert.IsNotNull(sourcePrefab, "sourcePrefab is not at " + sourcePrefabPath);
        Assert.IsNotNull(straightPipePrefab, "straightPipePrefab is not at " + straightPipePrefabPath);
        Assert.IsNotNull(turnPipePrefab, "turnPipePrefab is not at " + turnPipePrefabPath);
    }

    [Test]
    public void PipePrefabInstantiation()
    {
        GameObject sinkPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(sinkPrefabPath);
        GameObject sourcePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePrefabPath);
        GameObject straightPipePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(straightPipePrefabPath);
        GameObject turnPipePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(turnPipePrefabPath);

        GameObject sink = GameObject.Instantiate(sinkPrefab, new Vector3(0, 0, 0), sinkPrefab.transform.rotation);
        GameObject source = GameObject.Instantiate(sourcePrefab, new Vector3(0, 0, 0), sourcePrefab.transform.rotation);
        GameObject straight = GameObject.Instantiate(straightPipePrefab, new Vector3(0, 0, 0), straightPipePrefab.transform.rotation);
        GameObject turn = GameObject.Instantiate(turnPipePrefab, new Vector3(0, 0, 0), turnPipePrefab.transform.rotation);

        Assert.IsNotNull(sink, "sinkPrefab not instantiated properly");
        Assert.IsNotNull(source, "sourcePrefab not instantiated properly");
        Assert.IsNotNull(straight, "straightPrefab not instantiated properly");
        Assert.IsNotNull(turn, "turnPipePrefab not instantiated properly");
    }
}
