using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class WebRequestUtility
{
    public static void SendWebRequest(MonoBehaviour monoBehaviour, string url, string jsonData, System.Action<string> successCallback, System.Action<string> failCallback)
    {
        monoBehaviour.StartCoroutine(SendWebRequestCoroutine(url, jsonData, successCallback, failCallback));
    }

    public static void SendGetWebRequest(MonoBehaviour monobehavior, string url, System.Action<string> successCallback, System.Action<string> failCallback)
    {
        monobehavior.StartCoroutine(SendGetWebRequestCoroutine(url, successCallback, failCallback));
    }

    private static IEnumerator SendWebRequestCoroutine(string url, string jsonData, System.Action<string> successCallback, System.Action<string> failCallback)
    {
        byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("success!");
                Debug.Log("invoking callback");
                successCallback?.Invoke(webRequest.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + webRequest.error);
                failCallback?.Invoke("failed");
            }
        }
    }

    private static IEnumerator SendGetWebRequestCoroutine(string url, System.Action<string> successCallback, System.Action<string> failCallback)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "GET"))
        {
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("success!");
                Debug.Log("invoking callback");
                successCallback?.Invoke(webRequest.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + webRequest.error);
                failCallback?.Invoke("failed");
            }
        }
    }
}
