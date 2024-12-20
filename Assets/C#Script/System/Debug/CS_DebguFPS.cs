using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CS_DebguFPS : MonoBehaviour
{

    private float deltaTime = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        // �t���[���Ԃ̌o�ߎ��Ԃ��擾
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    private void Update()
    {
        // �t���[���Ԃ̌o�ߎ��Ԃ��v�Z
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }
    void OnGUI()
    {
        // ���݂�FPS���v�Z
        float fps = 1.0f / deltaTime;

        // �`��p�̃e�L�X�g
        string text = string.Format("{0:0.} FPS", fps);

        // �X�^�C����ݒ�
        GUIStyle style = new GUIStyle
        {
            fontSize = 24, // �t�H���g�T�C�Y
            normal = { textColor = Color.white } // �e�L�X�g�F
        };

        // �\���ʒu
        Rect rect = new Rect(10, 10, 200, 40); // ��ʍ���
        GUI.Label(rect, text, style);
    }
}
    
