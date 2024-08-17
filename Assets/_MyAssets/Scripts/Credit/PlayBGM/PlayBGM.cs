using General;
using UnityEngine;

namespace Credit
{
    public sealed class PlayBGM : MonoBehaviour
    {
        [SerializeField] SoundPlayer soundPlayer;

        private void Start()
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