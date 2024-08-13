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
        private float staminaT = 0;  // �X�^�~�i�����̎��ԃJ�E���^
        private bool isStaminaIncreasable = true;  // �X�^�~�i���񕜉\���ǂ���
        private bool isCharging = false;  // �ːi�����ǂ���

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

            // �v���C���[���J�����̕����ɉ�]������
            Vector3 lookAtLocal = playerBodyTf.position - freeLookCamera.position;
            Vector3 lookAtLocalXZ = new(lookAtLocal.x, 0, lookAtLocal.z);
            Vector3 lookAtXZ = playerBodyTf.position + lookAtLocalXZ;
            playerBodyTf.LookAt(lookAtXZ);

            // �v���C���[��O�㍶�E�ɓ�����
            Vector2 moveValueInputted = InputGetter.Instance.Main_MoveValue2.Get<Vector2>();
            Vector3 moveValueLocalNormed = new Vector3(moveValueInputted.x, 0, moveValueInputted.y).normalized;
            Vector3 moveValueLocal = moveValueLocalNormed * (moveSpeed * Time.deltaTime);
            Vector3 moveValue = playerBodyTf.right * moveValueLocal.x + playerBodyTf.forward * moveValueLocal.z;
            playerBodyTf.localPosition += moveValue;

            // �ːi�L�[�̓��͏�Ԃɂ���āA�����\�Ȃ�΁A�ːi��Ԃ��X�V����B
            // ���ɁA�ːi�L�[�������ꂽ�Ƃ��A�X�^�~�i�̉񕜂��\�ɂ���B([A]�ɑΉ����鏈��)
            if (InputGetter.Instance.Main_ChargeValue0.Get<bool>()) { SetCharge(true); }
            else { isStaminaIncreasable = true; SetCharge(false); }

            // ��莞�Ԗ��ɃX�^�~�i�𑝌�������B
            // �ːi��ԁ@�@�@�Ȃ�A�X�^�~�i������������B������0�ɂȂ�����A[A](�X�^�~�i�̉񕜂��o���Ȃ����������ŁA)�ːi��ԂłȂ�����B
            // �ːi��ԂłȂ��Ȃ�A�X�^�~�i�𑝉�������B[A](�������A�X�^�~�i�̉񕜂��\�ȏ�Ԃ̎��̂�)
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

        // �ːi��Ԃ�؂�ւ���Btrue�Ȃ�ːi��Ԃɂ��Afalse�Ȃ�ːi��ԂłȂ�����B
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

        // �X�^�~�i�𑝂₷�B���̏����ŃX�^�~�i���ő�Ɂu�Ȃ����v���̂݁Atrue��Ԃ��B
        public bool IncreaseStamina(int diff)
        {
            if (diff <= 0) throw new System.Exception("diff�͐��ł���K�v������܂�");
            if (stamina == SO_Player.Entity.MaxStamina) return false;

            stamina += diff;
            return IsStaminaFull();
        }

        // �X�^�~�i�����炷�B���̏����ŃX�^�~�i���ŏ��Ɂu�Ȃ����v���̂݁Atrue��Ԃ��B
        public bool DecreaseStamina(int diff)
        {
            if (diff <= 0) throw new System.Exception("diff�͐��ł���K�v������܂�");
            if (stamina == 0) return false;

            stamina -= diff;
            return IsStaminaZero();
        }

        // �X�^�~�i���ő�Ȃ�true�A�����łȂ��Ȃ�false�B
        public bool IsStaminaFull() { return stamina == SO_Player.Entity.MaxStamina; }

        // �X�^�~�i���ŏ��Ȃ�true�A�����łȂ��Ȃ�false�B
        public bool IsStaminaZero() { return stamina == 0; }
    }
}