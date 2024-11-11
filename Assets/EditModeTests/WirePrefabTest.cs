using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class WirePrefabTest
{
    private string wireEntryPrefabPath = "Assets/Prefabs/Wires/WireEntry.prefab";
    private string wirePlugPrefabPath = "Assets/Prefabs/Wires/WirePlug.prefab";

    [Test]
    public void WirePrefabTestSimplePasses()
    {
        GameObject wireEntry = AssetDatabase.LoadAssetAtPath<GameObject>(wireEntryPrefabPath);
        GameObject wirePlug = AssetDatabase.LoadAssetAtPath<GameObject>(wirePlugPrefabPath);

        Assert.IsNotNull(wireEntry, "wireEntry is not at " + wireEntryPrefabPath);
        Assert.IsNotNull(wirePlug, "wirePlug is not at " + wirePlugPrefabPath);
    }

    [Test]
    public void WirePrefabInstantiation()
    {
        GameObject wireEntry = AssetDatabase.LoadAssetAtPath<GameObject>(wireEntryPrefabPath);
        GameObject wirePlug = AssetDatabase.LoadAssetAtPath<GameObject>(wirePlugPrefabPath);

        GameObject wire = GameObject.Instantiate(wireEntry, new Vector3(0, 0, 0), wireEntry.transform.rotation);
        GameObject plug = GameObject.Instantiate(wirePlug, new Vector3(0, 0, 0), wirePlug.transform.rotation);

        Assert.IsNotNull(wire, "wireEntry not instantiated properly");
        Assert.IsNotNull(plug, "wirePlug not instantiated properly");
    }
}
