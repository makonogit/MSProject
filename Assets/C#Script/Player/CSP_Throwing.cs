using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

//**
//* ��������
//*
//* �S���F�����V�S
//**

public class CSP_Throwing : ActionBase
{
    [Header("�R�A�̈ʒu/��]")]
    [SerializeField]
    private float distance = 0.75f;
    public Vector3 fixedRotation;

    [Header("�����ݒ�")]
    [SerializeField]
    private string targetTag;// �����Ώۂ̃^�O

    [Header("�^�[�Q�b�g�֘A")]
    [SerializeField] 
    private GameObject targetObject;
    public GameObject GetEnergyCore() => targetObject;
    private Rigidbody rb;
    private LineRenderer lineRenderer;
    private List<Vector3> positions = new List<Vector3>();
    private Collider collider;

    [Header("�͂̐ݒ�")]
    public float forceMagnitude = 10f;          // �͂̑傫��
    public float angle = 45f;                   // ������p�x
    private int steps = 30;                     // �`��̐��x
    private float timeStep = 0.1f;              // ���Ԃ̃X�e�b�v

    private float oldLeftTrigger = 0f;// 1�t���[���O�̓��͂�ۑ�

    // ���Ԍv���N���X
    private CS_Countdown countdown;

    protected override void Start()
    {
        base.Start();
        // ���C�������_�[���擾���ď�����
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.enabled = false;

        // Countdown�I�u�W�F�N�g�𐶐�
        countdown = gameObject.AddComponent<CS_Countdown>();
    }

    void FixedUpdate()
    {
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

        if (countdown.IsCountdownFinished())
        {
            // �\�z�����\��
            lineRenderer.enabled = false;

            if (GetAnimator().GetBool("Mount"))
            {
                // �����鏈��
                if ((GetInputSystem().GetLeftTrigger() == 0) && (oldLeftTrigger > 0) && (!GetInputSystem().GetButtonBPressed()))
                {
                    if (targetObject != null)
                    {
                        GetAnimator().SetBool("Throwing", false);

                        collider.enabled = true;

                        rb.useGravity = true;

                        GetSoundEffect().PlaySoundEffect(2, 8);

                        // �]�v�Ȉړ���������菜��
                        Vector3 currentVelocity = rb.velocity;
                        rb.velocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;

                        // �J�����̐��ʕ����̈ʒu���擾
                        Vector3 cameraForward = Camera.main.transform.forward;

                        // �w�肵���p�x�ŗ͂�������
                        Vector3 force = GetForceVector(angle, forceMagnitude, cameraForward);
                        rb.AddForce(force, ForceMode.Impulse);

                        // �^�[�Q�b�g�����Z�b�g
                        targetObject = null;
                        rb = null;
                        collider = null;
                    }
                }

                // ���C�������_�[�̏����l�ʒu���X�V
                if (targetObject != null)
                {
                    positions.Add(targetObject.transform.position);
                    lineRenderer.positionCount = positions.Count;
                    lineRenderer.SetPositions(positions.ToArray());
                }
                else
                {
                    GetAnimator().SetBool("Mount", false);
                }

                // �����鎞�̃R�A�̈ʒu���X�V
                // ������O����\�����ĕ`��
                if ((GetInputSystem().GetLeftTrigger() > 0) && (targetObject != null) && (!GetInputSystem().GetButtonBPressed()))
                {
                    GetAnimator().SetBool("Throwing", true);

                    lineRenderer.enabled = true;

                    Vector3 offset = new Vector3(0, 2.5f, 0);
                    targetObject.transform.position = transform.position + offset;

                    DrawTrajectory();
                }
                // �w�������̃R�A�̈ʒu���X�V
                else if (targetObject != null)
                {
                    Vector3 offset = new Vector3(0, 1.0f, 0);
                    Vector3 backPosition = transform.position - transform.forward * distance;
                    targetObject.transform.position = backPosition + offset;
                    targetObject.transform.rotation = Quaternion.identity;
                    rb.useGravity = false;

                    targetObject.transform.rotation = Quaternion.Euler(fixedRotation);
                }

                oldLeftTrigger = GetInputSystem().GetLeftTrigger();
            }
        }

        if (!GetAnimator().GetBool("Mount"))
        {
            if (targetObject != null)
            {
                // �^�[�Q�b�g�����Z�b�g
                //collider.enabled = true;
                rb.useGravity = true;
                targetObject = null;
                rb = null;
                collider = null;
            }
        }
    }

