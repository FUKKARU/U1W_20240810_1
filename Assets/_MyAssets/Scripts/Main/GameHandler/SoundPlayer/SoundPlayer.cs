using UnityEngine;
using SO;

namespace General
{
    internal enum SoundType
    {
        Title_TitleBGM,
        Main_ShrineBGM,
        Main_ForestBGM,
        Main_VillageBGM,

        General_ClickSE,
        Main_ItemGetSE,
        Main_TradeDoneSE,
    }

    public sealed class SoundPlayer : MonoBehaviour
    {
        [SerializeField] AudioSource audioSource;

        private SoundPlayerBhv impl;

        private void OnEnable()
        {
            impl = new(audioSource);
        }

        private void OnDisable()
        {
            impl.Dispose();
            impl = null;
        }

        private void Update()
        {
            impl.Update();
        }

        internal void Play(SoundType soundType)
        {
            impl.Play(soundType);
        }
    }

    internal sealed class SoundPlayerBhv : System.IDisposable
    {
        private AudioSource audioSource;

        internal SoundPlayerBhv(AudioSource audioSource)
        {
            this.audioSource = audioSource;
        }

        public void Dispose()
        {
            audioSource = null;
        }

        internal void Update()
        {
            if (!audioSource) return;
        }

        internal void Play(SoundType soundType)
        {
            if (!audioSource) return;

            System.Action action = soundType switch
            {
                SoundType.Title_TitleBGM => () => audioSource.Raise(SO_Sound.Entity.TitleBGM, SType.BGM),
                SoundType.Main_ShrineBGM => () => audioSource.Raise(SO_Sound.Entity.MainShrineBGM, SType.BGM),
                SoundType.Main_ForestBGM => () => audioSource.Raise(SO_Sound.Entity.MainForestBGM, SType.BGM),
                SoundType.Main_VillageBGM => () => audioSource.Raise(SO_Sound.Entity.MainVillageBGM, SType.BGM),
                SoundType.General_ClickSE => () => audioSource.Raise(SO_Sound.Entity.ButtonClickSE, SType.SE),
                SoundType.Main_ItemGetSE => () => audioSource.Raise(SO_Sound.Entity.ItemGetSE, SType.SE),
                SoundType.Main_TradeDoneSE => () => audioSource.Raise(SO_Sound.Entity.TradeDoneSE, SType.SE),
                _ => () => throw new System.Exception("–³Œø‚ÈŽí—Þ‚Å‚·")
            };

            action();
        }
    }
}