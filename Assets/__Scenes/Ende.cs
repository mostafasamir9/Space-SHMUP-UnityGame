using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ende : MonoBehaviour
{

    public Text score;

    public Text highscore;
    void Awake()
    {
        score.text = PlayerPrefs.GetInt("Score").ToString();
        highscore.text = PlayerPrefs.GetInt("HighScore").ToString();

    }

    // Update is called once per frame
    public void OnClickTryAgain()
    {
        Application.LoadLevel("__Scene_0");

    }
    public void OnClickExit()
    {
        Application.Quit();
    }
}
