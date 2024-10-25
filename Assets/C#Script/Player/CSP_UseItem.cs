using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//**
//* �A�C�e���g�p����
//*
//* �S���F�����V�S
//**

public class CSP_UseItem : ActionBase
{
    [Header("�p�����[�^�[�ݒ�")]
    [SerializeField, Header("�񕜏��")]
    private bool isUse = false;
    [SerializeField, Header("�o�ߎ���")]
    private float elapsedTime = 0f;
    [SerializeField, Header("�񕜂ɂ����鎞��")]
    private float targetTime = 10f;
    [SerializeField, Header("1������̉񕜗�")]
    private float maxRecovery = 10f;

    // �̗͂̏����l��ۑ�
    private float initHp;

    void Start()
    {
        base.Start();

        initHp = GetPlayerManager().GetHP();
    }

    void FixedUpdate()
    {
        HandleUseItem();
    }

    //**
    //* �A�C�e���g�p����
    //*
    //* in�F����
    //* out�F����
    //**
    void HandleUseItem()
    {
        bool isItemStock = GetPlayerManager().GetItemStock() > 0;
        bool isDamage = GetPlayerManager().GetHP() <= initHp;

        if (GetInputSystem().GetButtonXPressed() && isItemStock && isDamage)
        {
            GetAnimator().SetBool("Recovery", true);

            // �o�ߎ��Ԃ��J�E���g
            elapsedTime += Time.deltaTime;

            // �̗͂���
            float hp = GetPlayerManager().GetHP();
            GetPlayerManager().SetHP(hp + (maxRecovery / targetTime) * Time.deltaTime);

            // �ő�܂ŉ񕜂����ꍇ�A�A�C�e���������
            if(elapsedTime > targetTime)
            {
                // �A�C�e��������
                int itemStock = GetPlayerManager().GetItemStock();
                GetPlayerManager().SetItemStock(itemStock - 1);

                // �g�p�ς݃A�C�e����ǉ�
                int ingredientsStock = GetPlayerManager().GetIngredientsStock();
                GetPlayerManager().SetIngredientsStock(ingredientsStock + 1);

                // �J�E���g�����Z�b�g
                elapsedTime = 0f;
            }
        }
        else
        {
            GetAnimator().SetBool("Recovery", false);
        }
    }

}
