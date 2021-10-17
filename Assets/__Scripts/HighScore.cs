using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScore : MonoBehaviour
{
    private int high;

    public HighScore S;

    private void Start()
    {
        if (PlayerPrefs.HasKey("HighScore"))
        {
            high = PlayerPrefs.GetInt("HighScore");
        }
        PlayerPrefs.SetInt("HighScore", high);
        
        S = this;

    }

    // Update is called once per frame
    void Update()
    {

        if (high > Score.S.score)
            S.GetComponent<Text>().text = high.ToString();
        else
        {
            S.GetComponent<Text>().text = Score.S.score.ToString();
            PlayerPrefs.SetInt("HighScore", Score.S.score);
            PlayerPrefs.Save();
        }


    }
}
