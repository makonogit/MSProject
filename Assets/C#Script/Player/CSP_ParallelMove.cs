using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

//**
//* ���s�ړ�
//*
//* �S���F�����V�S
//**

public class CSP_ParallelMove : ActionBase
{
    // �d��
    [System.Serializable]
    public class StringNumberPair
    {
        public string name;
        public float magnification;
        public bool flg;
    }

    // �ړ�
    [Header("�ړ��ݒ�")]
    [SerializeField, Header("�ړ����x")]
    private float speed = 1f;        // �ړ����x
    public float GetSpeed() => speed;
    public void SetSpeed(float set) {  speed = set; }
    [SerializeField, Header("�ڕW���x")]
    private float targetSpeed = 10f; // �ڕW���x
    [SerializeField, Header("�ō����x�ɓ��B����܂ł̎���")]
    private float smoothTime = 0.5f; // �ō����x�ɓ��B����܂ł̎���
    private float velocity = 0f;     // ���݂̑��x
    private Vector3 moveVec;         // ���݂̈ړ�����
    private float initSpeed;         // �X�s�[�h�̏����l��ۑ����Ă����ϐ�
    [SerializeField, Header("�_�b�V�����͂�臒l")]
    private float dashInputValue = 0.75f;
    //[SerializeField, Header("���/�ړ����x�̔{��")]
    private StringNumberPair[] animatorBoolSpeedList;
    //[SerializeField]
    private StringNumberPair[] animatorFloatSpeedList;
    //[SerializeField, Header("�A�j���[�V�����̐؂�ւ����x")]
    private float animSpeed = 0.25f;

    // �J�E���g�_�E���p�N���X
    private CS_Countdown countdown;
    private CS_Countdown countdownDamage;

    // �Փ˕���
    private Vector3 collisionNormal;
    [SerializeField, Header("�_���[�W���̃m�b�N�o�b�N")]
    private float forceMagnitude = 0.5f;

