using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_SoundEffect : MonoBehaviour
{
    // �Đ����鉹���t�@�C���̃��X�g
    [Header("���ʉ��ݒ�")]
    [SerializeField, Header("�I�[�f�B�I�\�[�X")]
    private AudioSource[] audioSource;
    [SerializeField, Header("�����t�@�C��")]
    private AudioClip[] audioClips;

    //**
    //* �����t�@�C�����Đ�����
    //*
    //* in�F�Đ����鉹���t�@�C���̃C���f�b�N�X
    //* out:����
    //**
    public void PlaySoundEffect(int indexSource, int indexClip)
    {
        // �T�E���h�G�t�F�N�g���Đ�
        if (CanPlaySound(indexSource, indexClip))
        {
            audioSource[indexSource].clip = audioClips[indexClip];
            audioSource[indexSource].Play();
        }
    }

    //**
    //* �����t�@�C�����~����
    //*
    //* in�F����
    //* out:����
    //**
    public void StopPlayingSound(int indexSource)
    {
        // �T�E���h�G�t�F�N�g���~
        if (audioSource[indexSource].isPlaying)
        {
            audioSource[indexSource].Stop();
        }
    }

    //**
    //* �����t�@�C�����Đ��\���`�F�b�N����
    //*
    //* in�F�Đ����鉹���t�@�C���̃C���f�b�N�X
    //* out:�Đ��\���`�F�b�N
    //**
    public bool CanPlaySound(int indexSource, int indexClip)
    {
        return audioSource[indexSource] != null
               && audioClips != null
               && indexClip >= 0
               && indexClip < audioClips.Length
               && (!audioSource[indexSource].isPlaying || audioSource[indexSource].clip != audioClips[indexClip]);
    }
}
