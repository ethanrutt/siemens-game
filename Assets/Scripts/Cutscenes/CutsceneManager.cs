using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    // Grab the dialogueManager
    public DialogueManager_Cutscene dialogueManager;

    // Grab the GameManager
    public GameManager gameManager => GameManager.Instance;

    // Grab the dialogueIndex from the dialogueManager
    private int dialogueIndex => dialogueManager.dialogueIndex;

    // Grab the Finish-Intro panel and activate it
    [SerializeField] private GameObject finishIntroPanel;

    // Grab ModalBack and active it
    [SerializeField] private GameObject modalBack;
    
    // Start
    void Start()
    {
        // Start the cutscene
        dialogueManager.StartDialogue();
    }

    void Update()
    {
        if (dialogueManager.dialogueIndex == 24)
        {
            // Activate the finishIntroPanel
            finishIntroPanel.SetActive(true);
            // Activate the modalBack
            modalBack.SetActive(true);
        }
    }

    // Take to Tutorial Scene or Laboratory-L1
    public void toTutorialScene()
    {
        // Load the tutorial scene
        // -14.89,0.23 gameManager
        gameManager.ChangePlayerSpawnPosition(new Vector2(-14.89f, 0.23f));
        UnityEngine.SceneManagement.SceneManager.LoadScene("Tutorial");
    }

    // Take to Laboratory-L1
    public void toTownSquare()
    {
        // Load the laboratory-l1 scene
        gameManager.ChangePlayerSpawnPosition(new Vector2(-30.1f, 20.49f));
        UnityEngine.SceneManagement.SceneManager.LoadScene("Town_Square");
    }

    
}
