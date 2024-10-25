using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//**
//* アイテム使用処理
//*
//* 担当：藤原昂祐
//**

public class CSP_UseItem : ActionBase
{
    [Header("パラメーター設定")]
    [SerializeField, Header("回復状態")]
    private bool isUse = false;
    [SerializeField, Header("経過時間")]
    private float elapsedTime = 0f;
    [SerializeField, Header("回復にかかる時間")]
    private float targetTime = 10f;
    [SerializeField, Header("1つあたりの回復量")]
    private float maxRecovery = 10f;

    // 体力の初期値を保存
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
    //* アイテム使用処理
    //*
    //* in：無し
    //* out：無し
    //**
    void HandleUseItem()
    {
        bool isItemStock = GetPlayerManager().GetItemStock() > 0;
        bool isDamage = GetPlayerManager().GetHP() <= initHp;

        if (GetInputSystem().GetButtonXPressed() && isItemStock && isDamage)
        {
            GetAnimator().SetBool("Recovery", true);

            // 経過時間をカウント
            elapsedTime += Time.deltaTime;

            // 体力を回復
            float hp = GetPlayerManager().GetHP();
            GetPlayerManager().SetHP(hp + (maxRecovery / targetTime) * Time.deltaTime);

            // 最大まで回復した場合、アイテムを消費する
            if(elapsedTime > targetTime)
            {
                // アイテムを消費
                int itemStock = GetPlayerManager().GetItemStock();
                GetPlayerManager().SetItemStock(itemStock - 1);

                // 使用済みアイテムを追加
                int ingredientsStock = GetPlayerManager().GetIngredientsStock();
                GetPlayerManager().SetIngredientsStock(ingredientsStock + 1);

                // カウントをリセット
                elapsedTime = 0f;
            }
        }
        else
        {
            GetAnimator().SetBool("Recovery", false);
        }
    }

}
