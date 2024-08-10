using SO;
using UnityEngine;

namespace General
{
    public static class PlaySound
    {
        // �^����ꂽAudioSource��p���āABGM/SE���Đ�����
        public static void Raise(this AudioSource source, AudioClip clip, bool sType, float volume = 1, float pitch = 1)
        {
            source.volume = volume;
            source.pitch = pitch;

            if (sType == SType.BGM)
            {
                source.clip = clip;
                source.outputAudioMixerGroup = SO_Sound.Entity.AMGroupBGM;
                source.playOnAwake = true;
                source.loop = true;
                source.Play();
            }
            else
            {
                source.outputAudioMixerGroup = SO_Sound.Entity.AMGroupSE;
                source.playOnAwake = true;
                source.loop = false;
                source.PlayOneShot(clip);
            }
        }
    }

    // �T�E���h�̎��(BGM or SE)
    public static class SType
    {
        public static bool BGM = true;
        public static bool SE = false;
    }
}