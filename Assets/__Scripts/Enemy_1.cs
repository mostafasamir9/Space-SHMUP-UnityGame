using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enemy_1 extends the Enemy class
public class Enemy_1 : Enemy
{
    //Because Enemy_1 extends Enemy, the _____ bool won't work
    //The same way in the Inspector pane //

    //# seconds for a full sine wave

    public float waveFrequency = 2;
    //Sine wave width in meters

    public float waveWidth = 4;
    public float waveRotY = 45;

    private float x0 = -12345; // the initial x value of pos
    private float birthTime;

    void Start()
    {
        x0 = pos.x;
        birthTime = Time.time;
    }

    //Override the move function on Enemy
    public override void Move()
    {
        //Because pos is a property , you can't directly set pos.x
        //So get the pos as an editable Vector3
        Vector3 tempPos = pos;
        //Theta adjusts based on time
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;

        //rotate a bit about Y
        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);

        //base.Move( still handles the movement down in y;
        base.Move();
    }
}
