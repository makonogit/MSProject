using UnityEngine;
using System.Collections.Generic;
using static Unity.VisualScripting.Member;
using Unity.VisualScripting;
using System.Linq;
using static UnityEngine.UI.Image;
using UnityEditor.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;

public class Exchanger :EditorWindow
{
    [MenuItem("Editor/ObjectExchanger")]
    private static void ShowWindow() 
    {
        // 生成
        var window = GetWindow<Exchanger>("ObjectExchanger");
        window.Show();
    }
    
    private Object newObject;
    private List<GameObject> objList = new List<GameObject>();
    private int num;
    private string objectName = null;
    
    private bool status = true;
    private bool copyStatus = true;
    private bool positionFlag = true;
    private bool rotationFlag = true;
    private bool scaleFlag = true;
    private Vector2 scrollPos = Vector2.zero;

    /// <summary>
    /// GUIの表示
    /// </summary>
    private void OnGUI()
    {
        GUILayout.Space(10);
        newObject = EditorGUILayout.ObjectField("オブジェクト", newObject, typeof(GameObject), true);
        objectName = EditorGUILayout.TextField("付ける名前",objectName);
        GUILayout.Space(5);
        CopyStatusGUI();
        GUILayout.Space(10);
        SelectGameObjectsGUI();
        GUILayout.FlexibleSpace();
        ExchangeGUI();
        GUILayout.Space(10);
    }


    /// <summary>
    /// コピーする情報の設定
    /// </summary>
    private void CopyStatusGUI() 
    {
        copyStatus = EditorGUILayout.Foldout(copyStatus, "コピーする情報");
        if (copyStatus)
        {
            positionFlag = EditorGUILayout.Toggle("    Position", positionFlag);
            rotationFlag = EditorGUILayout.Toggle("    Rotation", rotationFlag);
            scaleFlag = EditorGUILayout.Toggle("    Scale", scaleFlag);
        }
    }

    /// <summary>
    /// 選択されたオブジェクトの表示
    /// </summary>
    private void SelectGameObjectsGUI() 
    {
        EditorGUILayout.BeginHorizontal();
        status = EditorGUILayout.Foldout(status, "選択されたオブジェクト");
        GUILayout.Label(Selection.gameObjects.Length.ToString());
        EditorGUILayout.EndHorizontal();
        if (status) DrawList();
    }

    /// <summary>
    /// オブジェクトリストの描画
    /// </summary>
    private void DrawList() 
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        foreach (GameObject obj in Selection.gameObjects) GUILayout.Label("    " + obj.name);
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// オブジェクトを置換するGUI
    /// </summary>
    private void ExchangeGUI() 
    {
        bool exchange = false;
        exchange = GUILayout.Button("Exchange");
        if (exchange) Exchanges();
    }

    /// <summary>
    /// 置換処理
    /// </summary>
    private void Exchanges()
    {
        bool isNullObjects = newObject == null;
        bool isNullSelectObjects = Selection.gameObjects.Length <= 0;
        if (isNullObjects) { Debug.LogWarning("Null Reference : GameObject is not set.\n オブジェクトが設定されていません。"); return; }
        if (isNullSelectObjects) { Debug.LogWarning("Null Reference : No object selected.\n 変更するオブジェクトが選択されていません。"); return; }


        // 新しいオブジェクト生成
        foreach (var(obj ,index) in Selection.gameObjects.Select(( value, index) =>(value,index))) CreateObject(obj, newObject,index);
        
        // 元のオブジェクトを消去
        foreach (GameObject obj in Selection.gameObjects) DestroyOldObject(obj);
            
        
    }

