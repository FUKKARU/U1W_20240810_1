using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/SO_Sound", fileName = "SO_Sound")]
    public class SO_Sound : ScriptableObject
    {
        #region
        public const string PATH = "SO_Sound";

        private static SO_Sound _entity = null;
        public static SO_Sound Entity
        {
            get
            {
                if (_entity == null)
                {
                    _entity = Resources.Load<SO_Sound>(PATH);

                    if (_entity == null)
                    {
                        Debug.LogError(PATH + " not found");
                    }
                }

                return _entity;
            }
        }
        #endregion

        [Header("AudioMixer")] public AudioMixer AudioMixer;
        [Header("Master��AudioMixerGroup")] public AudioMixerGroup AMGroupMaster;
        [Header("BGM��AudioMixerGroup")] public AudioMixerGroup AMGroupBGM;
        [Header("SE��AudioMixerGroup")] public AudioMixerGroup AMGroupSE;
        [Space(50)]
        [Header("BGM")]
        [Header("�^�C�g��")] public AudioClip TitleBGM;
        [Header("�Q�[����")] public AudioClip MainBGM;
        [Space(50)]
        [Header("SE")]
        [Header("�e�X�g1")] public AudioClip Test1SE;
    }
}