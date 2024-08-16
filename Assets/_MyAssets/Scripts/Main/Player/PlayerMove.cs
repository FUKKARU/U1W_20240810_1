using UnityEngine;
using System.Collections.Generic;
using SO;
using IA;

namespace Main.Player
{
    public sealed class PlayerMove : MonoBehaviour
    {
        [Header("�`�� 1 (����)")]
        [SerializeField] private GameObject figure1;
        [SerializeField] private Transform figureTf1;
        [SerializeField] private Rigidbody figureRb1;
        [SerializeField] private Transform headTf1;
        [SerializeField] private Transform aboveHeadTf1;
        [SerializeField] private Transform footTf1;
        [SerializeField] private Animator animator1;
        [Space(25)]
        [Header("�`�� 2")]
        [SerializeField] private GameObject figure2;
        [SerializeField] private Transform figureTf2;
        [SerializeField] private Rigidbody figureRb2;
        [SerializeField] private Transform headTf2;
        [SerializeField] private Transform aboveHeadTf2;
        [SerializeField] private Transform footTf2;
        [SerializeField] private Animator animator2;
        [Space(25)]
        [SerializeField] private Cinemachine.CinemachineFreeLook freeLookCamera;
        [SerializeField] private Transform freeLookCameraTf;
        [Space(25)]
        [SerializeField] private Transform initPlaceTf;
        [SerializeField, Header("�X�e�[�W�̃{�[�_�[")] private Border.Border stageBorder;

        private PlayerMoveBhv impl;

