using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// �S���F���@�Z�[�u�V�X�e��
/// </summary>

/// <summary>
/// �ꎞ�ۑ��N���X(�V�[���Ԃ��܂����ϐ�)
/// </summary>
public static class TemporaryStorage
{

    private static Dictionary<string, int> intData = new Dictionary<string, int>();
   
    /// <summary>
    /// �f�[�^�̓o�^
    /// </summary>
    /// <param �L�[="name"></param>
    /// <param �l="value"></param>
    /// true:�����@false:���s
    public static bool DataRegistration(string name, int value)
    {
        bool addflg = intData.TryAdd(name, value);

        return addflg;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param �L�[="name"></param>
    /// <param �l="value"></param>
    /// true:���� false:���s<returns></returns>
    public static bool DataSave(string name, int value)
    {
        //�f�[�^�����邩���ׂ�
        bool data = intData.ContainsKey(name);

        //�Ȃ���ΏI��(���s)
        if (!data) { return false; }

        //�l�̕ۑ�
        intData[name] = value;
        return true;
    }

    /// <summary>
    /// �f�[�^�̎󂯎��
    /// </summary>
    /// <param �L�[="name"></param>
    /// -1:���s<returns></returns>
    public static int GetData(string name)
    {
        int value;
        bool data = intData.TryGetValue(name,out value);

        if (!data) { return -1; }

        return value;
    }


    /// <summary>
    /// �l�̍폜
    /// </summary>
    /// <param �L�[="name"></param>
    /// true:���� false:���s<returns></returns>
    public static bool DataDelete(string name)
    {
        bool data = intData.Remove(name);

        return data;
    }

}


/// <summary>
/// �Z�[�u�V�X�e��(�ۗ�)
/// </summary>
public static class SavaData
{
    //private static int CoreLife;
    //private static int StageLife;
    //private static int KanCorect;

    /// <summary>
    /// �Z�[�u
    /// </summary>
    /// <returns></returns>
    public static int Save()
    {

        return 0;
    }

}
