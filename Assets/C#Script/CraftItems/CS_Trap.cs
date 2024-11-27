using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �N���t�g�A�C�e���@�g���o�T�~
// �����V�S
public class CS_Trap : CraftItemBase
{
    // �ݒ荀��
    [SerializeField, Header("�S������")]
    private float restraintTime = 3f;
    [SerializeField, Header("�S���Ώ�")]
    private string restraintTag = "Enemy";
    [SerializeField, Header("���ˑ��x")]
    private float speed = 1f;

    // �g���b�v�̃��f��
    private GameObject openTrap;    // �ҋ@
    private GameObject closeTrap;   // �N��

    // �Փ˂����I�u�W�F�N�g
    private GameObject hitObject;

    // �I�[�f�B�I
    private CS_SoundEffect soundEffect;

    // ���Ԍv��
    private CS_Countdown countdown;

    // �ݒu���
    private bool isMove = true;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();

        // ���Ԍv���p�I�u�W�F�N�g���쐬
        countdown = gameObject.AddComponent<CS_Countdown>();

        // �q�I�u�W�F�N�g���擾
        openTrap = transform.GetChild(0).gameObject;
        closeTrap = transform.GetChild(1).gameObject;
        soundEffect = transform.GetChild(2).gameObject.GetComponent<CS_SoundEffect>();

        // �\����Ԃ�������
        openTrap.SetActive(true);
        closeTrap.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // ���˂��Đݒu
        if (isMove)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        // �ݒu���̏���
        else if (!countdown.IsCountdownFinished())
        {
            hitObject.transform.position = transform.position;
        }
        else if(hitObject != null)
        {
            hitObject = null;
            Destroy(gameObject);
        }
    }

    // �Փˏ���
    private void OnTriggerEnter(Collider other)
    {
        // �G�ƏՓ˂����ꍇ�A�G���ړ��s�\�ɂ���
        if (other.gameObject.CompareTag(restraintTag))
        {
            // �S���J�E���g�J�n
            countdown.Initialize(restraintTime);

            // �Փ˂����I�u�W�F�N�g���擾
            hitObject = other.gameObject;

            // ���f���̕\����Ԃ��X�V
            openTrap.SetActive(false);
            closeTrap.SetActive(true);

            // ���ʉ����Đ�
            soundEffect.PlaySoundEffect(0,0);
        }
        // �I�u�W�F�N�g�ɏՓ˂������~
        else
        {
            isMove = false;

            isSetUp = true;

        }

    }
}
