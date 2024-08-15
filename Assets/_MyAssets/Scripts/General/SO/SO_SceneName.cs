using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/SO_SceneName", fileName = "SO_SceneName")]
    public class SO_SceneName : ScriptableObject
    {
        #region
        public const string PATH = "SO_SceneName";

        private static SO_SceneName _entity = null;
        public static SO_SceneName Entity
        {
            get
            {
                if (_entity == null)
                {
                    _entity = Resources.Load<SO_SceneName>(PATH);

                    if (_entity == null)
                    {
                        Debug.LogError(PATH + " not found");
                    }
                }

                return _entity;
            }
        }
        #endregion

        [Header("シーン名")]
        [Space(25)]

        [SerializeField, Header("タイトル")] private string _title;
        public string Title => _title;

        [SerializeField, Header("クレジット")] private string _credit;
        public string Credit => _credit;

        [SerializeField, Header("メイン")] private string _main;
        public string Main => _main;
    }
}