    // �͂̕������v�Z
    private Vector3 GetForceVector(float angle, float magnitude, Vector3 direction)
    {
        // �����x�N�g���𐳋K��
        Vector3 normalizedDirection = direction.normalized;

        // �����Ɋ�Â��Ċp�x���v�Z
        float radian = angle * Mathf.Deg2Rad;

        // ��]�s����g�p���ĐV�����������v�Z
        Vector3 forceDirection = new Vector3(
            normalizedDirection.x * Mathf.Cos(radian) + normalizedDirection.x * Mathf.Sin(radian),
            normalizedDirection.y * Mathf.Sin(radian) + normalizedDirection.y * Mathf.Cos(radian),
            normalizedDirection.z * Mathf.Cos(radian) + normalizedDirection.z * Mathf.Sin(radian)
        );

        // �傫�����|����
        return forceDirection * magnitude;
    }




    // ������O����`��
    private void DrawTrajectory()
    {
        // �J�����̐��ʕ����̈ʒu���擾
        Vector3 cameraForward = Camera.main.transform.forward;
        // �J�n�ʒu
        Vector3 startPos = targetObject.transform.position;
        // �͂̕���
        Vector3 initialVelocity = GetForceVector(angle, forceMagnitude / targetObject.GetComponent<Rigidbody>().mass, cameraForward);
        // �d��
        float gravity = Physics.gravity.y;

        // �O����`��
        lineRenderer.positionCount = steps + 1;
        for (int i = 0; i <= steps; i++)
        {
            float t = i * timeStep;
            Vector3 position = startPos + initialVelocity * t + 0.5f * new Vector3(0, gravity, 0) * t * t;
            lineRenderer.SetPosition(i, position);
        }
    }

    // �����̃I�u�W�F�N�g���w�肵���I�u�W�F�N�g�̎q�Ƃ��Đݒ肷��֐�
    // ������: "Parent/Child/Grandchild"
    public void SetChildObject(string targetPath,Transform objectToChild)
    {
        // �w�肵���p�X�̃g�����X�t�H�[�����擾
        Transform targetTransform = transform.Find(targetPath);

        if (targetTransform != null && objectToChild != null)
        {
            objectToChild.SetParent(targetTransform);
        }
    }

    // �����̃I�u�W�F�N�g���w�肵���I�u�W�F�N�g����O���֐�
    public void RemoveChildObject(string targetPath, Transform objectToRemove)
    {
        Transform targetTransform = transform.Find(targetPath);

        if (targetTransform != null && objectToRemove != null)
        {
            if (objectToRemove.parent == targetTransform)
            {
                objectToRemove.SetParent(null); // �e��null�ɂ��ăV�[���̃��[�g�ɖ߂�
            }
        }
    }

//**
//* �Փˏ���
//**
    private void OnCollisionStay(Collision collision)
    {
        // �G�l���M�[�R�A���E��

        if (countdown.IsCountdownFinished())
        {
            if (collision.gameObject.tag == targetTag)
            {
                targetObject = collision.gameObject;

                // ���W�b�g�{�f�B���擾
                rb = targetObject.GetComponent<Rigidbody>();

                // �R���C�_�[���擾
                collider = targetObject.GetComponent<Collider>();
                collider.enabled = false;

                // ���C�������_�[�̏����ʒu��ݒ�
                positions.Add(targetObject.transform.position);

                GetAnimator().SetBool("Mount", true);

                //SetChildObject("Player/All_Base/All_Ctrl/Hip_Base/Spine1_Base/Spine1_CtrlSpine2_Base/Spine2_Ctrl/Neck_Base", targetObject.transform);
            }
        }

        // �G�ƏՓ˂����ꍇ�A�G�l���M�[�R�A�𗎂Ƃ�
        if (collision.gameObject.tag == "Enemy")
        {
            if(targetObject != null)
            {
                collider.enabled = true;

                rb.useGravity = true;
                rb.AddForce(Vector3.up * 30.0f, ForceMode.Impulse);

                // �^�[�Q�b�g�����Z�b�g
                targetObject = null;
                rb = null;
                collider = null;
            }
        }
    }
}
