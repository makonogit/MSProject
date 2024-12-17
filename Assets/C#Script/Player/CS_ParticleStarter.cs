using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

using UnityEngine;

// �p�[�e�B�N������@�S���F�����V�S
public class CS_ParticleStarter : MonoBehaviour
{
    [SerializeField, Header("�p�[�e�B�N���̃��X�g")]
    private ParticleSystem[] particleList;
    private static ParticleSystem[] particleSystems;

    // �X�^�[�g���������p�[�e�B�N���V�X�e���̃C���f�b�N�X
    public static int startIndex = 0;

    // �J�n���郁�\�b�h
    public static void StartParticleSystemAtIndex(int index)
    {
        // �z��͈͓̔����m�F
        if (index >= 0 && index < particleSystems.Length)
        {
            // �w�肵���C���f�b�N�X�̃p�[�e�B�N���V�X�e�����~���ăN���A
            particleSystems[index].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            // �Đ����J�n
            particleSystems[index].Play();
        }
    }

    private void Start()
    {
        if (particleSystems == null)
        {
            // �U���J�[�u���R�s�[
            particleSystems = new ParticleSystem[particleList.Length];
            particleList.CopyTo(particleSystems, 0);
        }

        foreach (var ps in particleSystems)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    private void Update()
    {
        // ���ׂẴp�[�e�B�N���V�X�e�����`�F�b�N
        foreach (var ps in particleSystems)
        {
            // �p�[�e�B�N���V�X�e������~���Ă���ꍇ���Z�b�g����
            if (!ps.isPlaying && ps.time <= 0)
            {
                ps.Clear();
            }
        }
    }
}
