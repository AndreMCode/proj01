using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PistolAction : MonoBehaviour
{
    public float speed = 2f;
    public float maxZ = 0.1277681f;
    public float minZ = 0.06f;
    public float xPos = 0; // from insp., to lock shifting
    private bool slide = false;
    bool bounced = false;

    private int direction = -1;

    void Update()
    { // Translate pistol action per shot
        if (slide && !bounced)
        {
            transform.Translate(0, 0, direction * speed * Time.deltaTime);

            if (transform.localPosition.z < minZ)
            {
                direction = -direction;
                bounced = true;
            }
        }

        if (slide && bounced)
        {
            transform.Translate(0, 0, direction * 0.5f * speed * Time.deltaTime);

            if (bounced && transform.localPosition.z > maxZ)
            {
                slide = false;
                bounced = false;
                direction = -direction;
                transform.localPosition = new Vector3(xPos, transform.localPosition.y, maxZ);
            }
        }
    }

    public void Action()
    { // Apply one action
        slide = true;
    }
}
