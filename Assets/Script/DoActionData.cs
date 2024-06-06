using System;
using UnityEngine;

[Serializable]
public class DoActionData
{
    public AttackState attackState;
    public float Power;
    public int StopFrame;
    public float Distance;


    public AudioClip HitAudioClip;

    public GameObject HitParticle;
    public Vector3 HitParticlePositionOffset;
    public Vector3 HitParticleScaleOffset = Vector3.one;
}