using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//**
//* エネルギーコア
//*
//* 担当：藤原
//**
public class CS_EnergyCure : MonoBehaviour
{
    [Header("パラメーター設定")]
    [SerializeField, Header("エネルギー量の初期値")]
    private float initEnergy = 10;
    [SerializeField]
    private float nowEnergy;
    public float GetEnergy() => nowEnergy;
    public void SetEnergy(float val) { nowEnergy = val; }

    // Start is called before the first frame update
    void Start()
    {
        nowEnergy = initEnergy;
        TemporaryStorage.DataRegistration("Core", Mathf.FloorToInt(nowEnergy));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TemporaryStorage.DataSave("Core", Mathf.FloorToInt(nowEnergy));
    }
}
