using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ドアを開閉する
 * 
 * 担当：藤原昂祐
 */
public class CS_Door : MonoBehaviour
{
    [SerializeField, Header("開閉状態")]
    private bool isOpen = false;
    public void Open() { isOpen = true; }
    public void Close() { isOpen = false; }
    [Header("アニメーター")]
    [SerializeField, Header("R")]
    private Animator animatorDoorR;
    [SerializeField, Header("L")]
    private Animator animatorDoorL;

    // Start is called before the first frame update
    void Start()
    {
        // アニメーションを停止
        animatorDoorR.speed = 0f;
        animatorDoorL.speed = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isOpen)
        {
            animatorDoorR.speed = 1f;
            animatorDoorL.speed = 1f;
        }
    }
}