    /// <summary>
    /// 新しいオブジェクトを生成
    /// </summary>
    /// <param name="original">置換元オブジェクト</param>
    /// <param name="newObject">新しく変更するオブジェクト</param>
    /// <param name="index">配列番号</param>
    private void CreateObject(GameObject original,Object newObject,int index) 
    {


        if (ShouldInterruptExchange(original)) return;
        
        // 生成
        GameObject newObj =Instantiate(newObject)as GameObject;
        newObj.transform.position = GetPosition(original);      // 位置設定
        newObj.transform.rotation = GetRotate(original);        // 角度設定
        newObj.transform.localScale = GetLossyScale(original);  // サイズ設定
        newObj.transform.name = GetName(newObj,index);          // 名前設定
        newObj.transform.parent = original.transform.parent;    // 親オブジェの設定

        // オブジェクトを追加したことをUndoレジスタに記録
        Undo.RegisterCreatedObjectUndo(newObj, "Create New Object");
    }

    private void DestroyOldObject(GameObject oldObject) 
    {
        if (ShouldInterruptExchange(oldObject,true)) return;
        // 削除
        Undo.DestroyObjectImmediate(oldObject);
    }

    /// <summary>
    /// Getters
    /// フラグがオンになっていると参照する。
    /// </summary>
    /// <param name="original">参照元</param>
    /// <returns>参照元の値又は初期値</returns>
    private Vector3 GetPosition(GameObject original) => positionFlag ? original.transform.position : Vector3.zero;
    private Quaternion GetRotate(GameObject original) => rotationFlag ? original.transform.rotation : Quaternion.identity;
    private Vector3 GetLossyScale(GameObject original) => scaleFlag ? original.transform.lossyScale : Vector3.zero;

    /// <summary>
    /// 名前の取得
    /// 設定なしなら元の名前を返す
    /// </summary>
    /// <param name="original">名付けるオブジェクト</param>
    /// /// <param name="index">配列番号</param>
    /// <returns>文字列</returns>
    private string GetName(GameObject original,int index) => objectName.Length <= 0 ? original.name : objectName + GetNumber(index);
    
    /// <summary>
    /// 配列番号を付ける
    /// </summary>
    /// <param name="index">配列番号</param>
    /// <returns></returns>
    private string GetNumber(int index) => index > 0 ?  " (" + index.ToString() + ")" :"";

    /// <summary>
    /// 置換を中断するべきか
    /// </summary>
    /// <param name="original"></param>
    /// <returns></returns>
    private bool ShouldInterruptExchange(GameObject original)
    {
        if (HasPath(original)) 
        { 
            Debug.LogWarning("※置換できません。\n選択されたオブジェクトはファイルデータの可能性があります。\nCausingObject:"+original.name);
            return true;
        }
        if (IsPrefabRoot(original)) 
        { 
            Debug.LogWarning("※置換できません。\n選択されたオブジェクトは編集中のプレハブルートです。\nCausingObject:" + original.name);
            return true;
        }
        return false;
    }
    /// <summary>
    /// 置換を中断するべきか(ワーニングログなし)
    /// </summary>
    /// <param name="original"></param>
    /// <returns></returns>
    private bool ShouldInterruptExchange(GameObject original,bool flag)
    {
        if (HasPath(original)) return true;
        if (IsPrefabRoot(original)) return true;
        return false;
    }

    /// <summary>
    /// パスを持っているか
    /// </summary>
    /// <param name="original"></param>
    /// <returns></returns>
    private bool HasPath(GameObject original) 
    {
        string path = AssetDatabase.GetAssetPath(original);
        //パスを所持しているか
        return !string.IsNullOrEmpty(path);
    }

    /// <summary>
    /// 開いているプレハブのルートか
    /// </summary>
    /// <param name="original"></param>
    /// <returns></returns>
    private bool IsPrefabRoot(GameObject original) 
    {
        // 現在開いているプレハブ
        PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        bool isNotOpenPrefab = prefabStage == null;
        // プレハブを開いてない
        if (isNotOpenPrefab) return false;
        
        // 開いているプレハブルートのGameObjectを取得
        GameObject prefabRoot = prefabStage.prefabContentsRoot;

        // ルートと選択されたオブジェクトが同じかどうか
        return prefabRoot == original;
    }

}
#endif