using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �S���F���@Minimap�V�X�e��
/// </summary>
public class CS_Minimap : MonoBehaviour
{
    //Gizmos�ŕ\������
    [SerializeField, Header("�X�^�[�g�n�_")]
    private Vector3 StartPosition;

    [SerializeField, Header("�S�[���n�_")]
    private Vector3 EndPosition;

    [SerializeField, Header("�v���C���[�ʒu")]
    private Transform PlayerTrans;

    [SerializeField, Header("Minimap�v���C���[�ʒu")]
    private RectTransform MiniPlayerPin;

    [SerializeField, Header("Minmap�\���͈�")]
    private RectTransform MinimapSize;

    [SerializeField, Header("����V�X�e���ʒu")]
    private Transform BreakObjTrans;

    [SerializeField, Header("���󒆈ʒu")]
    private RectTransform BreakNowPos;

    [SerializeField, Header("����ψʒu")]
    private RectTransform BreakedPos;

    private float BreakUImaxsize = 0;

    // Start is called before the first frame update
    void Start()
    {
        // �v���C���[�̌��݂�Y���W�������ɕϊ�
        float progress = Mathf.InverseLerp(StartPosition.y, EndPosition.y, PlayerTrans.position.y);

        // Minimap��̈ʒu��ݒ�
        Vector2 markerPosition = MinimapSize.rect.min + new Vector2(0, MinimapSize.rect.height * progress);
        MiniPlayerPin.anchoredPosition = markerPosition;

        //����UI�̃T�C�Y��ݒ�
        BreakUImaxsize = BreakNowPos.sizeDelta.y;
        BreakNowPos.sizeDelta = new Vector2(BreakNowPos.sizeDelta.x, 0f);
        BreakedPos.sizeDelta = new Vector2(BreakedPos.sizeDelta.x, 0f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // �v���C���[�̌��݂�Y���W�������ɕϊ�
        float progress = Mathf.InverseLerp(StartPosition.y - 10f, EndPosition.y, PlayerTrans.position.y);

        // Minimap��̈ʒu��ݒ�
        Vector2 markerPosition = MinimapSize.rect.min + new Vector2(0, MinimapSize.rect.height * progress);
        MiniPlayerPin.anchoredPosition = markerPosition;

        // �v���C���[�̌��݂�Y���W�������ɕϊ�
        float Breakprogress = Mathf.InverseLerp(StartPosition.y, EndPosition.y, PlayerTrans.position.y);
        // UI�T�C�Y��i���ɍ��킹�Đݒ�
        float currentSize = BreakUImaxsize * Breakprogress;
        BreakNowPos.sizeDelta = new Vector2(BreakNowPos.sizeDelta.x, currentSize); // UI�̍�����ω�

    }

    /// <summary>
    /// Ray��\��
    /// </summary>
    private void OnDrawGizmos()
    {
        //�J�n�ʒu
        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(StartPosition, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero,Vector3.one);

        //�I���ʒu
        Gizmos.color = Color.blue;
        Gizmos.matrix = Matrix4x4.TRS(EndPosition, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);

    }

}
