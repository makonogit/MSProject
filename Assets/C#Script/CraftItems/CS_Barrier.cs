using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityEngine;

// �N���t�g�A�C�e���@�o���A
// �����V�S
public class CS_Barrier : CraftItemBase
{
    // �ݒ荀��
    [SerializeField, Header("�L������")]
    private float validityTime = 3f;
    [SerializeField, Header("�ړ����x")]
    private float speed = 3f;


    // �ݒu�ʒu�m�F�p�I�u�W�F�N�g
    private GameObject setup;
    MeshRenderer[] meshRenderer;

    // �o���A�̃��f��
    private GameObject domeshield;

    // �I�[�f�B�I
    private CS_SoundEffect soundEffect;

    // ���Ԍv��
    private CS_Countdown countdown;

    // �R���W����
    Collider collider;

    // �ݒu���
    private bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();

        // �R���W�������擾
        collider = GetComponent<Collider>();
        collider.enabled = false;

        // ���Ԍv���p�I�u�W�F�N�g���쐬
        countdown = gameObject.AddComponent<CS_Countdown>();

        // �I�[�f�B�I���擾
        soundEffect = transform.GetChild(1).gameObject.GetComponent<CS_SoundEffect>();

        // �q�I�u�W�F�N�g���擾
        domeshield = transform.GetChild(0).gameObject;
        domeshield.SetActive(false);

        // �ݒu�ʒu��\��

        // "Player"�I�u�W�F�N�g��Transform���擾
        GameObject player = GameObject.Find("Player");
        Transform playerTransform = player.transform;

        // �q�I�u�W�F�N�g"setup"���������Ď擾
        setup = playerTransform.Find("setup").gameObject;
        meshRenderer = setup.GetComponentsInChildren<MeshRenderer>(true); ;
        foreach (MeshRenderer renderer in meshRenderer)
        {
            renderer.enabled = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isActive)
        {
            Vector3 moveVec = GetMovementVector();
            setup.transform.position = new Vector3(
                setup.transform.position.x + moveVec.x * speed * Time.deltaTime,
                setup.transform.position.y,
                setup.transform.position.z + moveVec.z * speed * Time.deltaTime
            );
        }


        // R�g���K�[�Őݒu
        if (GetInputSystem().GetRightTrigger() > 0
            && !isActive)
        {
            transform.position = setup.transform.position;
            countdown.Initialize(validityTime);
            domeshield.SetActive(true);
            setup.SetActive(false);
            isActive = true;
            soundEffect.PlaySoundEffect(0, 0);
            foreach (MeshRenderer renderer in meshRenderer)
            {
                renderer.enabled = false;
            }

            isSetUp = true;
        }

        // �g�p��A�����j��
        if (countdown.IsCountdownFinished()
            && isActive)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // �I�u�W�F�N�g�ɃA�^�b�`����Ă���S�ẴR���C�_�[���擾
        Collider[] colliders = GetComponents<Collider>();

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Attack"))
            {
                Destroy(other.gameObject);
            }
        }
    }

    //**
    //* �X�e�B�b�N���͂���J�������猩���ړ��x�N�g�����擾����
    //*
    //* in�F����
    //* out�F�ړ��x�N�g��
    //**
    Vector3 GetMovementVector()
    {
        Vector2 stick = GetInputSystem().GetRightStick();
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        Vector3 moveVec = forward * stick.y + right * stick.x;
        moveVec.y = 0f; // y ���̈ړ��͕s�v
        return moveVec.normalized;
    }
}
