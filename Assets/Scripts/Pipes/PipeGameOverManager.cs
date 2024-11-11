using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PipeGameOverManager : MonoBehaviour
{
    public TMP_Text timeElapsed;

    private string score;

    private PlayerData playerData => PlayerData.Instance;

    public void Setup(System.TimeSpan timeSpan)
    {
        timeElapsed.text = "Time: " + System.String.Format("{0:00}:{1:00}.{2:00}",
            timeSpan.Minutes, timeSpan.Seconds,
            timeSpan.Milliseconds / 10);

        score = System.String.Format("{0}.{1}", timeSpan.Minutes * 60 + timeSpan.Seconds, timeSpan.Milliseconds / 10);

        gameObject.SetActive(true);
    }

    public void RestartButton()
    {
        // FIXME: use actual userId from playerData
        uploadTime(300);
        SceneManager.LoadScene("PipeGame");
    }

    public void ExitButton()
    {
        // FIXME: use actual userId from playerData
        uploadTime(300);
        SceneManager.LoadScene("Laboratory_Main");
    }

    /**
     * uploadTime() is a function that sends a POST request to the backend to upload the time
     */
    private void uploadTime(int userId)
    {
        string url = "https://g7fh351dz2.execute-api.us-east-1.amazonaws.com/default/ScoreUpload";
        string jsonData = System.String.Format(@"{{
            ""user_id"": {0},
            ""game_id"": {1},
            ""score"": {2}
        }}", userId, 5, score);
        Debug.Log("uploading score " + score);
        WebRequestUtility.SendWebRequest(this, url, jsonData, OnRequestComplete);
    }

    void OnRequestComplete(string responseText)
    {
        Debug.Log(responseText);
    }
}
