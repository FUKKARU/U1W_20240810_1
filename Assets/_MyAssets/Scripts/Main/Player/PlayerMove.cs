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
        [Space(25)]
        [Header("�`�� 2")]
        [SerializeField] private GameObject figure2;
        [SerializeField] private Transform figureTf2;
        [SerializeField] private Rigidbody figureRb2;
        [SerializeField] private Transform headTf2;
        [SerializeField] private Transform aboveHeadTf2;
        [SerializeField] private Transform footTf2;
        [Space(25)]
        [SerializeField] private Cinemachine.CinemachineFreeLook freeLookCamera;
        [SerializeField] private Transform freeLookCameraTf;

        private PlayerMoveBhv impl;

        private void OnEnable()
        {
            impl = new(
                new(figure1, figureTf1, figureRb1, headTf1, aboveHeadTf1, footTf1),
                new(figure2, figureTf2, figureRb2, headTf2, aboveHeadTf2, footTf2),
                freeLookCamera, freeLookCameraTf
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
        private readonly PlayerFigure figure1;
        private readonly PlayerFigure figure2;
        private Cinemachine.CinemachineFreeLook freeLookCamera;
        private Transform freeLookCameraTf;

        private readonly PlayerStamina playerStamina;

        private int figureIndex = 1;  // �`�Ԃ̃C���f�b�N�X�ԍ�
        private float moveSpeed = SO_Player.Entity.NormalSpeed;  // �ړ��X�s�[�h
        private float staminaT = 0;  // �X�^�~�i�����̎��ԃJ�E���^
        private bool isStaminaIncreasable = true;  // �X�^�~�i���񕜉\���ǂ���
        private bool isCharging = false;  // �ːi�����ǂ���

        public PlayerMoveBhv(PlayerFigure figure1, PlayerFigure figure2,
            Cinemachine.CinemachineFreeLook freeLookCamera, Transform freeLookCameraTf)
        {
            this.figure1 = figure1;
            this.figure2 = figure2;
            this.freeLookCamera = freeLookCamera;
            this.freeLookCameraTf = freeLookCameraTf;

            playerStamina = new();

            ChangeFigure(2, 1);
        }

        public void Dispose()
        {
            figure1.Dispose();
            figure2.Dispose();
            freeLookCamera = null;
            freeLookCameraTf = null;
        }

        public void Update()
        {
            if (figure1.IsNullExist()) return;
            if (figure2.IsNullExist()) return;
            if (!freeLookCamera) return;
            if (!freeLookCameraTf) return;

            // �ϐg����
            if (InputGetter.Instance.Main_TransformClick.Get<bool>())
            {
                System.Action action = figureIndex switch
                {
                    1 => () => ChangeFigure(1, 2),
                    2 => () => ChangeFigure(2, 1),
                    _ => throw new System.Exception("�����ȃC���f�b�N�X�ԍ��ł�")
                };

                action();
            }

            // ����������
            GetFigure(figureIndex).FigureRb.velocity = Vector3.zero;

            // ��]�E�ړ�
            Turn(figureIndex);
            Move(figureIndex);

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

        private List<int> GetOtherFigureIndices(int i)
        {
            return i switch
            {
                1 => new() { 2 },
                2 => new() { 1 },
                _ => throw new System.Exception("�����ȃC���f�b�N�X�ԍ��ł�")
            };
        }

        private PlayerFigure GetFigure(int i)
        {
            return i switch
            {
                1 => figure1,
                2 => figure2,
                _ => throw new System.Exception("�����ȃC���f�b�N�X�ԍ��ł�")
            };
        }

        private void ChangeFigure(int oldI, int newI)
        {
            figureIndex = newI;
            GetFigure(oldI).Figure.SetActive(false);
            GetFigure(newI).Figure.SetActive(true);
            freeLookCamera.Follow = GetFigure(newI).FigureTf;
            freeLookCamera.LookAt = GetFigure(newI).HeadTf;
            freeLookCamera.GetRig(0).m_LookAt = GetFigure(newI).AboveHeadTf;
            freeLookCamera.GetRig(1).m_LookAt = GetFigure(newI).HeadTf;
            freeLookCamera.GetRig(2).m_LookAt = GetFigure(newI).FootTf;
        }

        // �v���C���[���J�����̕����ɉ�]������
        private void Turn(int i)
        {
            PlayerFigure fgr = GetFigure(i);
            Vector3 lookAtLocal = fgr.FigureTf.position - freeLookCameraTf.position;
            Vector3 lookAtLocalXZ = new(lookAtLocal.x, 0, lookAtLocal.z);
            Vector3 lookAtXZ = fgr.FigureTf.position + lookAtLocalXZ;
            fgr.FigureTf.LookAt(lookAtXZ);
            TurnConstrain(GetOtherFigureIndices(i), i);
        }

        private void TurnConstrain(List<int> fromIList, int toI)
        {
            Quaternion toQua = GetFigure(toI).FigureTf.rotation;

            foreach (int e in fromIList)
            {
                GetFigure(e).FigureTf.rotation = toQua;
            }
        }

        // �v���C���[��O�㍶�E�ɓ�����
        private void Move(int i)
        {
            PlayerFigure fgr = GetFigure(i);
            Vector2 moveValueInputted = InputGetter.Instance.Main_MoveValue2.Get<Vector2>();
            Vector3 moveValueLocalNormed = new Vector3(moveValueInputted.x, 0, moveValueInputted.y).normalized;
            Vector3 moveValueLocal = moveValueLocalNormed * (moveSpeed * Time.deltaTime);
            Vector3 moveValue = fgr.FigureTf.right * moveValueLocal.x + fgr.FigureTf.forward * moveValueLocal.z;
            fgr.FigureTf.localPosition += moveValue;
            MoveConstrain(GetOtherFigureIndices(i), i);
        }

        private void MoveConstrain(List<int> fromIList, int toI)
        {
            Vector3 toPos = GetFigure(toI).FigureTf.position;

            foreach (int e in fromIList)
            {
                GetFigure(e).FigureTf.position = toPos;
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

    internal sealed class PlayerFigure : System.IDisposable
    {
        public GameObject Figure { get; private set; }
        public Transform FigureTf { get; private set; }
        public Rigidbody FigureRb { get; private set; }
        public Transform HeadTf { get; private set; }
        public Transform AboveHeadTf { get; private set; }
        public Transform FootTf { get; private set; }

        public PlayerFigure(GameObject figure, Transform figureTf, Rigidbody figureRb,
            Transform headTf, Transform aboveHeadTf, Transform footTf)
        {
            this.Figure = figure;
            this.FigureTf = figureTf;
            this.FigureRb = figureRb;
            this.HeadTf = headTf;
            this.AboveHeadTf = aboveHeadTf;
            this.FootTf = footTf;
        }

        public void Dispose()
        {
            FigureTf = null;
            FigureRb = null;
            Figure = null;
            HeadTf = null;
            AboveHeadTf = null;
            FootTf = null;
        }

        public bool IsNullExist()
        {
            if (!Figure) return true;
            if (!FigureTf) return true;
            if (!FigureRb) return true;
            if (!HeadTf) return true;
            if (!AboveHeadTf) return true;
            if (!FootTf) return true;

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