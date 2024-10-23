using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 担当：菅　セーブシステム
/// </summary>

/// <summary>
/// 一時保存クラス(シーン間をまたぐ変数)
/// </summary>
public static class TemporaryStorage
{

    private static Dictionary<string, int> intData = new Dictionary<string, int>();
   
    /// <summary>
    /// データの登録
    /// </summary>
    /// <param キー="name"></param>
    /// <param 値="value"></param>
    /// true:成功　false:失敗
    public static bool DataRegistration(string name, int value)
    {
        bool addflg = intData.TryAdd(name, value);

        return addflg;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param キー="name"></param>
    /// <param 値="value"></param>
    /// true:成功 false:失敗<returns></returns>
    public static bool DataSave(string name, int value)
    {
        //データがあるか調べる
        bool data = intData.ContainsKey(name);

        //なければ終了(失敗)
        if (!data) { return false; }

        //値の保存
        intData[name] = value;
        return true;
    }

    /// <summary>
    /// データの受け取り
    /// </summary>
    /// <param キー="name"></param>
    /// -1:失敗<returns></returns>
    public static int GetData(string name)
    {
        int value;
        bool data = intData.TryGetValue(name,out value);

        if (!data) { return -1; }

        return value;
    }


    /// <summary>
    /// 値の削除
    /// </summary>
    /// <param キー="name"></param>
    /// true:成功 false:失敗<returns></returns>
    public static bool DataDelete(string name)
    {
        bool data = intData.Remove(name);

        return data;
    }

}


/// <summary>
/// セーブシステム(保留)
/// </summary>
public static class SavaData
{
    //private static int CoreLife;
    //private static int StageLife;
    //private static int KanCorect;

    /// <summary>
    /// セーブ
    /// </summary>
    /// <returns></returns>
    public static int Save()
    {

        return 0;
    }

}