    protected override void Start()
    {
        base.Start();

        // �ړ����x�̏����l��ۑ�
        initSpeed = speed;

        // Countdown�I�u�W�F�N�g�𐶐�
        countdown = gameObject.AddComponent<CS_Countdown>();
        countdownDamage = gameObject.AddComponent<CS_Countdown>();
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
                StopMove();
                break;
            }
        }
        // float
        foreach (var pair in GetAnimatorFloatParameterList())
        {
            if (GetAnimator().GetFloat(pair.name) >= 1)
            {
                countdown.Initialize(pair.time);
                StopMove();
                break;
            }
        }

        // �ړ�����
        if (countdown.IsCountdownFinished())
        {
            HandleMovement();
        }
        // �G�ƏՓ˂��Ă���ꍇ�A�t�����ɗ͂�������
        if (GetAnimator().GetBool("Damage"))
        {
            Vector3 reverseForce = collisionNormal * forceMagnitude;
            reverseForce.y = 0f;
            if ((reverseForce.x == 0f) && (reverseForce.z == 0f))
            {
                reverseForce.z = -forceMagnitude;
            }

            GetRigidbody().AddForce(reverseForce, ForceMode.Impulse);

            if (countdownDamage.IsCountdownFinished())
                GetAnimator().SetBool("Damage", false);
        }
    }

    //**
    //* �ړ�����
    //*
    //* in�F����
    //* out�F����
    //**
    void HandleMovement()
    {
        // L�X�e�b�N�̓��͂��`�F�b�N
        if (GetInputSystem().GetLeftStickActive())
        {
            // �X�e�B�b�N�̓��͂��擾
            Vector3 moveVec = GetMovementVector();

            Vector2 stick = GetInputSystem().GetLeftStick();

            GetAnimator().SetFloat("MoveVecZ", SmoothlyChange(GetAnimator().GetFloat("MoveVecZ"), stick.x,animSpeed));
            GetAnimator().SetFloat("MoveVecX", SmoothlyChange(GetAnimator().GetFloat("MoveVecX"), stick.y, animSpeed));

            // �ʒu���X�V
            MoveCharacter(moveVec);
            GetAnimator().SetBool("Move", true);
        }
        else
        {
            // 0�ԃC���f�b�N�X�̌��ʉ����~
            GetSoundEffect().StopPlayingSound(0);

            // �ړ����x��������
            speed = initSpeed;

            // �A�j���[�^�[�̒l��ύX
            GetAnimator().SetBool("Move", false);
            GetAnimator().SetBool("Dash", false);
        }
    }

    //**
    //* �X�e�B�b�N���͂���J�������猩���ړ��x�N�g�����擾����
    //*
    //* in�F����
    //* out�F�ړ��x�N�g��
    //**
    Vector3 GetMovementVector()
    {
        Vector2 stick = GetInputSystem().GetLeftStick();
        Vector3 forward = GetPlayerManager().GetCameraTransform().forward;
        Vector3 right = GetPlayerManager().GetCameraTransform().right;
        Vector3 moveVec = forward * stick.y + right * stick.x;
        moveVec.y = 0f; // y ���̈ړ��͕s�v
        return moveVec.normalized;
    }

    //**
    //* �L�����N�^�[�̈ʒu���X�V����
    //*
    //* in�F�ړ��x�N�g��
    //* out�F����
    //**
    void MoveCharacter(Vector3 moveVec)
    {
        // �X�e�B�b�N���傫���|����Ă���΁A��������
        Vector2 stick = GetInputSystem().GetLeftStick();
        if ((stick.y > dashInputValue) || (stick.x > dashInputValue)
            || (stick.y < -dashInputValue) || (stick.x < -dashInputValue))
        {
            speed = Mathf.SmoothDamp(speed, targetSpeed, ref velocity, smoothTime);
            GetAnimator().SetBool("Dash", true);

        }

        if (GetAnimator().GetBool("Push"))
        {
            speed = initSpeed;
            GetAnimator().SetBool("Dash", false);
        }


        //// �⓹���~��邽�߂ɑ��x�𒲐�
        //float maxSlopeAngle = 30f;
        //// �n�ʂ̖@���x�N�g�����擾
        //RaycastHit hit;
        //if ((Physics.Raycast(transform.position, Vector3.down, out hit, 5f))
        //    && GetPlayerManager().IsGrounded())
        //{
        //    // �⓹�̊p�x���v�Z
        //    float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

        //    // �⓹���}������ꍇ�A��щz����h�����߂ɑ��x�𐧌�
        //    if (slopeAngle > maxSlopeAngle)
        //    {
        //        speed = initSpeed;
        //        GetAnimator().SetBool("Dash", false);
        //    }
        //}


        // ���ʉ����Đ�����
        if (!GetAnimator().GetBool("isGrounded"))
        {
            // ��
            GetSoundEffect().StopPlayingSound(0);
        }
        else if (GetAnimator().GetBool("Dash"))
        {
            // �_�b�V��
            GetSoundEffect().PlaySoundEffect(0, 2);
        }
        else if (GetAnimator().GetBool("Move"))
        {
            // �ړ�
            GetSoundEffect().PlaySoundEffect(0, 0);
        }

        // �v���C���[�̈ʒu���X�V
        Vector3 direction = moveVec * speed * Time.deltaTime;

        // ��Ԃɂ���Ĉړ����x��ύX����
        //foreach (var pair in animatorBoolSpeedList)
        //{
        //    if (GetAnimator().GetBool(pair.name) == pair.flg)
        //    {
        //        direction *= pair.magnification;
        //    }
        //}
        //foreach (var pair in animatorFloatSpeedList)
        //{
        //    if ((GetAnimator().GetFloat(pair.name) >= 1) == pair.flg)
        //    {
        //        direction *= pair.magnification;
        //    }
        //}



        //GetRigidbody().MovePosition(GetRigidbody().position + direction);

        Vector3 newPos = transform.position + direction;
        transform.position = newPos;

    }

    void StopMove()
    {
        // 0�ԃC���f�b�N�X�̌��ʉ����~
        GetSoundEffect().StopPlayingSound(0);

        // �ړ����x��������
        speed = 0f;

        // �A�j���[�^�[�̒l��ύX
        GetAnimator().SetBool("Move", false);
        GetAnimator().SetBool("Dash", false);

        // ���s�Ȉړ���������菜��
        Vector3 currentVelocity = GetRigidbody().velocity;
        GetRigidbody().velocity = new Vector3(0f, currentVelocity.y, 0f);
    }

    private void OnCollisionStay(Collision collision)
    {
        GetAnimator().SetBool("Push", false);

        if (collision.gameObject.tag == "PushObject")
        {
            GetAnimator().SetBool("Push", true);
        }
        // �����ȕǂƏՓ˂����ꍇ�Ɉړ���Ԃ��~���A�������ړ��ʂ�0�ɂ���
        else if (collision.contactCount > 0)
        {
            Vector3 collisionNormal = collision.contacts[0].normal;

            if (Mathf.Abs(collisionNormal.y) < 0.1f)
            {
                StopMove();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy"
            && !GetAnimator().GetBool("Damage"))
        {
            GetSoundEffect().PlaySoundEffect(3, 7);

            GetAnimator().SetBool("Damage", true);

            // �Փ˕�����ۑ�
            collisionNormal = collision.contacts[0].normal;

            countdownDamage.Initialize(1);
        }
    }

    float SmoothlyChange(float curren,float target,float lerpSpeed)
    {
        // ���݂̒l����^�[�Q�b�g�l�֕��
        return Mathf.Lerp(curren, target, lerpSpeed);
    }
}
