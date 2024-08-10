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
        [Header("タイトル")] public string Title;
        [Header("メイン")] public string Main;
    }
}