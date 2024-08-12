using UnityEngine;

namespace Morikawa_Test_1
{
    public class PlayerCharge : MonoBehaviour
    {
        [SerializeField] private PlayerMove playerMove = null;

        [SerializeField, Header("�ːi���Ɉړ����x�����{�ɂ��邩")] private float chargeCoef;
        [SerializeField, Header("�ő�X�^�~�i")] private int maxStamina;
        [SerializeField, Header("���b���ƂɃX�^�~�i�̑���/�������s����")] private float staminaChangeDur;
        [SerializeField, Header("�X�^�~�i�̑���/�������ɁA�ǂꂾ���ω������邩")] private int staminaChangeDiff;

        private PlayerChargeInfo playerChargeInfo = null;

        private void Start()
        {
            if (playerMove == null) throw new System.Exception("PlayerMove���ݒ肳��Ă��܂���");

            playerChargeInfo = new PlayerChargeInfo(chargeCoef, maxStamina, staminaChangeDur, staminaChangeDiff);
        }

        private void Update()
        {
            playerChargeInfo.Update(IA.InputGetter.Instance, playerMove);
        }
    }

    public class PlayerChargeInfo
    {
        private readonly float chargeCoef = 0;  // �ːi���A�ړ����x�����{�ɂȂ邩
        private readonly PlayerStaminaInfo staminaInfo;  // �X�^�~�i���
        public PlayerStaminaInfo StaminaInfo => staminaInfo;
        private readonly float staminaChangeDur = 0;  // ���b���ƂɃX�^�~�i�̑���/�������s����
        private readonly int staminaChangeDiff = 0;  // �X�^�~�i�̑���/�������ɁA�ǂꂾ���ω������邩
        private float staminaT = 0;  // �X�^�~�i�̎��ԃJ�E���^
        private bool isStaminaIncreasable = true;  // �X�^�~�i�̉񕜂��n�߂��邩�ǂ���
        private bool isCharging = false;  // �ːi�����ǂ���

        public PlayerChargeInfo(float chargeCoef, int maxStamina, float staminaChangeDur, int staminaChangeDiff)
        {
            if (chargeCoef <= 1) throw new System.Exception("coef��1���傫���K�v������܂�");
            if (maxStamina <= 5) throw new System.Exception("maxStamina���ɒ[�ɏ������ł�");
            if (staminaChangeDur <= 0) throw new System.Exception("staminaChangeDur�͐��ł���K�v������܂�");
            if (staminaChangeDiff <= 0) throw new System.Exception("staminaChangeDiff�͐��ł���K�v������܂�");

            this.chargeCoef = chargeCoef;
            this.staminaInfo = new PlayerStaminaInfo(maxStamina);
            this.staminaChangeDur = staminaChangeDur;
            this.staminaChangeDiff = staminaChangeDiff;
        }

        /// <summary>
        /// ���t���[���Ăяo�����ƁI
        /// </summary>
        public void Update(IA.InputGetter inputGetter, PlayerMove playerMove)
        {
            // �ːi�L�[�̓��͏�Ԃɂ���āA�����\�Ȃ�΁A�ːi��Ԃ��X�V����B
            // �ːi�L�[�������ꂽ�Ƃ��A�X�^�~�i�̉񕜂��\�ɂ���B([A]�ɑΉ����鏈��)
            if (inputGetter.Main_ChargeValue0.Get<bool>())
            {
                Activate(playerMove);
            }
            else
            {
                if (!isStaminaIncreasable) isStaminaIncreasable = true;

                Deactivate(playerMove);
            }

            // ��莞�Ԗ��ɃX�^�~�i�𑝌�������B
            // �ːi��ԁ@�@�@�Ȃ�A�X�^�~�i������������B������0�ɂȂ�����A[A](�X�^�~�i�̉񕜂��o���Ȃ����������ŁA)�ːi��ԂłȂ�����B
            // �ːi��ԂłȂ��Ȃ�A�X�^�~�i�𑝉�������B[A](�������A�X�^�~�i�̉񕜂��\�ȏ�Ԃ̎��̂݁B)
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
        /// �ːi��Ԃɂ���BPlayerMove�̈ړ����x���X�V����
        /// </summary>
        private void Activate(PlayerMove playerMove)
        {
            // �ːi��ԂȂ�A�������Ȃ�
            if (isCharging) return;

            // �X�^�~�i���ŏ��Ȃ�A�������Ȃ�
            if (staminaInfo.IsStaminaZero()) return;

            isCharging = true;
            playerMove.MoveSpeed *= chargeCoef;
            staminaT = 0;
        }

        /// <summary>
        /// �ːi��ԂłȂ�����BPlayerMove�̈ړ����x���X�V����
        /// </summary>
        private void Deactivate(PlayerMove playerMove)
        {
            // �ːi��ԂłȂ��Ȃ�A�������Ȃ�
            if (!isCharging) return;

            isCharging = false;
            playerMove.MoveSpeed /= chargeCoef;
            staminaT = 0;
        }
    }

    public class PlayerStaminaInfo
    {
        private readonly int maxStamina = 0;  // �ő�X�^�~�i
        private int _stamina = 0;  // ���݂̃X�^�~�i
        private int stamina
        {
            get { return _stamina; }
            set { _stamina = Mathf.Clamp(value, 0, maxStamina); }
        }
        public int Stamina => stamina;

        public PlayerStaminaInfo(int maxStamina)
        {
            if (maxStamina <= 5) throw new System.Exception("maxStamina���ɒ[�ɏ������ł�");
            this.maxStamina = maxStamina;
            stamina = maxStamina;
        }

        /// <summary>
        /// �X�^�~�i�𑝂₷�B���̏����ŃX�^�~�i���ő�Ɂu�Ȃ����v���̂݁Atrue��Ԃ��B
        /// </summary>
        public bool IncreaseStamina(int diff)
        {
            if (diff <= 0) throw new System.Exception("diff�͐��ł���K�v������܂�");

            // ���ɍő�Ȃ珈���X�g�b�v
            if (stamina == maxStamina) return false;

            // �X�^�~�i�𑝂₷
            stamina += diff;
            return stamina == maxStamina;
        }

        /// <summary>
        /// �X�^�~�i�����炷�B���̏����ŃX�^�~�i���ŏ��Ɂu�Ȃ����v���̂݁Atrue��Ԃ��B
        /// </summary>
        public bool DecreaseStamina(int diff)
        {
            if (diff <= 0) throw new System.Exception("diff�͐��ł���K�v������܂�");

            // ���ɍŏ��Ȃ珈���X�g�b�v
            if (stamina == 0) return false;

            // �X�^�~�i�����炷
            stamina -= diff;
            return stamina == 0;
        }

        /// <summary>
        /// �X�^�~�i���ő�Ȃ�true�A�����łȂ��Ȃ�false
        /// </summary>
        public bool IsStaminaFull()
        {
            return stamina == maxStamina;
        }

        /// <summary>
        /// �X�^�~�i���ŏ��Ȃ�true�A�����łȂ��Ȃ�false
        /// </summary>
        public bool IsStaminaZero()
        {
            return stamina == 0;
        }
    }
}