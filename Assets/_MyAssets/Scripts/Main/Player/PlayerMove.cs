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
        [SerializeField] private Transform staminaGaugeRoot1;
        [SerializeField] private Transform stackRoot1;
        [SerializeField] private Animator animator1;
        [Space(25)]
        [Header("�`�� 2")]
        [SerializeField] private GameObject figure2;
        [SerializeField] private Transform figureTf2;
        [SerializeField] private Rigidbody figureRb2;
        [SerializeField] private Transform headTf2;
        [SerializeField] private Transform aboveHeadTf2;
        [SerializeField] private Transform footTf2;
        [SerializeField] private Transform staminaGaugeRoot2;
        [SerializeField] private Transform stackRoot2;
        [SerializeField] private Animator animator2;
        [Space(25)]
        [SerializeField] private Cinemachine.CinemachineFreeLook freeLookCamera;
        [SerializeField] private Transform freeLookCameraTf;
        [Space(25)]
        [SerializeField] private Transform stackStartPoint;
        [Space(25)]
        [SerializeField] private Transform staminaGaugeParentTf;
        [SerializeField] private Transform staminaGaugeTf;
        [SerializeField] private SpriteRenderer staminaGaugeSr;
        [Space(25)]
        [SerializeField] private Transform initPlaceTf;
        [SerializeField, Header("SoundPlayer")] private General.SoundPlayer soundPlayer;
        [SerializeField, Header("�X�e�[�W�̃{�[�_�[")] private Border.Border stageBorder;
        [SerializeField, Header("�_�Ђ̃{�[�_�[")] private Border.Border shrineBorder;
        [SerializeField, Header("�X�т̃{�[�_�[")] private Border.Border forestBorder;
        [SerializeField, Header("�����̃{�[�_�[")] private Border.Border villageBorder;

        private PlayerMoveBhv impl;

        private void OnEnable()
        {
            if (!soundPlayer) throw new System.Exception($"{nameof(soundPlayer)}���ݒ肳��Ă��܂���");
            if (!stageBorder) throw new System.Exception($"{nameof(stageBorder)}���ݒ肳��Ă��܂���");
            if (!shrineBorder) throw new System.Exception($"{nameof(shrineBorder)}���ݒ肳��Ă��܂���");
            if (!forestBorder) throw new System.Exception($"{nameof(forestBorder)}���ݒ肳��Ă��܂���");
            if (!villageBorder) throw new System.Exception($"{nameof(villageBorder)}���ݒ肳��Ă��܂���");

            impl = new(
                new(figure1, figureTf1, figureRb1, headTf1, aboveHeadTf1, footTf1,
                staminaGaugeRoot1, stackRoot1, animator1),
                new(figure2, figureTf2, figureRb2, headTf2, aboveHeadTf2, footTf2,
                staminaGaugeRoot1, stackRoot2, animator2),
                freeLookCamera, freeLookCameraTf, stackStartPoint,
                staminaGaugeParentTf, staminaGaugeTf, staminaGaugeSr,
                initPlaceTf, soundPlayer, new(stageBorder, shrineBorder, forestBorder, villageBorder)
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

        internal float CalcSqrMagnitude(Vector3 pos)
        {
            return impl.CalcSqrMagnitude(pos);
        }

        internal bool IsFoxFigure()
        {
            return impl.IsFoxFigure();
        }

        internal void SetLookAroundable(bool isAble)
        {
            impl.IsLookAroundable = isAble;
        }
    }

    internal sealed class PlayerMoveBhv : System.IDisposable
    {
        private enum FigureIndex
        {
            Fox = 1,
            Human = 2
        }

        private enum Location
        {
            Shrine,
            Forest,
            Village
        }

        private readonly PlayerFigure figure1;
        private readonly PlayerFigure figure2;
        private readonly PlayerStamina playerStamina;
        private Cinemachine.CinemachineFreeLook freeLookCamera;
        private Transform freeLookCameraTf;
        private Transform stackStartPoint;
        private General.SoundPlayer soundPlayer;
        private Borders borders;

        private FigureIndex figureIndex = FigureIndex.Fox;  // �`�Ԃ̃C���f�b�N�X�ԍ�
        private float moveSpeed = SO_Player.Entity.NormalSpeed;  // �ړ��X�s�[�h
        private Vector3 initPosition = Vector3.zero;  // �������W
        private Vector3 prePosition = Vector3.zero;  // �O�t���[���ł̍��W
        private float staminaT = 0;  // �X�^�~�i�����̎��ԃJ�E���^
        private bool isStaminaIncreasable = true;  // �X�^�~�i���񕜉\���ǂ���
        private bool isCharging = false;  // �ːi�����ǂ���
        private Location location = Location.Shrine;  // ���t���[���ł̃��P�[�V����
        private Location preLocation = Location.Shrine;  // �O�t���[���ł̃��P�[�V����

        private bool isCheckedOnFirstUpdate = false;  // �ŏ���Update�ōs���A�`�F�b�N�̃t���O

        private static readonly string IS_MOVING_PARAM = "IsMoving";

        internal bool IsLookAroundable { get; set; } = true;  // �J������2�����͂��󂯕t���邩�ǂ���

        public PlayerMoveBhv(PlayerFigure figure1, PlayerFigure figure2,
            Cinemachine.CinemachineFreeLook freeLookCamera, Transform freeLookCameraTf, Transform stackStartPoint,
            Transform staminaGaugeParentTf, Transform staminaGaugeTf, SpriteRenderer staminaGaugeSr,
            Transform initPlaceTf, General.SoundPlayer soundPlayer, Borders borders)
        {
            this.figure1 = figure1;
            this.figure2 = figure2;
            this.freeLookCamera = freeLookCamera;
            this.freeLookCameraTf = freeLookCameraTf;
            this.stackStartPoint = stackStartPoint;
            initPosition = initPlaceTf.position;
            prePosition = initPosition;
            this.soundPlayer = soundPlayer;
            this.borders = borders;

            figure1.FigureTf.position = initPosition;
            figure2.FigureTf.position = initPosition;

            playerStamina = new(staminaGaugeParentTf, staminaGaugeTf, staminaGaugeSr);

            ChangeFigure(FigureIndex.Human, FigureIndex.Fox);
            SetStaminaGaugeParentBhv();
        }

        public void Dispose()
        {
            figure1.Dispose();
            figure2.Dispose();
            playerStamina.Dispose();
            borders.Dispose();
            freeLookCamera = null;
            freeLookCameraTf = null;
            stackStartPoint = null;
            soundPlayer = null;
        }

        public void Update()
        {
            if (figure1.IsNullExist()) return;
            if (figure2.IsNullExist()) return;
            if (playerStamina.IsNullExist()) return;
            if (borders.IsNullExist()) return;
            if (!freeLookCamera) return;
            if (!freeLookCameraTf) return;
            if (!stackStartPoint) return;
            if (!soundPlayer) return;

            if (!isCheckedOnFirstUpdate)
            {
                isCheckedOnFirstUpdate = true;

                if (!borders.StageBorder.IsIn(initPosition))
                    throw new System.Exception($"�v���C���[�̏������W {initPosition} ���A�X�e�[�W�͈͓̔��ɂ���܂���");

                location = GetLocation(GetFigure(figureIndex).FigureTf.position);
                PlayBGM(location);
                preLocation = location;
            }

            // �ϐg
            TransformBhv();

            // ����������
            ResetInertiaBhv();

            // ���_�ړ��ɑ΂�����͂̉ۂ�؂�ւ�
            UpdateLookAroundableBhv();

            // ��]�E�ړ�
            TurnBhv();
            MoveBhv();

            // ����s��
            ChargeBhv();
            GetItemBhv();  // ���g�Ȃ�
            TradeBhv();  // ���g�Ȃ�

            // �X�^�~�i�Q�[�W�̐e���X�V
            SetStaminaGaugeParentBhv();

            // �ςݏグ��ꏊ���X�V
            UpdateStackStartPointBhv();

            // ���P�[�V�����̍X�V + ����ɔ�������
            UpdateLocationBhv();
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

        internal float CalcSqrMagnitude(Vector3 pos)
        {
            return (GetFigure(figureIndex).FigureTf.position - pos).sqrMagnitude;
        }

        internal bool IsFoxFigure()
        {
            return figureIndex == FigureIndex.Fox;
        }

        private Location GetLocation(Vector3 pos)
        {
            if (borders.ShrineBorder.IsIn(pos)) return Location.Shrine;
            else if (borders.ForestBorder.IsIn(pos)) return Location.Forest;
            else if (borders.VillageBorder.IsIn(pos)) return Location.Village;
            else throw new System.Exception("�v���C���[���s���ȏꏊ�ɂ��܂�");
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

        // �^����ꂽ���P�[�V�����ɑΉ�����BGM���Đ�����
        private void PlayBGM(Location location)
        {
            General.SoundType soundType = location switch
            {
                Location.Shrine => General.SoundType.Main_ShrineBGM,
                Location.Forest => General.SoundType.Main_ForestBGM,
                Location.Village => General.SoundType.Main_VillageBGM,
                _ => throw new System.Exception("�v���C���[���s���ȏꏊ�ɂ��܂�")
            };

            soundPlayer.Play(soundType);
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

        // �ςݏグ��ꏊ���X�V����
        private void UpdateStackStartPointBhv()
        {
            stackStartPoint.position = GetFigure(figureIndex).StackRoot.position;
        }

        // ����������(Rigidbody�̑��x�́Ax��z��0�ɂ���)
        private void ResetInertiaBhv()
        {
            Vector3 vel = GetFigure(figureIndex).FigureRb.velocity;
            vel.x = 0; vel.z = 0;
            GetFigure(figureIndex).FigureRb.velocity = vel;
        }

        // ���_�ړ��ɑ΂�����͂̉ۂ�؂�ւ���
        private void UpdateLookAroundableBhv()
        {
            freeLookCamera.m_XAxis.m_InputAxisName = IsLookAroundable ? "Mouse X" : "";
            freeLookCamera.m_YAxis.m_InputAxisName = IsLookAroundable ? "Mouse Y" : "";
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
                fgr.Animator.Set(IS_MOVING_PARAM, false);
                return;
            }
            // �����Ă���
            else
            {
                Vector3 moveValueLocalNormed = new Vector3(moveValueInputted.x, 0, moveValueInputted.y).normalized;
                Vector3 moveValueLocal = moveValueLocalNormed * (moveSpeed * Time.deltaTime);
                Vector3 moveValue = fgr.FigureTf.right * moveValueLocal.x + fgr.FigureTf.forward * moveValueLocal.z;
                fgr.FigureTf.position += moveValue;
                if (!borders.StageBorder.IsIn(fgr.FigureTf.position)) fgr.FigureTf.position = prePosition;
                prePosition = fgr.FigureTf.position;

                // ���̌`�Ԃ����݂̌`�ԂɃR���X�g���C��
                Vector3 toPos = GetFigure(figureIndex).FigureTf.position;
                foreach (FigureIndex e in GetOtherFigureIndices(figureIndex))
                {
                    GetFigure(e).FigureTf.position = toPos;
                }

                fgr.Animator.Set(IS_MOVING_PARAM, true);
            }
        }

        // �ːi
        private void ChargeBhv()
        {
            if (figureIndex != FigureIndex.Fox) return;

            // �ːi�L�[�̓��͏�Ԃɂ���āA�����\�Ȃ�΁A�ːi��Ԃ��X�V����B
            // ���ɁA�ːi�L�[�������ꂽ�Ƃ��A�X�^�~�i�̉񕜂��\�ɂ���B([A]�ɑΉ����鏈��)
            if (InputGetter.Instance.Main_DashValue0.Get<bool>()) { SetCharge(true); }
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

        // �X�^�~�i�Q�[�W�̐e���X�V����
        private void SetStaminaGaugeParentBhv()
        {
            PlayerFigure fgr = GetFigure(figureIndex);
            playerStamina.SetGaugeParent(fgr.FigureTf, fgr.StaminaGaugeRoot.localPosition);
        }

        // ���P�[�V�����̍X�V + ����ɔ������� ���s��
        private void UpdateLocationBhv()
        {
            location = GetLocation(GetFigure(figureIndex).FigureTf.position);

            // ���P�[�V�������ς������ABGM��ύX
            if (location != preLocation)
                PlayBGM(location);

            preLocation = location;
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
        public Transform StaminaGaugeRoot { get; private set; }
        public Transform StackRoot { get; private set; }
        public Animator Animator { get; private set; }

        public PlayerFigure(GameObject figure, Transform figureTf, Rigidbody figureRb,
            Transform headTf, Transform aboveHeadTf, Transform footTf,
            Transform staminaGaugeRoot, Transform stackRoot, Animator animator)
        {
            this.Figure = figure;
            this.FigureTf = figureTf;
            this.FigureRb = figureRb;
            this.HeadTf = headTf;
            this.AboveHeadTf = aboveHeadTf;
            this.FootTf = footTf;
            this.StaminaGaugeRoot = staminaGaugeRoot;
            this.StackRoot = stackRoot;
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
            StaminaGaugeRoot = null;
            StackRoot = null;
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
            if (!StaminaGaugeRoot) return true;
            if (!StackRoot) return true;
            if (!Animator) return true;

            return false;
        }
    }

    internal sealed class PlayerStamina : System.IDisposable
    {
        private int _stamina = 0;
        private int stamina
        {
            get { return _stamina; }
            set { _stamina = Mathf.Clamp(value, 0, SO_Player.Entity.MaxStamina); }
        }
        private Transform gaugeParentTf;
        private Transform gaugeTf;
        private SpriteRenderer gaugeSr;

        internal PlayerStamina(Transform gaugeParentTf, Transform gaugeTf, SpriteRenderer gaugeSr)
        {
            this.gaugeParentTf = gaugeParentTf;
            this.gaugeTf = gaugeTf;
            this.gaugeSr = gaugeSr;

            stamina = SO_Player.Entity.MaxStamina;
        }

        public void Dispose()
        {
            gaugeParentTf = null;
            gaugeTf = null;
            gaugeSr = null;
        }

        internal bool IsNullExist()
        {
            if (!gaugeParentTf) return true;
            if (!gaugeTf) return true;
            if (!gaugeSr) return true;

            return false;
        }

        // �X�^�~�i�𑝂₷�B���̏����ŃX�^�~�i���ő�Ɂu�Ȃ����v���̂݁Atrue��Ԃ��B
        internal bool IncreaseStamina(int diff)
        {
            if (!gaugeParentTf) return false;
            if (!gaugeTf) return false;
            if (!gaugeSr) return false;

            if (diff <= 0) throw new System.Exception("diff�͐��ł���K�v������܂�");
            if (stamina == SO_Player.Entity.MaxStamina) return false;

            stamina += diff;

            UpdateGauge();

            return IsStaminaFull();
        }

        // �X�^�~�i�����炷�B���̏����ŃX�^�~�i���ŏ��Ɂu�Ȃ����v���̂݁Atrue��Ԃ��B
        internal bool DecreaseStamina(int diff)
        {
            if (!gaugeParentTf) return false;
            if (!gaugeTf) return false;
            if (!gaugeSr) return false;

            if (diff <= 0) throw new System.Exception("diff�͐��ł���K�v������܂�");
            if (stamina == 0) return false;

            stamina -= diff;

            UpdateGauge();

            return IsStaminaZero();
        }

        // �Q�[�W�̃X�P�[���ƐF���X�V����
        private void UpdateGauge()
        {
            if (!gaugeTf) return;
            if (!gaugeSr) return;

            if (IsStaminaFull()) { gaugeSr.enabled = false; return; }

            gaugeSr.enabled = true;

            float p = 1.0f * stamina / SO_Player.Entity.MaxStamina;
            gaugeTf.localScale = new(Ex.Remap(1, 0, 1, 0, p), 1, 1);
            gaugeTf.localPosition = Vector3.zero;
            gaugeTf.localPosition = new(Ex.Remap(1, 0, 0, -1.2f, p), 0, 0);
            gaugeSr.color = new(1, Ex.Remap(1, 0, 1, 0, p), 0, 1);
        }

        internal void SetGaugeParent(Transform parentTf, Vector3 localPosition)
        {
            gaugeParentTf.parent = parentTf;
            gaugeParentTf.localPosition = localPosition;
        }

        internal void ResetGaugeParent()
        {
            gaugeParentTf.parent = null;
            gaugeParentTf.localPosition = Vector3.zero;
        }

        // �X�^�~�i���ő�Ȃ�true�A�����łȂ��Ȃ�false�B
        internal bool IsStaminaFull() { return stamina == SO_Player.Entity.MaxStamina; }

        // �X�^�~�i���ŏ��Ȃ�true�A�����łȂ��Ȃ�false�B
        internal bool IsStaminaZero() { return stamina == 0; }
    }

    internal sealed class Borders : System.IDisposable
    {
        private Border.Border _stageBorder;
        internal Border.Border StageBorder => _stageBorder;

        private Border.Border _shrineBorder;
        internal Border.Border ShrineBorder => _shrineBorder;

        private Border.Border _forestBorder;
        internal Border.Border ForestBorder => _forestBorder;

        private Border.Border _villageBorder;
        internal Border.Border VillageBorder => _villageBorder;

        internal Borders(Border.Border stageBorder,
            Border.Border shrineBorder, Border.Border forestBorder, Border.Border villageBorder)
        {
            this._stageBorder = stageBorder;
            this._shrineBorder = shrineBorder;
            this._forestBorder = forestBorder;
            this._villageBorder = villageBorder;
        }

        public void Dispose()
        {
            _stageBorder = null;
            _shrineBorder = null;
            _forestBorder = null;
            _villageBorder = null;
        }

        internal bool IsNullExist()
        {
            if (!_stageBorder) return true;
            if (!_shrineBorder) return true;
            if (!_forestBorder) return true;
            if (!_villageBorder) return true;

            return false;
        }
    }

    internal static class Ex
    {
        // ���X[a, b]���A���Y[c, d]�ɐ��`�}�b�s���O�����Ƃ���B���X���̒lx�ɂ��āA�ϊ���̋��Y�ɂ�����ly��Ԃ��B
        internal static float Remap(float a, float b, float c, float d, float x)
        {
            float y = (x - a) * (d - c) / (b - a) + c;
            return y;
        }

        internal static bool Has(this Animator animator, string param)
        {
            foreach (var e in animator.parameters)
            {
                if (e.name == param)
                    return true;
            }

            return false;
        }

        internal static void Set<T>(this Animator animator, string param, T var)
        {
            if (typeof(T) == typeof(bool))
            {
                if (!animator.Has(param))
                    throw new System.Exception($"{nameof(animator)}���A�p�����[�^'{param}'�������Ă��܂���");
                else
                    animator.SetBool(param, (bool)(object)var);
            }
            else if (typeof(T) == typeof(int))
            {
                if (!animator.Has(param))
                    throw new System.Exception($"{nameof(animator)}���A�p�����[�^'{param}'�������Ă��܂���");
                else
                    animator.SetInteger(param, (int)(object)var);
            }
            else if (typeof(T) == typeof(float))
            {
                if (!animator.Has(param))
                    throw new System.Exception($"{nameof(animator)}���A�p�����[�^'{param}'�������Ă��܂���");
                else
                    animator.SetFloat(param, (float)(object)var);
            }
            else
            {
                throw new System.Exception($"{nameof(T)}�͖����Ȍ^�ł�");
            }
        }
    }
}