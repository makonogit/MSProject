using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * �h�A���J����
 * 
 * �S���F�����V�S
 */
public class CS_Door : MonoBehaviour
{
    [SerializeField, Header("�J���")]
    private bool isOpen = false;
    public void Open() { isOpen = true; }
    public void Close() { isOpen = false; }
    [Header("�A�j���[�^�[")]
    [SerializeField, Header("R")]
    private Animator animatorDoorR;
    [SerializeField, Header("L")]
    private Animator animatorDoorL;

    // Start is called before the first frame update
    void Start()
    {
        // �A�j���[�V�������~
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
