using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// �S���F���@�X�|�i�[�̏��
/// </summary>
public static class CS_SpawnerInfo
{
    static private Transform CoreTrans;
    static private CS_Core CoreState;
    static private Transform PlayerTrans;

    public static Transform GetCoreTrans() => CoreTrans;
    public static Transform GetPlayerTrans() => PlayerTrans;

    public static CS_Core GetCoreState() => CoreState;

    public static void SetCoreTrans(Transform trans) { CoreTrans = trans; }
    public static void SetPlayerTrans(Transform trans) { PlayerTrans = trans; }

    public static void SetCoreState(CS_Core core) { CoreState = core; }

}
