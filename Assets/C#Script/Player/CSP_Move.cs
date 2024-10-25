using UnityEngine;

public class CSP_Move : ActionBase
{
    // �ړ�
    [Header("�ړ��ݒ�")]
    [SerializeField, Header("�ړ����x")]
    private float speed = 1f;        // �ړ����x
    [SerializeField, Header("�ڕW���x")]
    private float targetSpeed = 10f; // �ڕW���x
    [SerializeField, Header("�ō����x�ɓ��B����܂ł̎���")]
    private float smoothTime = 0.5f; // �ō����x�ɓ��B����܂ł̎���
    private float velocity = 0f;     // ���݂̑��x
    private Vector3 moveVec;         // ���݂̈ړ�����
    private float initSpeed;         // �X�s�[�h�̏����l��ۑ����Ă����ϐ�
    [SerializeField, Header("�_�b�V�����͂�臒l")]
    private float dashInputValue = 0.75f;
    [SerializeField, Header("�}�E���g���̌����{��")]
    private float decelerationMount;
    [SerializeField, Header("�Q�쎞�̌����{��")]
    private float decelerationHunger;
    [SerializeField, Header("�����d���̎���")]
    private float stunnedTime;

    [Header("�ǔ���")]
    [SerializeField, Header("�ǂƂ̋���")]
    private float climbCheckDistance = 1f;
    [SerializeField, Header("�o���ǂ̍���")]
    private float climbMax = 2f;
    [SerializeField]
    private float climbMin = 0.25f;
    [SerializeField, Header("�o���ǂ̃��C���[")]
    private LayerMask climbLayer;

    // �J�E���g�_�E���p�N���X
    private CS_Countdown countdown;

    protected override void Start()
    {
        base.Start();

        // �ړ����x�̏����l��ۑ�
        initSpeed = speed;

        // Countdown�I�u�W�F�N�g�𐶐�
        countdown = gameObject.AddComponent<CS_Countdown>();
    }

    void FixedUpdate()
    {
        // �d������
        if (GetPlayerManager().GetStunned())
        {
            // �J�E���g�_�E�����Z�b�g
            countdown.Initialize(stunnedTime);
            GetPlayerManager().SetStunned(false);
            StopMove();
        }

        // �ړ�����
        if (countdown.IsCountdownFinished())
        {
            HandleMovement();
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

            // �ʒu���X�V
            MoveCharacter(moveVec);

            // �O���������X���[�Y�ɒ���
            AdjustForwardDirection(moveVec);
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

        // ���ʉ����Đ�����
        if (GetAnimator().GetBool("Dash"))
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

        if (GetAnimator().GetBool("Mount"))
        {
            direction *= decelerationMount;
        }
        if (GetAnimator().GetFloat("Hunger") == 1)
        {
            direction *= decelerationHunger;
        }

        GetRigidbody().MovePosition(GetRigidbody().position + direction);

    }

    //**
    //* �L�����N�^�[��i�s�����Ɍ�����
    //*
    //* in�F�ړ��x�N�g��
    //* out�F����
    //**
    void AdjustForwardDirection(Vector3 moveVec)
    {
        if (moveVec.sqrMagnitude > 0)
        {
            Vector3 targetForward = moveVec.normalized;
            transform.forward = Vector3.Lerp(transform.forward, targetForward, 0.1f);
        }
    }

    void StopMove()
    {
        // 0�ԃC���f�b�N�X�̌��ʉ����~
        GetSoundEffect().StopPlayingSound(0);

        // �ړ����x��������
        speed = initSpeed;

        // �A�j���[�^�[�̒l��ύX
        GetAnimator().SetBool("Move", false);
        GetAnimator().SetBool("Dash", false);

        // ���s�Ȉړ���������菜��
        Vector3 currentVelocity = GetRigidbody().velocity;
        GetRigidbody().velocity = new Vector3(0f, currentVelocity.y, 0f);

    }

    private void OnCollisionStay(Collision collision)
    {
        //**
        //* �߂荞�ݖh�~
        //**

        // �����ȕǂƏՓ˂����ꍇ�Ɉړ���Ԃ��~���A�������ړ��ʂ�0�ɂ���
        if (collision.contactCount > 0)
        {
            Vector3 collisionNormal = collision.contacts[0].normal;

            if (Mathf.Abs(collisionNormal.y) < 0.1f)
            {
                StopMove();
            }
        }
    }

    //**
    //* �i���𖳎�����
    //*
    //* in:����
    //* out:���茋��
    //**
    void StepSkip()
    {
        if (IsStep() && GetAnimator().GetBool("Dash") && GetAnimator().GetBool("isGrounded") && !GetAnimator().GetBool("Jump"))
        {
            // ������ɗ͂�������
            float liftForce = 0.75f;

            GetRigidbody().AddForce(Vector3.up * liftForce, ForceMode.Impulse);
        }
    }

    //**
    //* ���ʂɓo���i�������邩����
    //*
    //* in:����
    //* out:���茋��
    //**

    bool IsStep()
    {
        bool isMax = false;
        bool isMin = false;

        RaycastHit hit;
        Vector3 offsetMax = new Vector3(0f, climbMax, 0f);

        // ���Raycast
        isMax = Physics.Raycast(transform.position + offsetMax, transform.forward, out hit, climbCheckDistance, climbLayer);

        Vector3 offsetMin = new Vector3(0f, climbMin, 0f);

        // ����Raycast
        isMin = Physics.Raycast(transform.position + offsetMin, transform.forward, out hit, climbCheckDistance, climbLayer);

        // �o��������Ԃ�
        return !isMax && isMin;
    }
}
