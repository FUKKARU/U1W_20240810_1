using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/SO_System", fileName = "SO_System")]
    public class SO_System : ScriptableObject
    {
        #region
        public const string PATH = "SO_System";

        private static SO_System _entity = null;
        public static SO_System Entity
        {
            get
            {
                if (_entity == null)
                {
                    _entity = Resources.Load<SO_System>(PATH);

                    if (_entity == null)
                    {
                        Debug.LogError(PATH + " not found");
                    }
                }

                return _entity;
            }
        }
        #endregion

        [SerializeField, Header("シーン遷移後、何秒待ってからボタンを有効にするか")] private float _beforeButtonDur;
        public float BeforeButtonDur => _beforeButtonDur;

        [SerializeField, Header("ボタンを押した後、何秒待って処理を行うか")] private float _afterButtonDur;
        public float AfterButtonDur => _afterButtonDur;
    }
}