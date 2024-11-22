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
    public Canvas desiredCanvas;
    //void Start()
    //{
    //    currentCanvas = GetComponent<Canvas>();
    //}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeScene()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void endGame() 
    {
        Application.Quit();
    }

    public void changeCanvas()
    {
        currentCanvas.enabled = false;
        desiredCanvas.enabled = true;  
    }



}
