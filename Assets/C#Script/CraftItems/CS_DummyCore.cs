using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_DummyCore : CraftItemBase
{
    [SerializeField, Header("���ˑ��x")]
    private float speed = 1f;

    // �ݒu���
    private bool isMove = true;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // ���˂��Đݒu
        if (isMove)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    // �Փˏ���
    private void OnTriggerEnter(Collider other)
    {
        isMove = false;

        isSetUp = true;
    }
}
