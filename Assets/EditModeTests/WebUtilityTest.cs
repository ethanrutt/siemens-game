using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebUtilityTest
{
    private class DummyMonoBehavior : MonoBehaviour { }

    [Test]
    public void WebUtilityGet()
    {
        var bruh = new GameObject().AddComponent<DummyMonoBehavior>();

        string url = "https://g7fh351dz2.execute-api.us-east-1.amazonaws.com/default/GetAllItems";
        WebRequestUtility.SendGetWebRequest(bruh, url, null, null);
    }

    [Test]
    public void WebUtilityPost()
    {
        var bruh = new GameObject().AddComponent<DummyMonoBehavior>();

        string url = "https://g7fh351dz2.execute-api.us-east-1.amazonaws.com/default/Leaderboard";
        string jsonData = System.String.Format(@"{{
            ""game_id"": {0},
        }}", 5);
        WebRequestUtility.SendWebRequest(bruh, url, jsonData, null, null);
    }

    [Test]
    public void WebUtilityPostFail()
    {
        var bruh = new GameObject().AddComponent<DummyMonoBehavior>();

        string url = "asdf";
        string jsonData = System.String.Format(@"{{
            ""game_id"": {0},
        }}", 5);
        WebRequestUtility.SendWebRequest(bruh, url, jsonData, null, null);
    }

    [Test]
    public void WebUtilityGetFail()
    {
        var bruh = new GameObject().AddComponent<DummyMonoBehavior>();

        string url = "asdf";
        string jsonData = System.String.Format(@"{{
            ""game_id"": {0},
        }}", 5);
        WebRequestUtility.SendGetWebRequest(bruh, url, null, null);
    }
}
