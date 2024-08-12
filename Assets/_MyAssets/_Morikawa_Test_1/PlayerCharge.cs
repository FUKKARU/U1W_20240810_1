using UnityEngine;

namespace Morikawa_Test_1
{
    public class PlayerCharge : MonoBehaviour
    {
        [SerializeField] private PlayerMove playerMove = null;

        [SerializeField, Header("突進時に移動速度を何倍にするか")] private float chargeCoef;
        [SerializeField, Header("最大スタミナ")] private int maxStamina;
        [SerializeField, Header("何秒ごとにスタミナの増加/減少を行うか")] private float staminaChangeDur;
        [SerializeField, Header("スタミナの増加/減少時に、どれだけ変化させるか")] private int staminaChangeDiff;

        private PlayerChargeInfo playerChargeInfo = null;

        private void Start()
        {
            if (playerMove == null) throw new System.Exception("PlayerMoveが設定されていません");

            playerChargeInfo = new PlayerChargeInfo(chargeCoef, maxStamina, staminaChangeDur, staminaChangeDiff);
        }

        private void Update()
        {
            playerChargeInfo.Update(IA.InputGetter.Instance, playerMove);
        }
    }

    public class PlayerChargeInfo
    {
        private readonly float chargeCoef = 0;  // 突進時、移動速度が何倍になるか
        private readonly PlayerStaminaInfo staminaInfo;  // スタミナ情報
        public PlayerStaminaInfo StaminaInfo => staminaInfo;
        private readonly float staminaChangeDur = 0;  // 何秒ごとにスタミナの増加/減少を行うか
        private readonly int staminaChangeDiff = 0;  // スタミナの増加/減少時に、どれだけ変化させるか
        private float staminaT = 0;  // スタミナの時間カウンタ
        private bool isStaminaIncreasable = true;  // スタミナの回復を始められるかどうか
        private bool isCharging = false;  // 突進中かどうか

        public PlayerChargeInfo(float chargeCoef, int maxStamina, float staminaChangeDur, int staminaChangeDiff)
        {
            if (chargeCoef <= 1) throw new System.Exception("coefは1より大きい必要があります");
            if (maxStamina <= 5) throw new System.Exception("maxStaminaが極端に小さいです");
            if (staminaChangeDur <= 0) throw new System.Exception("staminaChangeDurは正である必要があります");
            if (staminaChangeDiff <= 0) throw new System.Exception("staminaChangeDiffは正である必要があります");

            this.chargeCoef = chargeCoef;
            this.staminaInfo = new PlayerStaminaInfo(maxStamina);
            this.staminaChangeDur = staminaChangeDur;
            this.staminaChangeDiff = staminaChangeDiff;
        }

        /// <summary>
        /// 毎フレーム呼び出すこと！
        /// </summary>
        public void Update(IA.InputGetter inputGetter, PlayerMove playerMove)
        {
            // 突進キーの入力状態によって、もし可能ならば、突進状態を更新する。
            // 突進キーが離されたとき、スタミナの回復を可能にする。([A]に対応する処理)
            if (inputGetter.Main_ChargeValue0.Get<bool>())
            {
                Activate(playerMove);
            }
            else
            {
                if (!isStaminaIncreasable) isStaminaIncreasable = true;

                Deactivate(playerMove);
            }

            // 一定時間毎にスタミナを増減させる。
            // 突進状態　　　なら、スタミナを減少させる。もしも0になったら、[A](スタミナの回復を出来なくしたうえで、)突進状態でなくする。
            // 突進状態でないなら、スタミナを増加させる。[A](ただし、スタミナの回復が可能な状態の時のみ。)
            staminaT += Time.deltaTime;
            if (staminaT >= staminaChangeDur)
            {
                staminaT -= staminaChangeDur;

                if (isCharging)
                {
                    bool ret = staminaInfo.DecreaseStamina(staminaChangeDiff);
                    if (ret)
                    {
                        isStaminaIncreasable = false;
                        Deactivate(playerMove);
                    }
                }
                else
                {
                    if (isStaminaIncreasable) staminaInfo.IncreaseStamina(staminaChangeDiff);
                }
            }
        }

        /// <summary>
        /// 突進状態にする。PlayerMoveの移動速度を更新する
        /// </summary>
        private void Activate(PlayerMove playerMove)
        {
            // 突進状態なら、何もしない
            if (isCharging) return;

            // スタミナが最小なら、何もしない
            if (staminaInfo.IsStaminaZero()) return;

            isCharging = true;
            playerMove.MoveSpeed *= chargeCoef;
            staminaT = 0;
        }

        /// <summary>
        /// 突進状態でなくする。PlayerMoveの移動速度を更新する
        /// </summary>
        private void Deactivate(PlayerMove playerMove)
        {
            // 突進状態でないなら、何もしない
            if (!isCharging) return;

            isCharging = false;
            playerMove.MoveSpeed /= chargeCoef;
            staminaT = 0;
        }
    }

    public class PlayerStaminaInfo
    {
        private readonly int maxStamina = 0;  // 最大スタミナ
        private int _stamina = 0;  // 現在のスタミナ
        private int stamina
        {
            get { return _stamina; }
            set { _stamina = Mathf.Clamp(value, 0, maxStamina); }
        }
        public int Stamina => stamina;

        public PlayerStaminaInfo(int maxStamina)
        {
            if (maxStamina <= 5) throw new System.Exception("maxStaminaが極端に小さいです");
            this.maxStamina = maxStamina;
            stamina = maxStamina;
        }

        /// <summary>
        /// スタミナを増やす。この処理でスタミナが最大に「なった」時のみ、trueを返す。
        /// </summary>
        public bool IncreaseStamina(int diff)
        {
            if (diff <= 0) throw new System.Exception("diffは正である必要があります");

            // 既に最大なら処理ストップ
            if (stamina == maxStamina) return false;

            // スタミナを増やす
            stamina += diff;
            return stamina == maxStamina;
        }

        /// <summary>
        /// スタミナを減らす。この処理でスタミナが最小に「なった」時のみ、trueを返す。
        /// </summary>
        public bool DecreaseStamina(int diff)
        {
            if (diff <= 0) throw new System.Exception("diffは正である必要があります");

            // 既に最小なら処理ストップ
            if (stamina == 0) return false;

            // スタミナを減らす
            stamina -= diff;
            return stamina == 0;
        }

        /// <summary>
        /// スタミナが最大ならtrue、そうでないならfalse
        /// </summary>
        public bool IsStaminaFull()
        {
            return stamina == maxStamina;
        }

        /// <summary>
        /// スタミナが最小ならtrue、そうでないならfalse
        /// </summary>
        public bool IsStaminaZero()
        {
            return stamina == 0;
        }
    }
}