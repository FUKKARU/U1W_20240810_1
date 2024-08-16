using SO;
using UnityEngine;

namespace General
{
    public static class PlaySound
    {
        // 与えられたAudioSourceを用いて、BGM/SEを再生する
        public static void Raise(this AudioSource source, AudioClip clip, bool sType, float volume = 1, float pitch = 1)
        {
            source.volume = volume;
            source.pitch = pitch;

            if (sType == SType.BGM)
            {
                source.clip = clip;
                source.outputAudioMixerGroup = SO_Sound.Entity.BGMAMGroup;
                source.playOnAwake = true;
                source.loop = true;
                source.Play();
            }
            else
            {
                source.outputAudioMixerGroup = SO_Sound.Entity.SEAMGroup;
                source.playOnAwake = true;
                source.loop = false;
                source.PlayOneShot(clip);
            }
        }
    }

    // サウンドの種類(BGM or SE)
    public static class SType
    {
        public static bool BGM = true;
        public static bool SE = false;
    }
}