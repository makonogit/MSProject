using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Specialized;

public class CSP_Shot : ActionBase
{
    //[Header("�\������")]
    //[SerializeField, Header("�ҋ@���̕���")]
    //private GameObject weaponIdle;
    //[SerializeField, Header("�ˌ����̕���")]
    //private GameObject weaponShot;

    [Header("�ˌ��ݒ�")]
    [SerializeField, Header("��C�C�̒e�I�u�W�F�N�g")]
    private GameObject AirBall;// �e
    [SerializeField, Header("���U���̏����l")]
    private int initMagazine;// ���U���̏����l 
    [SerializeField, Header("���݂̑��U��")]
    private int magazine;// ���U��
    //[SerializeField, Header("1�g���K�[�̘A�ː�")]
    private int burstfire = 1;// �A�ː�
    //[SerializeField, Header("�c�e���̏����l")]
    private int initBulletStock;
    //[SerializeField, Header("���݂̎c�e��")]
    private int bulletStock;// �c�e��
    private bool isShot = false;// �ˌ���
    //[SerializeField, Header("���炷HP")]
    private float shotHp;
    [SerializeField, Header("�˒�����")]
    private float range = 100f;
    [SerializeField, Header("�U�e�͈�")]
    private float scatter = 0.1f;
    [SerializeField, Header("�I�[�g�G�C���L��")]
    private bool isAuto;
    public void SetAuto(bool flg) { isAuto = flg; }
    public bool GetAuto() => isAuto;
    [SerializeField, Header("�t���I�[�g�̔��ˊԊu")]
    private float interval = 0.5f;
    [SerializeField, Header("�����[�h�ɂ����鎞��")]
    private float intervalReload = 3f;

    [Header("���e�B�N���ݒ�")]
    [SerializeField, Header("�\���ʒu")]
    private UnityEngine.UI.Image detectionImage;
    private Vector2 defaultSize;
    //[SerializeField, Header("���e�B�N���ύX�̌��o�͈�")]
    //float radius = 1.0f;
    //[SerializeField, Header("�I�[�g�G�C�����e�B�N��")]
    private UnityEngine.UI.Image autoaim;
    //[SerializeField, Header("���o���̃��e�B�N���ݒ�")]
    private Sprite detectedSprite;
    //[SerializeField]
    //private Color detectedColor = Color.green; // ���o���̐F
    [SerializeField, Header("�ʏ펞�̃��e�B�N��")]
    private Sprite reticle;
    [SerializeField, Header("��p���̃��e�B�N��")]
    private Sprite ColdREticle;
    //[SerializeField, Header("�񌟏o���̃��e�B�N���ݒ�")]
    private Sprite notDetectedSprite; 
    //[SerializeField]
    //private Color notDetectedColor = Color.red; // �񌟏o���̐F
    [SerializeField, Header("�I�[�g�G�C���Ώۂ̃^�O")]
    private List<string> targetTag;
    private GameObject targetObject;// ���e�B�N�����̔j��\�I�u�W�F�N�g
    [SerializeField, Header("�I�[�o�[�q�[�g�Q�[�W")]
    private UnityEngine.UI.Image overheat;
    [SerializeField, Header("�I�[�o�[�q�[�g���Q�[�W")]
    private UnityEngine.UI.Image overheatOut;
    [SerializeField, Header("�I�[�o�[�q�[�g���e�L�X�g")]
    private TextMeshProUGUI overheatText;
    //[SerializeField, Header("���b�N�I�����e�B�N��")]
    private UnityEngine.UI.Image lockon;
    private RectTransform rectTransform;

    [Header("�I�[�g�G�C���ݒ�")]
    [SerializeField, Header("�I�[�g�G�C�����ʎ���")]
    private float autoTime = 10f;
    [SerializeField, Header("����Ԋu")]
    private float autoaimCheckInterval = 0.5f;
    private float nextAutoaimCheckTime = 0;
    [SerializeField, Header("�J�����^�[�Q�b�g")]
    private GameObject cameraTarget; 
    [SerializeField, Header("�G�C���^�[�Q�b�g")]
    private GameObject aimTarget;
    [SerializeField, Header("���m�͈�")]
    float radiusAuto = 10f;

