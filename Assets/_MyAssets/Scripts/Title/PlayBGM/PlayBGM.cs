using General;
using UnityEngine;

namespace Title
{
    public sealed class PlayBGM : MonoBehaviour
    {
        [SerializeField] SoundPlayer soundPlayer;

        private void OnEnable()
        {
            if (!soundPlayer) return;

            soundPlayer.Play(SoundType.Title_TitleBGM);
        }

        private void OnDisable()
        {
            soundPlayer = null;
        }
    }
}