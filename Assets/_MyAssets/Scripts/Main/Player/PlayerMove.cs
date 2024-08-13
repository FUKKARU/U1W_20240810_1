using UnityEngine;
using SO;
using IA;

namespace Main.Player
{
    public sealed class PlayerMove : MonoBehaviour
    {
        [SerializeField] private Transform playerBodyTf;
        [SerializeField] private Rigidbody playerBodyRb;
        [SerializeField] private Transform freeLookCamera;

        private PlayerMoveBhv impl;

        private void OnEnable()
        {
            impl = new(playerBodyTf, playerBodyRb, freeLookCamera);
        }

        private void OnDisable()
        {
            impl.Dispose();
            impl = null;
        }

        private void Update()
        {
            impl.Update();
        }
    }

    internal sealed class PlayerMoveBhv : System.IDisposable
    {
        private Transform playerBodyTf;
        private Rigidbody playerBodyRb;
        private Transform freeLookCamera;

        private readonly PlayerStamina playerStamina;

        private float moveSpeed = SO_Player.Entity.NormalSpeed;
        private float staminaT = 0;  // スタミナ増減の時間カウンタ
        private bool isStaminaIncreasable = true;  // スタミナが回復可能かどうか
        private bool isCharging = false;  // 突進中かどうか

        public PlayerMoveBhv(Transform playerBodyTf, Rigidbody playerBodyRb, Transform freeLookCamera)
        {
            this.playerBodyTf = playerBodyTf;
            this.playerBodyRb = playerBodyRb;
            this.freeLookCamera = freeLookCamera;

            playerStamina = new();
        }

        public void Dispose()
        {
            playerBodyTf = null;
            playerBodyRb = null;
            freeLookCamera = null;
        }

        public void Update()
        {
            if (!playerBodyTf) return;
            if (!playerBodyRb) return;
            if (!freeLookCamera) return;

            playerBodyRb.velocity = Vector3.zero;

            // プレイヤーをカメラの方向に回転させる
            Vector3 lookAtLocal = playerBodyTf.position - freeLookCamera.position;
            Vector3 lookAtLocalXZ = new(lookAtLocal.x, 0, lookAtLocal.z);
            Vector3 lookAtXZ = playerBodyTf.position + lookAtLocalXZ;
            playerBodyTf.LookAt(lookAtXZ);

            // プレイヤーを前後左右に動かす
            Vector2 moveValueInputted = InputGetter.Instance.Main_MoveValue2.Get<Vector2>();
            Vector3 moveValueLocalNormed = new Vector3(moveValueInputted.x, 0, moveValueInputted.y).normalized;
            Vector3 moveValueLocal = moveValueLocalNormed * (moveSpeed * Time.deltaTime);
            Vector3 moveValue = playerBodyTf.right * moveValueLocal.x + playerBodyTf.forward * moveValueLocal.z;
            playerBodyTf.localPosition += moveValue;

            // 突進キーの入力状態によって、もし可能ならば、突進状態を更新する。
            // 特に、突進キーが離されたとき、スタミナの回復を可能にする。([A]に対応する処理)
            if (InputGetter.Instance.Main_ChargeValue0.Get<bool>()) { SetCharge(true); }
            else { isStaminaIncreasable = true; SetCharge(false); }

            // 一定時間毎にスタミナを増減させる。
            // 突進状態　　　なら、スタミナを減少させる。もしも0になったら、[A](スタミナの回復を出来なくしたうえで、)突進状態でなくする。
            // 突進状態でないなら、スタミナを増加させる。[A](ただし、スタミナの回復が可能な状態の時のみ)
            staminaT += Time.deltaTime;
            if (staminaT >= SO_Player.Entity.StaminaChangeDur)
            {
                staminaT -= SO_Player.Entity.StaminaChangeDur;

                if (isCharging)
                {
                    bool ret = playerStamina.DecreaseStamina(SO_Player.Entity.StaminaDecreaseDiff);
                    if (ret) { isStaminaIncreasable = false; SetCharge(false); }
                }
                else
                {
                    if (isStaminaIncreasable) playerStamina.IncreaseStamina(SO_Player.Entity.StaminaIncreaseDiff);
                }
            }
        }

        // 突進状態を切り替える。trueなら突進状態にし、falseなら突進状態でなくする。
        private void SetCharge(bool isCharge)
        {
            if (isCharge)
            {
                if (isCharging) return;
                if (playerStamina.IsStaminaZero()) return;

                isCharging = true;
                moveSpeed = SO_Player.Entity.ChargeSpeed;
                staminaT = 0;
            }
            else
            {
                if (!isCharging) return;

                isCharging = false;
                moveSpeed = SO_Player.Entity.NormalSpeed;
                staminaT = 0;
            }
        }
    }

    internal sealed class PlayerStamina
    {
        private int _stamina = 0;
        private int stamina
        {
            get { return _stamina; }
            set { _stamina = Mathf.Clamp(value, 0, SO_Player.Entity.MaxStamina); }
        }

        public PlayerStamina()
        {
            stamina = SO_Player.Entity.MaxStamina;
        }

        // スタミナを増やす。この処理でスタミナが最大に「なった」時のみ、trueを返す。
        public bool IncreaseStamina(int diff)
        {
            if (diff <= 0) throw new System.Exception("diffは正である必要があります");
            if (stamina == SO_Player.Entity.MaxStamina) return false;

            stamina += diff;
            return IsStaminaFull();
        }

        // スタミナを減らす。この処理でスタミナが最小に「なった」時のみ、trueを返す。
        public bool DecreaseStamina(int diff)
        {
            if (diff <= 0) throw new System.Exception("diffは正である必要があります");
            if (stamina == 0) return false;

            stamina -= diff;
            return IsStaminaZero();
        }

        // スタミナが最大ならtrue、そうでないならfalse。
        public bool IsStaminaFull() { return stamina == SO_Player.Entity.MaxStamina; }

        // スタミナが最小ならtrue、そうでないならfalse。
        public bool IsStaminaZero() { return stamina == 0; }
    }
}