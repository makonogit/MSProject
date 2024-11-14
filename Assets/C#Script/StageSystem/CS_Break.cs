using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �S��:���@����
/// </summary>
public class CS_Break : MonoBehaviour
{
    private enum BREAKSTATE
    {
        NONE = 0,
        ALART = 1,
        BREAK = 2,
    }

    [SerializeField]
    private BREAKSTATE CurrentBreakState;   //���݂̕�����
    [SerializeField]
    private int CurrentBreakAreaNum = 0;    //���݂̕���G���A�ԍ�


    [SerializeField, Header("���󂷂�G���A�����Ԃɓo�^")]
    private List<GameObject> BreakArea;

    [SerializeField, Header("����X�s�[�h")]
    private float BreakSpeed = 1.0f;
    [SerializeField, Header("���󎞊ԊԊu")]
    private float BreakTime = 5.0f;
    [SerializeField, Header("�A���[�g��\�����Ă������܂ł̎���")]
    private float AlartTime = 3.0f;

    [Header("==============�T�����i�L�P��==============")]
    [SerializeField, Header("����A���[�gPrefab")]
    private GameObject BreakAlartBord;
    [SerializeField, Header("���󂷂�I�u�W�F�N�g��Layer")]
    private LayerMask breakLayer;

    private float BreakTimeMesure = 0.0f;   //���󎞊Ԍv���p
    
   
    private GameObject CurrentAlartObj;     //���ݍĐ����̃A���[�gObj

    // Start is called before the first frame update
    void Start()
    {
        CurrentBreakState = BREAKSTATE.NONE;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //���Ԍv��
        BreakTimeMesure += Time.deltaTime * BreakSpeed;

        if(BreakTimeMesure < BreakTime) { return; }

        switch(CurrentBreakState)
        {
            case BREAKSTATE.NONE:
                //�A���[�g�𐶐�
                CreateAlart(BreakArea[CurrentBreakAreaNum].transform);
                CurrentBreakState = BREAKSTATE.ALART;
                break;
            case BREAKSTATE.ALART:
                //�Đ��I��������A���[�gObject��������̂ŏ����������
                bool AlartEnd = CurrentAlartObj == null;
                if (AlartEnd) { CurrentBreakState = BREAKSTATE.BREAK; }
                break;
            case BREAKSTATE.BREAK:
                BreakStage(BreakArea[CurrentBreakAreaNum].transform);
                //�������ƍX�V
                BreakTimeMesure = 0f;
                CurrentBreakAreaNum++;
                CurrentBreakState = BREAKSTATE.NONE;
                break;
        }
    }


    /// <summary>
    /// �j��V�X�e��
    /// </summary>
    /// <param �j��G���A��Transform="areatrans"></param>
    private void BreakStage(Transform areatrans)
    {
        RaycastHit[] hits = Physics.BoxCastAll(areatrans.position, areatrans.localScale * 0.5f, Vector3.one,areatrans.rotation, 1f, breakLayer);
        if(hits.Length <= 0) { return; }
        
        //�Փ˂����I�u�W�F�N�g�S�Ă�j��
        for(int i = 0; i< hits.Length;i++)
        {
            hits[i].collider.gameObject.SetActive(false);
            //SimplestarGame.VoronoiFragmenter voronoi;
            //hits[i].collider.TryGetComponent<SimplestarGame.VoronoiFragmenter>(out voronoi);
            //if (voronoi) { voronoi.Fragment(hits[i]); }
        }

    }

    /// <summary>
    /// �A���[�g�̐���
    /// </summary>
    /// <param �j��G���A��Transform="areatrans"></param>
    private void CreateAlart(Transform areatrans)
    {
        //�A���[�g�̐���
        CurrentAlartObj = Instantiate(BreakAlartBord);
        CurrentAlartObj.transform.position = areatrans.position;

        //�A���[�g��Animator���擾���Đݒ�A�Đ�����
        Animator anim;
        CurrentAlartObj.TryGetComponent<Animator>(out anim);
        anim.SetBool("Alart",true);
        anim.SetFloat("AlartTime", AlartTime);
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(BreakArea[CurrentBreakAreaNum].transform.position, BreakArea[CurrentBreakAreaNum].transform.localScale);
    }

}
