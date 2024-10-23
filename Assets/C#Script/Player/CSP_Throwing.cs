using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//**
//* ��������
//*
//* �S���F�����V�S
//**

public class CSP_Throwing : ActionBase
{
    [Header("�����ݒ�")]
    [SerializeField]
    private string targetTag;// �����Ώۂ̃^�O

    [Header("�^�[�Q�b�g�֘A")]
    [SerializeField] 
    private GameObject targetObject;
    private Rigidbody rb;
    private LineRenderer lineRenderer;
    private List<Vector3> positions = new List<Vector3>();

    [Header("�͂̐ݒ�")]
    public float forceMagnitude = 10f;          // �͂̑傫��
    public float angle = 45f;                   // ������p�x
    private int steps = 30;                     // �`��̐��x
    private float timeStep = 0.1f;              // ���Ԃ̃X�e�b�v

    private float oldLeftTrigger = 0f;// 1�t���[���O�̓��͂�ۑ�

    protected override void Start()
    {
        base.Start();
        // ���C�������_�[���擾���ď�����
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.enabled = false;
    }

    void FixedUpdate()
    {
        lineRenderer.enabled = false;

        if ((GetInputSystem().GetLeftTrigger() == 0)&&(oldLeftTrigger > 0)&&(!GetInputSystem().GetButtonBPressed()))
        {
            if (targetObject != null)
            {
                GetAnimator().SetBool("Throwing", false);

                rb.useGravity = true;

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
            }
        }

        // �ʒu���X�V
        if (targetObject != null)
        {
            GetAnimator().SetBool("Mount", true);

            positions.Add(targetObject.transform.position);
            lineRenderer.positionCount = positions.Count;
            lineRenderer.SetPositions(positions.ToArray());
        }
        else
        {
            GetAnimator().SetBool("Mount", false);
        }

        // ������O����\�����ĕ`��
        if ((GetInputSystem().GetLeftTrigger() > 0)&&(targetObject != null)&&(!GetInputSystem().GetButtonBPressed()))
        {
            GetAnimator().SetBool("Throwing", true);

            lineRenderer.enabled = true;

            Vector3 offset = new Vector3(0, 2.5f, 0);
            targetObject.transform.position = transform.position + offset;

            DrawTrajectory();
        }
        else if (targetObject != null)
        {
            float distance = 0.75f;
            Vector3 offset = new Vector3(0, 1.5f, 0);
            Vector3 backPosition = transform.position - transform.forward * distance;
            targetObject.transform.position = backPosition + offset;
            targetObject.transform.rotation = Quaternion.identity;
            rb.useGravity = false;
        }

        oldLeftTrigger = GetInputSystem().GetLeftTrigger();
       
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

    //**
    //* �Փˏ���
    //**
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == targetTag)
        {
            targetObject = collision.gameObject;

            // ���W�b�g�{�f�B���擾
            rb = targetObject.GetComponent<Rigidbody>();

            // ���C�������_�[�̏����ʒu��ݒ�
            positions.Add(targetObject.transform.position);
        }
    }
}