    [Header("�U�����̐U���ݒ�")]
    [SerializeField, Header("�U���̒���")]
    private float duration = 0.5f;         // �U���̒���
    [SerializeField, Header("�U���̋���")]
    private int powerType = 1;          // �U���̋����i4�i�K�j
    [SerializeField, Header("�U���̎��g��")]
    private AnimationCurve curveType;          // �U���̎��g��
    [SerializeField, Header("�J��Ԃ���")]
    private int repetition = 1;         // �J��Ԃ���

    private bool Cold = false;  //��p�����ǂ���
    private float OverHeatAlpha = 0f;    //�I�[�o�[�q�[�g���̉摜�̓����x(���X�ɐԂ�����)

    // �J�E���g�_�E���p�N���X
    private CS_Countdown countdown;
    private CS_Countdown intervalCountdown;
    private CS_Countdown autoaimCountdown;

    protected override void Start()
    {
        base.Start();

        // �I�[�o�[�q�[�g�e�L�X�g���\��
        overheatText.enabled = false;
        // �I�[�o�[�q�[�g�Q�[�W���\��
        overheat.enabled = false;
        overheatOut.enabled = false;

        // �c�e����������
        bulletStock = initBulletStock;

        // ���U����������
        magazine = initMagazine;

        // Countdown�I�u�W�F�N�g�𐶐�
        countdown = gameObject.AddComponent<CS_Countdown>();
        intervalCountdown = gameObject.AddComponent<CS_Countdown>();
        autoaimCountdown = gameObject.AddComponent<CS_Countdown>();

        // �������e�B�N���T�C�Y��ۑ�
        //defaultSize = autoaim.transform.localScale;

        // ���b�N�I���J�[�\�����\��
        //lockon.enabled = false;
    }

    void FixedUpdate()
    {
        // �I�[�g�G�C������
        HandlAutoaim();

        // ���e�B�N������
        HandlReticle();

        // �Ə�����            
        //if (GetInputSystem().GetLeftTrigger() > 0.1f 
        //    && GetPlayerManager().GetCameraManager().NowCameraNumber() != 1)
        //{
        //    GetPlayerManager().GetCameraManager().SwitchingCamera(2);
        //}
        //if (GetInputSystem().GetLeftTrigger() < 0.1f
        //    && GetPlayerManager().GetCameraManager().NowCameraNumber() != 1)
        //{
        //    GetPlayerManager().GetCameraManager().SwitchingCamera(0);
        //}

        // �d������

        // bool
        foreach (var pair in GetAnimatorBoolParameterList())
        {
            if (GetAnimator().GetBool(pair.name))
            {
                countdown.Initialize(pair.time);
                break;
            }
        }
        // float
        foreach (var pair in GetAnimatorFloatParameterList())
        {
            if (GetAnimator().GetFloat(pair.name) >= 1)
            {
                countdown.Initialize(pair.time);
                break;
            }
        }

        // �ˌ�����
        if (countdown.IsCountdownFinished())
        {
            if (!GetAnimator().GetBool("Push"))
            {
                HandlShot();
            }
        }
        if (intervalCountdown.IsCountdownFinished() && isShot)
        {
           isShot = false;
        }

        // �q�[�g�Q�[�W����
        HandlOverheat();

        // �v���C���[�̌����𒲐�
        HandlPlayerAngle();
    }

    //**
    //* ���e�B�N��
    //*
    //* in:����
    //* out:����
    //**
    void HandlReticle()
    {
        // ���e�B�N���ƃ^�[�Q�b�g��������
        //detectionImage.color = notDetectedColor;
        //detectionImage.sprite = notDetectedSprite;
        targetObject = null;


        //// �J�������ʂ��烌�C���쐬
        //Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        //RaycastHit[] hits = Physics.SphereCastAll(ray, radius, range);

        //// ���e�B�N���̕ύX����
        //// ���e�B�N�����ɔj��\�I�u�W�F�N�g�����邩����
        //foreach (RaycastHit hit in hits)
        //{
        //    // �q�b�g�����ꍇ�̏���
        //    if (targetTag.Contains(hit.collider.tag))
        //    {
                
        //        break;
        //    }
        //}

        //// ���e�B�N�����I�[�g�G�C���p�ɐ؂�ւ�
        //if (isAuto)
        //{
        //    // �ʏ펞�̃��e�B�N�����\���ɂ���
        //    detectionImage.enabled = false;

        //    // ���b�N�I���J�[�\����\��
        //    lockon.enabled = true;

        //    // �J�[�\�����g��i�����̓I�[�g�G�C���p�̃��e�B�N����\������悤�ɕύX�j
        //    autoaim.transform.localScale = new Vector3(2, 2, 2);

        //    // ���b�N�I���J�[�\�����ŏ��Ɍ��m�����G�̈ʒu�̏�Ɉړ�������
        //    Vector2 screenPos = Camera.main.WorldToScreenPoint(targetObject.transform.position);
        //    lockon.rectTransform.position = screenPos;
        //}
        //// �ʏ펞
        //else
        //{
        //    // �ʏ탌�e�B�N����\��
        //    detectionImage.enabled = true;

        //    // ���b�N�I���J�[�\�����\��
        //    lockon.enabled = false;

        //    // �J�[�\�������ɖ߂��i�����̓I�[�g�G�C���p�̃��e�B�N�����\���ɂ���悤�ύX�j
        //    autoaim.transform.localScale = defaultSize;
        //}
    }


