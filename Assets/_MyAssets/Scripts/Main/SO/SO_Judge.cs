using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/SO_Judge", fileName = "SO_Judge")]
    public class SO_Judge : ScriptableObject
    {
        #region
        public const string PATH = "SO_Judge";

        private static SO_Judge _entity = null;
        public static SO_Judge Entity
        {
            get
            {
                if (_entity == null)
                {
                    _entity = Resources.Load<SO_Judge>(PATH);

                    if (_entity == null)
                    {
                        Debug.LogError(PATH + " not found");
                    }
                }

                return _entity;
            }
        }
        #endregion

        [SerializeField, Header("ƒQ[ƒ€I—¹‚Ü‚Å‚ÌŽžŠÔ§ŒÀ[•b]")] private float _timeLimit;
        public float TimeLimit => _timeLimit;

        [SerializeField, Header("Directional Light‚ÌŒX‚«x‚Ì•Ï‰»(ŠJŽn ¨ I—¹)")] private Vector2 _sunAngles;
        public float SunStartAngle => _sunAngles.x;
        public float SunEndAngle => _sunAngles.y;
    }
}