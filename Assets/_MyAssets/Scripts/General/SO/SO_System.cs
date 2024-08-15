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

        [SerializeField, Header("ƒ{ƒ^ƒ“‚ð‰Ÿ‚µ‚½ŒãA‰½•b‘Ò‚Â‚©")] private float _buttonDur;
        public float ButtonDur => _buttonDur;
    }
}