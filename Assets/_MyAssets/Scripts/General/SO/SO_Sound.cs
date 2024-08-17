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

        [SerializeField, Header("AudioMixer")] private AudioMixer _audioMixer;
        public AudioMixer AudioMixer => _audioMixer;

        [SerializeField, Header("AudioMixerGroup\n1.Master\n2.BGM\n3.SE")] private List<AudioMixerGroup> _audioMixerGroupList;
        public AudioMixerGroup MasterAMGroup => _audioMixerGroupList[0];
        public AudioMixerGroup BGMAMGroup => _audioMixerGroupList[1];
        public AudioMixerGroup SEAMGroup => _audioMixerGroupList[2];

        [Space(25)]
        [Header("BGM")]

        [SerializeField, Header("�^�C�g��")] private AudioClip _titleBGM;
        public AudioClip TitleBGM => _titleBGM;

        [SerializeField, Header("�Q�[����\n1.�_��\n2.�X��\n3.����")] private List<AudioClip> _mainBGMList;
        public AudioClip MainShrineBGM => _mainBGMList[0];
        public AudioClip MainForestBGM => _mainBGMList[1];
        public AudioClip MainVillageBGM => _mainBGMList[2];

        [Space(25)]
        [Header("SE")]

        [SerializeField, Header("�{�^���̃N���b�N")] private AudioClip _buttonClickSE;
        public AudioClip ButtonClickSE => _buttonClickSE;

        [SerializeField, Header("�A�C�e������")] private AudioClip _itemGetSE;
        public AudioClip ItemGetSE => _itemGetSE;

        [SerializeField, Header("���Պ���")] private AudioClip _tradeDoneSE;
        public AudioClip TradeDoneSE => _tradeDoneSE;
    }
}