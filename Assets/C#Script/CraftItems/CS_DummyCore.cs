using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_DummyCore : CraftItemBase
{
    [SerializeField, Header("�L������")]
    private float validityTime = 3f;
    [SerializeField, Header("���ˑ��x")]
    private float speed = 1f;

    // ���Ԍv��
    private CS_Countdown countdown;

    // �ݒu���
    private bool isMove = true;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();

        // ���Ԍv���p�I�u�W�F�N�g���쐬
        countdown = gameObject.AddComponent<CS_Countdown>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // ���˂��Đݒu
        if (isMove)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }

        // �g�p��A�����j��
        if (countdown.IsCountdownFinished()
            && !isMove)
        {
            Destroy(gameObject);
        }
    }

    // �Փˏ���
    private void OnTriggerEnter(Collider other)
    {
        isMove = false;

        isSetUp = true;

        countdown.Initialize(validityTime);
    }
}
