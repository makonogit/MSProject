using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_DummyCore : CraftItemBase
{
    [SerializeField, Header("”­Ë‘¬“x")]
    private float speed = 1f;

    // İ’uó‘Ô
    private bool isMove = true;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // ”­Ë‚µ‚Äİ’u
        if (isMove)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    // Õ“Ëˆ—
    private void OnTriggerEnter(Collider other)
    {
        isMove = false;

        isSetUp = true;
    }
}
