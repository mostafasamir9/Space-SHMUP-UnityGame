using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 10f; // The speed in m/s
    public float fireRate = 0.3f; // Seconds/shot ( Unused )
    public float health = 10;
    public int score = 100; // Points earned for destroying this

    public bool ____________________;

    public Bounds bounds; // The bounds of this and its Childern
    public Vector3 boundsCenterOffset; // Dist of bounds.center from position

    // Start is called before the first frame update
    void Awake()
    {
        InvokeRepeating("CheckOffscreen", 0f, 2f);
    }


    void CheckOffscreen()
    {
        // If bounds are still their default value
        if (bounds.size == Vector3.zero)
        {
            // then set them
            bounds = Utils.CombineBoundsOfChildren(this.gameObject);
            //Also find the diff between bounds.center & transform.position
            boundsCenterOffset = bounds.center - transform.position;
        }

        // Every time, update the bounds to the current position
        bounds.center = transform.position + boundsCenterOffset;
        // Check to see whether the bounds are completely offscreen
        Vector3 off = Utils.ScreenBoundsCheck(bounds, BoundsTest.offScreen);
        if (off != Vector3.zero)
        {
            // If the enemy has gone off the bottom edge of the screen
            if (off.y < 0)
            {
                // then destroy it
                Destroy(this.gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    public Vector3 pos
    {
        get
        {
            return this.transform.position;
        }
        set
        {
            this.transform.position = value;
        }
    }
}
