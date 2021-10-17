using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public int score = 0;
    static public Score S;

    void Start()
    {
        S = this;
    }

    void Update()
    {
        S.GetComponent<Text>().text = score.ToString();
    }

    public void IncreaseScore(int num)
    {

        score += num;

        PlayerPrefs.SetInt("Score", score);
    }
}
