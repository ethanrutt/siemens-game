using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class WireConnectionTest : MonoBehaviour
{
    private string wireEntryPrefabPath = "Assets/Prefabs/Wires/WireEntry.prefab";
    private string wirePlugPrefabPath = "Assets/Prefabs/Wires/WirePlug.prefab";

    private GameObject wireEntry;
    private GameObject wirePlug;

    private GameObject wire;
    private GameObject plug;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        Debug.Log("setting up");
        SceneManager.LoadScene("EmptyScene");
        yield return null;

        wireEntry = AssetDatabase.LoadAssetAtPath<GameObject>(wireEntryPrefabPath);
        wirePlug = AssetDatabase.LoadAssetAtPath<GameObject>(wirePlugPrefabPath);

        Assert.IsNotNull(wireEntry, "wireEntry was not able to be loaded");
        Assert.IsNotNull(wirePlug, "wirePlug was not able to be loaded");

        wire = GameObject.Instantiate(
                wireEntry,
                new Vector3(-5, 5, 0),
                wireEntry.transform.rotation
        );

        plug = GameObject.Instantiate(
                wirePlug,
                new Vector3(5, 5, 0),
                wirePlug.transform.rotation
        );

        Assert.IsNotNull(wire, "wire was not able to be instantiated");
        Assert.IsNotNull(plug, "plug was not able to be instantiated");
    }

    [UnityTest]
    public IEnumerator WireConnection()
    {
        SimulateClick(wire);

        yield return SimulateDragToPosition(wire, plug.transform.position);

        bool connected = plug.GetComponent<PlugStats>().connected;

        Assert.IsTrue(connected, "wire entry and wire plug are not successfully connected");
    }

    [UnityTest]
    public IEnumerator WireSnapBack()
    {
        Vector3 startPosition = wire.transform.position;

        SimulateClick(wire);

        yield return SimulateDragToPosition(wire, new Vector3(0, -5, 0));

        wire.SendMessage("OnMouseUp", SendMessageOptions.DontRequireReceiver);
        yield return null;

        Vector3 endingPosition = wire.transform.position;
    }

    private void SimulateClick(GameObject target)
    {
        target.SendMessage("OnMouseDown", SendMessageOptions.DontRequireReceiver);
    }

    private IEnumerator SimulateDragToPosition(GameObject draggedObject, Vector3 targetPosition)
    {
        float elapsed = 0;
        float duration = 0.5f;

        Vector3 draggedObjectPosition = draggedObject.transform.position;

        while (elapsed < duration)
        {
            draggedObject.transform.position = Vector3.Lerp(draggedObjectPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        draggedObject.transform.position = targetPosition;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Debug.Log("tearing down");
        if (wire != null) GameObject.Destroy(wire);
        if (plug != null) GameObject.Destroy(plug);

        yield return null;
    }
}
