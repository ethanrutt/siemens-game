using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PipeGameOverManager : MonoBehaviour
{
    public TMP_Text timeElapsed;

    private string score;

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
        // uploadTime();
        SceneManager.LoadScene("PipeGame");
    }

    public void ExitButton()
    {
        // uploadTime();
        SceneManager.LoadScene("Laboratory_Main");
    }
}
