using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_SoundEffect : MonoBehaviour
{
    // 再生する音声ファイルのリスト
    [Header("効果音設定")]
    [SerializeField, Header("オーディオソース")]
    private AudioSource[] audioSource;
    [SerializeField, Header("音声ファイル")]
    private AudioClip[] audioClips;

    //**
    //* 音声ファイルを再生する
    //*
    //* in：再生する音声ファイルのインデックス
    //* out:無し
    //**
    public void PlaySoundEffect(int indexSource, int indexClip)
    {
        // サウンドエフェクトを再生
        if (CanPlaySound(indexSource, indexClip))
        {
            audioSource[indexSource].clip = audioClips[indexClip];
            audioSource[indexSource].Play();
        }
    }

    //**
    //* 音声ファイルを停止する
    //*
    //* in：無し
    //* out:無し
    //**
    public void StopPlayingSound(int indexSource)
    {
        // サウンドエフェクトを停止
        if (audioSource[indexSource].isPlaying)
        {
            audioSource[indexSource].Stop();
        }
    }

    //**
    //* 音声ファイルが再生可能かチェックする
    //*
    //* in：再生する音声ファイルのインデックス
    //* out:再生可能かチェック
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
