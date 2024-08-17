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

        [SerializeField, Header("タイトル")] private AudioClip _titleBGM;
        public AudioClip TitleBGM => _titleBGM;

        [SerializeField, Header("ゲーム内\n1.神社\n2.森林\n3.村落")] private List<AudioClip> _mainBGMList;
        public AudioClip MainShrineBGM => _mainBGMList[0];
        public AudioClip MainForestBGM => _mainBGMList[1];
        public AudioClip MainVillageBGM => _mainBGMList[2];

        [Space(25)]
        [Header("SE")]

        [SerializeField, Header("ボタンのクリック")] private AudioClip _buttonClickSE;
        public AudioClip ButtonClickSE => _buttonClickSE;

        [SerializeField, Header("アイテム入手")] private AudioClip _itemGetSE;
        public AudioClip ItemGetSE => _itemGetSE;

        [SerializeField, Header("交易完了")] private AudioClip _tradeDoneSE;
        public AudioClip TradeDoneSE => _tradeDoneSE;
    }
}