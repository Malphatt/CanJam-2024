using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartLevelChange : MonoBehaviour
{
    public string sceneName;
    // Start is called before the first frame update

    public Canvas currentCanvas;
    public Canvas Canvas1;
    public Canvas Canvas4;

    void Start()
    {
        Canvas1.enabled = true;

        Canvas4.enabled = false;
        currentCanvas = Canvas1;
    }


    public void changeScene()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void endGame()
    {
        Application.Quit();
    }

 
    public void changeCanvas4()
    {
        currentCanvas.enabled = false;
        Canvas4.enabled = true;
        currentCanvas = Canvas4;
    }

    public void changeCanvas1()
    {
        currentCanvas.enabled = false;
        Canvas1.enabled = true;
        currentCanvas = Canvas1;
    }

}
