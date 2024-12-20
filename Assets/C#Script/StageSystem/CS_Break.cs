using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("�m�F�p......")]
    [SerializeField]
    private BREAKSTATE CurrentBreakState;   //���݂̕�����
    [SerializeField]
    private bool StopBreak = false;     //���󂵂Ă��邩
    [SerializeField]
    private int CurrentBreakAreaNum = 0;    //���݂̕���G���A�ԍ�

    private int PlayerOnBreakAtraNum = 0;   //�v���C���[�������Ă���G���A

    [SerializeField, Header("���󂷂�G���A�����Ԃɓo�^")]
    private List<GameObject> BreakArea;

    public void SetBreakList(List<GameObject> list) { BreakArea = list; }

    [Header("----------------------------------------------------")]


    [SerializeField, Header("����X�s�[�h")]
    private float BreakSpeed = 1.0f;
    [SerializeField, Header("���󎞊ԊԊu")]
    private float BreakTime = 5.0f;
    [SerializeField, Header("�A���[�g��\�����Ă������܂ł̎���")]
    private float AlartTime = 3.0f;
    [SerializeField, Header("�A���[�g�̕\�����鍂��")]
    private float ArartHeight = 2.4f;
    [SerializeField, Header("�A���[�g���ǂꂾ�������ɗ��邩")]
    private float ArartInnerOffset = 1.0f;
    [SerializeField, Header("���󉉏oUI�\���ő勗��")]
    private int BreakEffectMaxDistance = 5;

    [Header("==============�T�����i�L�P��==============")]
    [SerializeField, Header("����A���[�gPrefab")]
    private GameObject BreakAlartBord;
    [SerializeField, Header("�A���[�g���oUI")]
    private Image ArartEffectUI;
    //�A���[�g�֌W
    private float EffectAlpha = 0f;
    private bool AlphaChange = false;

    [SerializeField, Header("���󂷂�I�u�W�F�N�g��Layer")]
    private LayerMask breakLayer;

    [SerializeField, Header("Player��Transform")]
    private Transform PlayerTrans;
  
    private float BreakTimeMesure = 0.0f;   //���󎞊Ԍv���p
   
    private GameObject CurrentAlartObj;     //���ݍĐ����̃A���[�gObj


    /// <summary>
    /// ������~������
    /// </summary>
    /// <param true�Œ�~false�ŉ���="stopflg"></param>
    public void ArartStop(bool stopflg)
    {
        StopBreak = stopflg;
        //��~
        if (stopflg)
        {
            ArartEffectUI.enabled = false;
            if (CurrentAlartObj == null) { return; }
            //�A���[�g��Animator���擾���Ĉꎞ��~
            Animator anim;
            CurrentAlartObj.TryGetComponent<Animator>(out anim);
            anim.speed = 0;
        }
        //�Đ�
        else
        {
            ArartEffectUI.enabled = true;
            if (CurrentAlartObj == null) { return; }
            //�A���[�g��Animator���擾���čĐ�
            Animator anim;
            CurrentAlartObj.TryGetComponent<Animator>(out anim);
            anim.speed = 1;
        }
    }

    //���݂̕���G���A�ԍ����擾
    public int GetBreakAreaDistance() => PlayerOnBreakAtraNum - CurrentBreakAreaNum;


    /// <summary>
    /// �G���A�̔ԍ����擾
    /// </summary>
    /// <param BreakArea�I�u�W�F�N�g="breakarea"></param>
    /// <returns></returns>
    public int GetBreakAreaNum(GameObject breakarea)
    {
        return BreakArea.IndexOf(breakarea);
    }

    // Start is called before the first frame update
    void Start()
    {
        CurrentBreakState = BREAKSTATE.NONE; 
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //��~���͍X�V���Ȃ�
        if (StopBreak) 
        {
            return; 
        }

        //�v���C���[�����Ă�G���A���擾
        {
            int OldDistance = GetBreakAreaDistance();

            //��������Ray���΂��ď�Get
            Vector3 Pos = PlayerTrans.position;
            Pos.y += 1f;
            Ray ray = new Ray(Pos, Vector3.down);
            RaycastHit[] hits = Physics.RaycastAll(ray, 5f, breakLayer);
            bool AreaHit = hits.Length > 0 && hits[0].collider.transform.childCount > 0;

            if (AreaHit)
            {

                PlayerOnBreakAtraNum = GetBreakAreaNum(hits[0].collider.transform.GetChild(0).gameObject);
            }

            //�A���[�gUI�̃��l�ҏW
            ChangeAlpha();
        }

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

    private void ChangeAlpha()
    {
        //�ő勗����菬�������
        int BreakAreaDistance = GetBreakAreaDistance();
        bool ViewEffect = BreakEffectMaxDistance >= BreakAreaDistance;

        if (ViewEffect)
        {
            float alpha = 1 - ((float)BreakAreaDistance / (float)BreakEffectMaxDistance);

            //���̒l�𒼐ڕύX�A�傫���ɂ���đ��x��ς���
            if (!AlphaChange) { EffectAlpha += (alpha / 1f) * Time.deltaTime; }
            else { EffectAlpha -= (alpha / 1f) * Time.deltaTime; }
            if (EffectAlpha >= alpha) { AlphaChange = true; }
            if(EffectAlpha <= 0) { AlphaChange = false; }

            ArartEffectUI.color = new Color(1, 0, 0, EffectAlpha);
        }
        else
        {
            ArartEffectUI.color = new Color(1, 0, 0, 0);
        }

    }

    /// <summary>
    /// �j��V�X�e��
    /// </summary>
    /// <param �j��G���A��Transform="areatrans"></param>
    private void BreakStage(Transform areatrans)
    {
        RaycastHit[] hits = Physics.BoxCastAll(areatrans.position, areatrans.localScale * 0.5f, Vector3.one, areatrans.rotation, 1f, breakLayer);
        if (hits.Length <= 0) { return; }

        hits[0].transform.TryGetComponent<Renderer>(out Renderer renderer);

        RaycastHit[] Area = Physics.BoxCastAll(renderer.bounds.center, renderer.bounds.size * 0.1f, Vector3.one, areatrans.rotation, 1f, breakLayer);

        if(Area.Length <= 0) { return; }

        //�Փ˂����I�u�W�F�N�g�S�Ă�j��
        for (int i = 0; i< Area.Length;i++)
        {

            SimplestarGame.VoronoiFragmenter voronoi;
            //�K�i�̏����A�q�I�u�W�F�N�g�ɂȂ��Ă���̂�
            if (Area[i].collider.gameObject.name == "Collider")
            {
                Area[i].transform.parent.TryGetComponent<SimplestarGame.VoronoiFragmenter>(out voronoi);
                Destroy(Area[i].collider.gameObject);
            }
            else
            {
                Area[i].collider.TryGetComponent<SimplestarGame.VoronoiFragmenter>(out voronoi);
            }

            Area[i].point = renderer.bounds.center;
            
            if (voronoi) { voronoi.Fragment(Area[i]); }
            //else { Debug.Log(hits[i].transform.name); }
        }

    }

    /// <summary>
    /// �A���[�g�̐���
    /// </summary>
    /// <param �j��G���A��Transform="areatrans"></param>
    private void CreateAlart(Transform areatrans)
    {
        RaycastHit[] hits = Physics.BoxCastAll(areatrans.position, areatrans.localScale * 0.5f, Vector3.one, areatrans.rotation, 1f, breakLayer);
        if (hits.Length <= 0) { return; }

        hits[0].transform.TryGetComponent<Renderer>(out Renderer renderer);
        Vector3 HalfScale = renderer.bounds.size * 0.5f;

        Vector3[] ArartPos = new Vector3[]
        {
            new Vector3(0f                               , ArartHeight,-(HalfScale.z - ArartInnerOffset)), // ��
            new Vector3(-(HalfScale.x - ArartInnerOffset), ArartHeight,0f                               ), // �E
            new Vector3(0f                               , ArartHeight,HalfScale.z - ArartInnerOffset   ), // ��
            new Vector3(HalfScale.x - ArartInnerOffset   , ArartHeight,0f                               )  // ��
        };

        // �e�R�[�i�[�̃��[���h���W���v�Z
        Vector3[] WorldCorners = new Vector3[ArartPos.Length];
        for (int i = 0; i < ArartPos.Length; i++)
        {
            Vector3 Pos = renderer.bounds.center;
            WorldCorners[i] = Pos + ArartPos[i];
        }

        //Debug.Log("4���̍��W" + "\n����" + WorldCorners[0] + "\n����" + WorldCorners[1] + "\n�E��" + WorldCorners[2] + "\n�E��" + WorldCorners[3]);

        //�A���[�g�̐���
        CurrentAlartObj = Instantiate(BreakAlartBord);
        
        //�A���[�g�̈ʒu��ݒ�
        for(int i=0;i<CurrentAlartObj.transform.childCount;i++)
        {
            Transform childtrans = CurrentAlartObj.transform.GetChild(i).transform;
            childtrans.position = WorldCorners[i];
            childtrans.localScale = new Vector3(renderer.bounds.size.x * 0.08f, childtrans.localScale.y, childtrans.localScale.x * 0.08f);
            //�A���[�g�̃T�C�Y�{��0.025f
        }

        //CurrentAlartObj.transform.position = areatrans.position;

        //�A���[�g��Animator���擾���Đݒ�A�Đ�����
        Animator anim;
        CurrentAlartObj.TryGetComponent<Animator>(out anim);
        anim.SetBool("Alart",true);
        anim.SetFloat("AlartTime", AlartTime);
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(BreakArea.Count > 0)Gizmos.DrawCube(BreakArea[CurrentBreakAreaNum].transform.position, BreakArea[CurrentBreakAreaNum].transform.localScale);
    }

}
