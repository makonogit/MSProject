using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �N���t�g�A�C�e���@�o���A
// �����V�S
public class CS_Barrier : MonoBehaviour
{
    // �ݒ荀��
    [SerializeField, Header("�L������")]
    private float validityTime = 3f;

    // �I�[�f�B�I
    private CS_SoundEffect soundEffect;

    // ���Ԍv��
    private CS_Countdown countdown;

    // Start is called before the first frame update
    void Start()
    {
        // ���Ԍv���p�I�u�W�F�N�g���쐬
        countdown = gameObject.AddComponent<CS_Countdown>();

        // �q�I�u�W�F�N�g���擾
        soundEffect = transform.GetChild(0).gameObject.GetComponent<CS_SoundEffect>();

        // ���ʉ����Đ�
        soundEffect.PlaySoundEffect(0, 0);

        // �L�����Ԃ̃J�E���g�J�n
        countdown.Initialize(validityTime);
    }

    // Update is called once per frame
    void Update()
    {
        // �g�p��A�����j��
        if (countdown.IsCountdownFinished())
        {
            Destroy(gameObject);
        }
    }
}