        private void OnEnable()
        {
            if (!stageBorder) throw new System.Exception($"{nameof(stageBorder)}���ݒ肳��Ă��܂���");

            impl = new(
                new(figure1, figureTf1, figureRb1, headTf1, aboveHeadTf1, footTf1, animator1),
                new(figure2, figureTf2, figureRb2, headTf2, aboveHeadTf2, footTf2, animator2),
                freeLookCamera, freeLookCameraTf,
                initPlaceTf, stageBorder
                );
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
        private enum FigureIndex
        {
            Fox = 1,
            Human = 2
        }

        private readonly PlayerFigure figure1;
        private readonly PlayerFigure figure2;
        private Cinemachine.CinemachineFreeLook freeLookCamera;
        private Transform freeLookCameraTf;
        private Border.Border stageBorder;

        private readonly PlayerStamina playerStamina;

        private FigureIndex figureIndex = FigureIndex.Fox;  // �`�Ԃ̃C���f�b�N�X�ԍ�
        private float moveSpeed = SO_Player.Entity.NormalSpeed;  // �ړ��X�s�[�h
        private Vector3 initPosition = Vector3.zero;  // �������W
        private Vector3 prePosition = Vector3.zero;  // �O�t���[���ł̍��W
        private float staminaT = 0;  // �X�^�~�i�����̎��ԃJ�E���^
        private bool isStaminaIncreasable = true;  // �X�^�~�i���񕜉\���ǂ���
        private bool isCharging = false;  // �ːi�����ǂ���

        private bool isInitPositionChecked = false;  // initPosition���L���Ȓl���ǂ����A�m�F�����t���O

        public PlayerMoveBhv(PlayerFigure figure1, PlayerFigure figure2,
            Cinemachine.CinemachineFreeLook freeLookCamera, Transform freeLookCameraTf,
            Transform initPlaceTf, Border.Border stageBorder)
        {
            this.figure1 = figure1;
            this.figure2 = figure2;
            this.freeLookCamera = freeLookCamera;
            this.freeLookCameraTf = freeLookCameraTf;
            initPosition = initPlaceTf.position;
            prePosition = initPosition;
            this.stageBorder = stageBorder;

            figure1.FigureTf.position = initPosition;
            figure2.FigureTf.position = initPosition;

            playerStamina = new();

            ChangeFigure(FigureIndex.Human, FigureIndex.Fox);
        }

        public void Dispose()
        {
            figure1.Dispose();
            figure2.Dispose();
            freeLookCamera = null;
            freeLookCameraTf = null;
            stageBorder = null;
        }

        public void Update()
        {
            if (figure1.IsNullExist()) return;
            if (figure2.IsNullExist()) return;
            if (!freeLookCamera) return;
            if (!freeLookCameraTf) return;
            if (!stageBorder) return;

            if (!isInitPositionChecked)
            {
                isInitPositionChecked = true;

                if (!stageBorder.IsIn(initPosition))
                    throw new System.Exception($"�v���C���[�̏������W {initPosition} ���A�X�e�[�W�͈͓̔��ɂ���܂���");
            }

            // �ϐg
            TransformBhv();

            // ����������
            ResetInertiaBhv();

            // ��]�E�ړ�
            TurnBhv();
            MoveBhv();

            // ����s��
            ChargeBhv();
            GetItemBhv();
            TradeBhv();
        }

        // �����ȊO�̃C���f�b�N�X�����ׂĎ擾
        private List<FigureIndex> GetOtherFigureIndices(FigureIndex i)
        {
            return i switch
            {
                FigureIndex.Fox => new() { FigureIndex.Human },
                FigureIndex.Human => new() { FigureIndex.Fox },
                _ => throw new System.Exception("�����ȃC���f�b�N�X�ԍ��ł�")
            };
        }

        // �����̃C���f�b�N�X�ɑΉ�����PlayerFigure���擾
        private PlayerFigure GetFigure(FigureIndex i)
        {
            return i switch
            {
                FigureIndex.Fox => figure1,
                FigureIndex.Human => figure2,
                _ => throw new System.Exception("�����ȃC���f�b�N�X�ԍ��ł�")
            };
        }

        // oldI����newI�Ɍ`�ԕω�
        private void ChangeFigure(FigureIndex oldI, FigureIndex newI)
        {
            figureIndex = newI;
            GetFigure(oldI).Figure.SetActive(false);
            GetFigure(newI).Figure.SetActive(true);
            freeLookCamera.Follow = GetFigure(newI).FigureTf;
            freeLookCamera.LookAt = GetFigure(newI).HeadTf;
            freeLookCamera.GetRig(0).m_LookAt = GetFigure(newI).FootTf;
            freeLookCamera.GetRig(1).m_LookAt = GetFigure(newI).HeadTf;
            freeLookCamera.GetRig(2).m_LookAt = GetFigure(newI).AboveHeadTf;
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

        // �ϐg����
        private void TransformBhv()
        {
            if (!InputGetter.Instance.Main_TransformClick.Get<bool>()) return;

            System.Action action = figureIndex switch
            {
                FigureIndex.Fox => () => ChangeFigure(FigureIndex.Fox, FigureIndex.Human),
                FigureIndex.Human => () => ChangeFigure(FigureIndex.Human, FigureIndex.Fox),
                _ => throw new System.Exception("�����ȃC���f�b�N�X�ԍ��ł�")
            };

            action();
        }

        // ����������(Rigidbody�̑��x�́Ax��z��0�ɂ���)
        private void ResetInertiaBhv()
        {
            Vector3 vel = GetFigure(figureIndex).FigureRb.velocity;
            vel.x = 0; vel.z = 0;
            GetFigure(figureIndex).FigureRb.velocity = vel;
        }

        // �v���C���[���J�����̕����ɉ�]������
        private void TurnBhv()
        {
            PlayerFigure fgr = GetFigure(figureIndex);
            Vector3 lookAtLocal = fgr.FigureTf.position - freeLookCameraTf.position;
            Vector3 lookAtLocalXZ = new(lookAtLocal.x, 0, lookAtLocal.z);
            Vector3 lookAtXZ = fgr.FigureTf.position + lookAtLocalXZ;
            fgr.FigureTf.LookAt(lookAtXZ);

            // ���̌`�Ԃ����݂̌`�ԂɃR���X�g���C��
            Quaternion toQua = GetFigure(figureIndex).FigureTf.rotation;
            foreach (FigureIndex e in GetOtherFigureIndices(figureIndex))
            {
                GetFigure(e).FigureTf.rotation = toQua;
            }
        }

        // �ړ�
        private void MoveBhv()
        {
            PlayerFigure fgr = GetFigure(figureIndex);
            if (fgr.FigureTf.position.y < -100) fgr.FigureTf.position = initPosition;  // �������߂�����A�������W�ɖ߂�
            Vector2 moveValueInputted = InputGetter.Instance.Main_MoveValue2.Get<Vector2>();

            // �����Ă��Ȃ�
            if (moveValueInputted == Vector2.zero)
            {
                fgr.Animator.SetBool("IsMoving", false);
                return;
            }
            // �����Ă���
            else
            {
                Vector3 moveValueLocalNormed = new Vector3(moveValueInputted.x, 0, moveValueInputted.y).normalized;
                Vector3 moveValueLocal = moveValueLocalNormed * (moveSpeed * Time.deltaTime);
                Vector3 moveValue = fgr.FigureTf.right * moveValueLocal.x + fgr.FigureTf.forward * moveValueLocal.z;
                fgr.FigureTf.position += moveValue;
                if (!stageBorder.IsIn(fgr.FigureTf.position)) fgr.FigureTf.position = prePosition;
                prePosition = fgr.FigureTf.position;

                // ���̌`�Ԃ����݂̌`�ԂɃR���X�g���C��
                Vector3 toPos = GetFigure(figureIndex).FigureTf.position;
                foreach (FigureIndex e in GetOtherFigureIndices(figureIndex))
                {
                    GetFigure(e).FigureTf.position = toPos;
                }

                fgr.Animator.SetBool("IsMoving", true);
            }
        }

        // �ːi
        private void ChargeBhv()
        {
            if (figureIndex != FigureIndex.Fox) return;

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

        // �A�C�e���擾
        private void GetItemBhv()
        {
            if (figureIndex != FigureIndex.Fox) return;
        }

        // ����
        private void TradeBhv()
        {
            if (figureIndex != FigureIndex.Human) return;
        }
    }

    internal sealed class PlayerFigure : System.IDisposable
    {
        public GameObject Figure { get; private set; }
        public Transform FigureTf { get; private set; }
        public Rigidbody FigureRb { get; private set; }
        public Transform HeadTf { get; private set; }
        public Transform AboveHeadTf { get; private set; }
        public Transform FootTf { get; private set; }
        public Animator Animator { get; private set; }

        public PlayerFigure(GameObject figure, Transform figureTf, Rigidbody figureRb,
            Transform headTf, Transform aboveHeadTf, Transform footTf, Animator animator)
        {
            this.Figure = figure;
            this.FigureTf = figureTf;
            this.FigureRb = figureRb;
            this.HeadTf = headTf;
            this.AboveHeadTf = aboveHeadTf;
            this.FootTf = footTf;
            this.Animator = animator;
        }

        public void Dispose()
        {
            FigureTf = null;
            FigureRb = null;
            Figure = null;
            HeadTf = null;
            AboveHeadTf = null;
            FootTf = null;
            Animator = null;
        }

        public bool IsNullExist()
        {
            if (!Figure) return true;
            if (!FigureTf) return true;
            if (!FigureRb) return true;
            if (!HeadTf) return true;
            if (!AboveHeadTf) return true;
            if (!FootTf) return true;
            if (!Animator) return true;

            return false;
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