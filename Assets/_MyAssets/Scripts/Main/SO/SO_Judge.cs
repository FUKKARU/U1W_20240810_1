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

        [SerializeField, Header("�Q�[���I���܂ł̎��Ԑ���[�b]")] private float _timeLimit;
        public float TimeLimit => _timeLimit;

        [SerializeField, Header("Directional Light�̌X��x�̕ω�(�J�n �� �I��)")] private Vector2 _sunAngles;
        public float SunStartAngle => _sunAngles.x;
        public float SunEndAngle => _sunAngles.y;

        [SerializeField, Range(1, 5), Header("�Q�[���N���A�ɕK�v�Ȗ��g���̌�")] private int _aburaageGoalNum;
        public int AburaageGoalNum => _aburaageGoalNum;

        [SerializeField, Header("���\�ɂǂ̂��炢�߂Â�����A�N���A�̔�����s�����Ƃ��ł��邩")] private float _goalDistance;
        public float GoalDistance => _goalDistance;

        [SerializeField, TextArea(1, 1000), Header("�Q�[���N���A�̃��b�Z�[�W")] private string _gameClearMessage;
        public string GameClearMessage => _gameClearMessage;

        [SerializeField, TextArea(1, 1000), Header("�Q�[���I�[�o�[�̃��b�Z�[�W")] private string _gameOverMessage;
        public string GameOverMessage => _gameOverMessage;

        [SerializeField, Header("�Q�[�����I�����Ă��牽�b��Ƀ^�C�g���ɖ߂邩")] private float _beforeGoToTitleDur;
        public float BeforeGoToTitleDur => _beforeGoToTitleDur;
    }
}