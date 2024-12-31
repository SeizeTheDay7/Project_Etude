using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSound : MonoBehaviour
{
    public AudioClip[] hitSounds;

    public AudioClip RandomHitSound()
    {
        int index = Random.Range(0, hitSounds.Length);
        return hitSounds[index];
    }
}