    //**
    //* �ˌ�����
    //*
    //* in:����
    //* out:����
    //**
    void HandlShot()
    {
        // ���U������
        bool isMagazine = magazine > 0;

        // �R���g���[���[����/���˒�/���U���𔻒�
        if (GetInputSystem().GetRightTrigger() > 0 && !isShot)
        {
            // �ˌ���ԂɕύX
            GetAnimator().SetBool("Shot", true);

            // �I�[�o�[�q�[�g�Q�[�W��\��
            overheat.enabled = true;
            overheatOut.enabled = true;

            // ���U����0�Ȃ烊���[�h
            if (isMagazine)
            {
                // �R���g���[���[��U��
                CS_ControllerVibration.StartVibrationWithCurve(duration, powerType, curveType, repetition);

                CreateBullet(burstfire);
                isShot = true;
                //weaponIdle.SetActive(false);
                //weaponShot.SetActive(true);

                GetAnimator().SetBool("Shot", true);

                intervalCountdown.Initialize(interval);
            }
            else if (!GetAnimator().GetBool("Reload"))
            {
                isShot = true;
                StartCoroutine(ReloadCoroutine());

                // �A�j���[�V�����̏I���܂őҋ@
                countdown.Initialize(intervalReload);
                // �����[�h����
                ReloadMagazine(initMagazine);
            }
        }
        else
        {
            GetAnimator().SetBool("Shot", false);

            //ReloadMagazine(initMagazine);

            //weaponIdle.SetActive(true);
            //weaponShot.SetActive(false);
        }

        if (GetInputSystem().GetRightTrigger() <= 0 && isShot)
        {
            ReloadMagazine(initMagazine);
            Cold = true;
        }
    }

    //**
    //* �����[�h����
    //*
    //* in:�����[�h��
    //* out:����
    //**
    void ReloadMagazine(int reload)
    {
        magazine = reload;

        //if (bulletStock < reload)
        //{
        //    magazine = bulletStock;
        //    bulletStock = 0;
        //}
        //else
        //{
        //    bulletStock -= (reload - magazine);
        //    magazine = reload;
        //}
    }
    private IEnumerator ReloadCoroutine()
    {
        // �����[�h�A�j���[�V�������J�n
        GetAnimator().SetBool("Reload", true);

        GetAnimator().SetTrigger("Reload");

        // �A�j���[�V�����̒������擾
        AnimationClip clip = GetAnimator().runtimeAnimatorController.animationClips[0];
        float animationLength = clip.length;

        // �A�j���[�V�������Đ������ǂ������m�F
        float elapsedTime = 0f;
        while (elapsedTime < animationLength)
        {
            elapsedTime += Time.deltaTime;
            yield return null; // ���̃t���[���܂őҋ@
        }

        // �����[�h�������s��
        //ReloadMagazine(initMagazine);
        GetAnimator().SetBool("Reload", false);
    }

