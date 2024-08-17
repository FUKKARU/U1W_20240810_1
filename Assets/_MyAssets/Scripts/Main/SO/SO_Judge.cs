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

        [SerializeField, Header("ゲーム終了までの時間制限[秒]")] private float _timeLimit;
        public float TimeLimit => _timeLimit;

        [SerializeField, Header("Directional Lightの傾きxの変化(開始 → 終了)")] private Vector2 _sunAngles;
        public float SunStartAngle => _sunAngles.x;
        public float SunEndAngle => _sunAngles.y;

        [SerializeField, Range(1, 5), Header("ゲームクリアに必要な油揚げの個数")] private int _aburaageGoalNum;
        public int AburaageGoalNum;

        [SerializeField, Header("兵十にどのくらい近づいたら、クリアの判定を行うことができるか")] private float _goalDistance;
        public float GoalDistance => _goalDistance;
    }
}