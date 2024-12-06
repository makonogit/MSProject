using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_TextScaleDownOrUp : MonoBehaviour
{
    public float time, changeSpeed;
    public bool enlarge;

    void Start()
    {
        enlarge = true;
    }

    void Update()
    {
        changeSpeed = Time.deltaTime * 0.1f;

        if (time < 0)
        {
            enlarge = true;
        }
        if (time > 0.7f)
        {
            enlarge = false;
        }

        if (enlarge == true)
        {
            time += Time.deltaTime;
            transform.localScale += new Vector3(changeSpeed, changeSpeed, changeSpeed);
        }
        else
        {
            time -= Time.deltaTime;
            transform.localScale -= new Vector3(changeSpeed, changeSpeed, changeSpeed);
        }
    }
}
