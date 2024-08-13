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

        [SerializeField, Header("移動スピード\n1.通常\n2.突進")] private Vector2 _moveSpeed;
        public float NormalSpeed => _moveSpeed.x;
        public float ChargeSpeed => _moveSpeed.y;

        [SerializeField, Header("最大スタミナ")] private int _maxStamina;
        public int MaxStamina => _maxStamina;

        [SerializeField, Header("スタミナの増減を何秒ごとに行うか")] private float _staminaChangeDur;
        public float StaminaChangeDur => _staminaChangeDur;

        [SerializeField, Header("スタミナの1回ごとの増減量\n1.増加量\n2.減少量")] private Vector2Int _staminaChangeDiff;
        public int StaminaIncreaseDiff => _staminaChangeDiff.x;
        public int StaminaDecreaseDiff => _staminaChangeDiff.y;
    }
}