    //**
    //* �e�𐶐����鏈��
    //*
    //* in:������
    //* out:����
    //**
    void CreateBullet(int burst)
    {
        GetSoundEffect().PlaySoundEffect(1, 3);
        
        Vector3 forwardVec = GetPlayerManager().GetCameraTransform().forward;

        // ���e�B�N�����ɔj��\�I�u�W�F�N�g�����݂���Ȃ�A���̕����ɔ��˂���
        if ((targetObject != null)&&(isAuto))
        {            
            forwardVec = targetObject.transform.position - GetPlayerManager().GetCameraTransform().position;
            forwardVec = forwardVec.normalized;
        }

        float offsetDistance = 0f;//1.5f; // �e�e�̊Ԋu

        if (burst > magazine)
        {
            burst = magazine;
        }

        for (int i = 0; i < burst; i++)
        {
            // �e�𐶐�
            GameObject ballobj = Instantiate(AirBall);

            //Vector3 pos = this.transform.position;
            Vector3 pos = GetPlayerManager().GetCameraTransform().position;
            Vector3 offset = new Vector3(0, 0, 0);

            pos += offset;
            pos += forwardVec * (offsetDistance * (i + (burst - 1) / 2f) + 1f);

            // �U�e����
            if (!(GetInputSystem().GetLeftTrigger() > 0.1f) && (!isAuto))
            {
                float randomRangeX = UnityEngine.Random.Range(-scatter, scatter);
                float randomRangeY = UnityEngine.Random.Range(-scatter, scatter);
                float randomRangeZ = UnityEngine.Random.Range(-scatter, scatter);
                forwardVec.x += randomRangeX;
                forwardVec.y += randomRangeY;
                forwardVec.z += randomRangeZ;
            }

            ballobj.transform.position = pos;
            ballobj.transform.forward = forwardVec;

            // ���U�������炷
            magazine--;
            //bulletStock--;
            //float hp = GetPlayerManager().GetHP();
            //GetPlayerManager().SetHP(hp - -shotHp);
        }
    }

    // IKPass
    private void OnAnimatorIK(int layerIndex)
    {
        // �Ր�Ԑ��A�j���[�V�����i�e���\����j
        if ((!GetAnimator().GetBool("Reload")) && (!GetAnimator().GetBool("Throwing")
            && !GetAnimator().GetBool("Recovery") && !GetAnimator().GetBool("Use Item")
            && !GetAnimator().GetBool("Use EnergyCore") && !GetAnimator().GetBool("Push")))
        {
            // �J�����̐��ʕ����̈ʒu���v�Z
            Vector3 cameraForward = Camera.main.transform.forward * 100;
            Vector3 targetPosition = Camera.main.transform.position + cameraForward;

            // ���Ƙr���^�[�Q�b�g�̕����Ɍ�����
            if (targetPosition != null)
            {
                // �����^�[�Q�b�g�Ɍ�����
                GetAnimator().SetLookAtWeight(1f, 0.3f, 1f, 0f, 0.5f);
                GetAnimator().SetLookAtPosition(targetPosition);

                // �E�r���^�[�Q�b�g�Ɍ�����
                GetAnimator().SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
                GetAnimator().SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
                GetAnimator().SetIKPosition(AvatarIKGoal.RightHand, targetPosition);
                GetAnimator().SetIKRotation(AvatarIKGoal.RightHand, Quaternion.LookRotation(cameraForward));

            }
        }

    }

    /*
     * �I�[�g�G�C������
     */
    void HandlAutoaim()
    {
        // �I�[�g�G�C����Ԃ̏���
        if (isAuto && Time.time >= nextAutoaimCheckTime)
        {
            // �I�[�g�G�C���̌��ʎ��Ԃ̃J�E���g���J�n
            autoaimCountdown.Initialize(autoTime);

            // ���C�̔���Ԋu���X�V
            nextAutoaimCheckTime = Time.time + autoaimCheckInterval;

            // �J�������ʂ��烌�C���쐬
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;
            Vector3 boxSize = new Vector3(radiusAuto, radiusAuto, radiusAuto);

            if (Physics.BoxCast(ray.origin, boxSize / 2, ray.direction, out hit, Quaternion.identity, range))
            {
                if (targetTag.Contains(hit.collider.tag))
                {
                    targetObject = hit.collider.gameObject;
                }
            }
        }

        // ���ʎ��Ԃ̏I������
        if (autoaimCountdown.IsCountdownFinished())
        {
            isAuto = false;
        }
    }

