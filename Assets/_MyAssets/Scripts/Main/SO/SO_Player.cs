using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/SO_Player", fileName = "SO_Player")]
    public class SO_Player : ScriptableObject
    {
        #region
        public const string PATH = "SO_Player";

        private static SO_Player _entity = null;
        public static SO_Player Entity
        {
            get
            {
                if (_entity == null)
                {
                    _entity = Resources.Load<SO_Player>(PATH);

                    if (_entity == null)
                    {
                        Debug.LogError(PATH + " not found");
                    }
                }

                return _entity;
            }
        }
        #endregion

        [SerializeField, Header("�ړ��X�s�[�h\n1.�ʏ�\n2.�ːi")] private Vector2 _moveSpeed;
        public float NormalSpeed => _moveSpeed.x;
        public float ChargeSpeed => _moveSpeed.y;

        [SerializeField, Header("�ő�X�^�~�i")] private int _maxStamina;
        public int MaxStamina => _maxStamina;

        [SerializeField, Header("�X�^�~�i�̑��������b���Ƃɍs����")] private float _staminaChangeDur;
        public float StaminaChangeDur => _staminaChangeDur;

        [SerializeField, Header("�X�^�~�i��1�񂲂Ƃ̑�����\n1.������\n2.������")] private Vector2Int _staminaChangeDiff;
        public int StaminaIncreaseDiff => _staminaChangeDiff.x;
        public int StaminaDecreaseDiff => _staminaChangeDiff.y;
    }
}