using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelChange : MonoBehaviour
{
    public string sceneName;
    // Start is called before the first frame update

    public Canvas currentCanvas;
    public Canvas Canvas1;
    public Canvas Canvas2;
    public Canvas Canvas3;
    public Canvas Canvas4;
    public Canvas Canvas5;
    void Start(){
        Canvas1.enabled = true;
        Canvas2.enabled = false;
        Canvas3.enabled = false;
        Canvas4.enabled = false;
        Canvas5.enabled = false;
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

    public void changeCanvas2()
    {
        currentCanvas.enabled = false;
        Canvas2.enabled = true;
        currentCanvas = Canvas2;
    }

    public void changeCanvas3()
    {
        currentCanvas.enabled = false;
        Canvas3.enabled = true;
        currentCanvas = Canvas3;
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

    public void changeCanvas5()
    {
        currentCanvas.enabled = false;
        Canvas5.enabled = true;
        currentCanvas = Canvas5;
    }
}