    /*
     * �I�[�o�[�q�[�g�Q�[�W����
     */
    void HandlOverheat()
    {
        //��p������Ȃ����
        if (!Cold)
        {
            // �I�[�o�[�q�[�g����
            overheat.fillAmount = magazine * 0.1f;
            overheatOut.fillAmount = overheat.fillAmount;

        }
        else
        {
            //���e�B�N�����p���摜�ɂ���
            detectionImage.sprite = ColdREticle;

            //��p����
            overheat.fillAmount += interval * Time.deltaTime;
            overheatOut.fillAmount = overheat.fillAmount;
            //�����[�h�A�j���[�V�������I�����Ă������p����
            if (overheat.fillAmount >= 1f)
            {
                // ���e�B�N�������ɖ߂�
                detectionImage.sprite = reticle;

                // �I�[�o�[�q�[�g�Q�[�W���\��
                overheat.enabled = false;
                overheatOut.enabled = false;
                Cold = false;
                overheatText.enabled = false;
            }
            else
            {
                overheat.enabled = true;
                overheatOut.enabled = true;
            }
        }

        //��萔�c�e������������Q�[�W��Ԃ�����
        if (overheat.fillAmount < 0.5f)
        {
            //float AlphaSpeed = 2f;
            //OverHeatAlpha -= AlphaSpeed * Time.deltaTime;
            //overheat.color = new Color(1.0f, 1.0f, 1.0f, OverHeatAlpha);

            overheat.enabled = false;
            overheatText.enabled = true;
        }
        else
        {
            //float AlphaSpeed = 2f;
            //OverHeatAlpha += AlphaSpeed * Time.deltaTime;
            //overheat.color = new Color(1.0f, 1.0f, 1.0f, OverHeatAlpha);

            //overheat.enabled = true;
            overheatText.enabled = false;
        }

        //�c�e����0�ɂȂ������p
        if (magazine <= 0)
        {
            Cold = true;
        }
    }

    /*
     * �J�����̊p�x����v���C���[�̌����𒲐�����֐�
     */
    void HandlPlayerAngle()
    {
        // ��]�����v�Z
        Vector3 axis = Vector3.Cross(transform.forward, cameraTarget.transform.forward);

        // �p�x���v�Z
        float angle = Vector3.Angle(transform.forward, cameraTarget.transform.forward) * (axis.y < 0 ? -1 : 1);

        // �p�x��臒l���傫���ꍇ�̂݉�]���s��
        if (Mathf.Abs(angle) > 25f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, cameraTarget.transform.rotation, 2f * Time.deltaTime);
            cameraTarget.transform.rotation = Quaternion.Slerp(cameraTarget.transform.rotation, transform.rotation, 2f * Time.deltaTime);
        }
        else
        {
            GetAnimator().SetFloat("Turn", 0f);
        }

        // Aim�̂��炷�����𒲐�
        Vector2 stick = GetInputSystem().GetRightStick();
        if (stick.x > 0.3f)
        {
            aimTarget.transform.localPosition = Vector3.Lerp(
            aimTarget.transform.localPosition,
            new Vector3(1, 1, -1),
            1f * Time.deltaTime
            );

            if (Mathf.Abs(angle) > 25f)
                GetAnimator().SetFloat("Turn", 1f);
        }
        else if (stick.x < -0.3f)
        {
            aimTarget.transform.localPosition = Vector3.Lerp(
            aimTarget.transform.localPosition,
            new Vector3(1, 1, 1),
            1f * Time.deltaTime
            );

            if (Mathf.Abs(angle) > 25f)
                GetAnimator().SetFloat("Turn", -1f);
        }
    }

    // �f�o�b�N�p�Ɋg�U�͈͂̉~����\��
    //private void OnDrawGizmos()
    //{
    //    float radius = range * scatter;
    //    float height = range;
    //    int segments = 30;

    //    // ��ʂ̉~��`��
    //    Vector3 center = Camera.main.transform.position + Camera.main.transform.forward * height;
    //    for (int i = 0; i < segments; i++)
    //    {
    //        float angle0 = i * Mathf.PI * 2f / segments;
    //        float angle1 = (i + 1) * Mathf.PI * 2f / segments;

    //        Vector3 point0 = center + new Vector3(Mathf.Cos(angle0) * radius, Mathf.Sin(angle0) * radius,0);
    //        Vector3 point1 = center + new Vector3(Mathf.Cos(angle1) * radius, Mathf.Sin(angle1) * radius,0);

    //        Gizmos.DrawLine(point0, point1); // ��ʂ̉~��`��
    //        Gizmos.DrawLine(center, point0); // ���_�Ɍ������Đ���`��
    //    }

    //    // ��ʂƒ��_�����Ԑ���`��
    //    Vector3 top = Camera.main.transform.position;
    //    for (int i = 0; i < segments; i++)
    //    {
    //        float angle = i * Mathf.PI * 2f / segments;
    //        Vector3 point = center + new Vector3(Mathf.Cos(angle) * radius,  Mathf.Sin(angle) * radius,0);
    //        Gizmos.DrawLine(top, point);
    //    }
    //}
}
