using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//**
//* �G�l���M�[�R�A
//*
//* �S���F����
//**
public class CS_EnergyCure : MonoBehaviour
{
    [Header("�p�����[�^�[�ݒ�")]
    [SerializeField, Header("�G�l���M�[�ʂ̏����l")]
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
