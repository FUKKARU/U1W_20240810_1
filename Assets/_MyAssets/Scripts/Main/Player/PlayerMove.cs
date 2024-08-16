using UnityEngine;
using System.Collections.Generic;
using SO;
using IA;

namespace Main.Player
{
    public sealed class PlayerMove : MonoBehaviour
    {
        [Header("形態 1 (初期)")]
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
        [Header("形態 2")]
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
        [SerializeField, Header("ステージのボーダー")] private Border.Border stageBorder;

        private PlayerMoveBhv impl;

        private void OnEnable()
        {
            if (!stageBorder) throw new System.Exception($"{nameof(stageBorder)}が設定されていません");

            impl = new(
                new(figure1, figureTf1, figureRb1, headTf1, aboveHeadTf1, footTf1,
                staminaGaugeRoot1, stackRoot1, animator1),
                new(figure2, figureTf2, figureRb2, headTf2, aboveHeadTf2, footTf2,
                staminaGaugeRoot1, stackRoot2, animator2),
                freeLookCamera, freeLookCameraTf, stackStartPoint,
                staminaGaugeParentTf, staminaGaugeTf, staminaGaugeSr,
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

        internal float CalcSqrMagnitude(Vector3 pos)
        {
            return impl.CalcSqrMagnitude(pos);
        }

        internal bool IsFoxFigure()
        {
            return impl.IsFoxFigure();
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
        private readonly PlayerStamina playerStamina;
        private Cinemachine.CinemachineFreeLook freeLookCamera;
        private Transform freeLookCameraTf;
        private Transform stackStartPoint;
        private Border.Border stageBorder;

        private FigureIndex figureIndex = FigureIndex.Fox;  // 形態のインデックス番号
        private float moveSpeed = SO_Player.Entity.NormalSpeed;  // 移動スピード
        private Vector3 initPosition = Vector3.zero;  // 初期座標
        private Vector3 prePosition = Vector3.zero;  // 前フレームでの座標
        private float staminaT = 0;  // スタミナ増減の時間カウンタ
        private bool isStaminaIncreasable = true;  // スタミナが回復可能かどうか
        private bool isCharging = false;  // 突進中かどうか

        private bool isInitPositionChecked = false;  // initPositionが有効な値かどうか、確認したフラグ

        public PlayerMoveBhv(PlayerFigure figure1, PlayerFigure figure2,
            Cinemachine.CinemachineFreeLook freeLookCamera, Transform freeLookCameraTf, Transform stackStartPoint,
            Transform staminaGaugeParentTf, Transform staminaGaugeTf, SpriteRenderer staminaGaugeSr,
            Transform initPlaceTf, Border.Border stageBorder)
        {
            this.figure1 = figure1;
            this.figure2 = figure2;
            this.freeLookCamera = freeLookCamera;
            this.freeLookCameraTf = freeLookCameraTf;
            this.stackStartPoint = stackStartPoint;
            initPosition = initPlaceTf.position;
            prePosition = initPosition;
            this.stageBorder = stageBorder;

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
            freeLookCamera = null;
            freeLookCameraTf = null;
            stackStartPoint = null;
            stageBorder = null;
        }

        public void Update()
        {
            if (figure1.IsNullExist()) return;
            if (figure2.IsNullExist()) return;
            if (!freeLookCamera) return;
            if (!freeLookCameraTf) return;
            if (!stackStartPoint) return;
            if (!stageBorder) return;

            if (!isInitPositionChecked)
            {
                isInitPositionChecked = true;

                if (!stageBorder.IsIn(initPosition))
                    throw new System.Exception($"プレイヤーの初期座標 {initPosition} が、ステージの範囲内にありません");
            }

            // 変身
            TransformBhv();

            // 慣性を消す
            ResetInertiaBhv();

            // 回転・移動
            TurnBhv();
            MoveBhv();

            // 特殊行動
            ChargeBhv();
            GetItemBhv();  // 中身なし
            TradeBhv();  // 中身なし

            // スタミナゲージの親を更新
            SetStaminaGaugeParentBhv();

            // 積み上げる場所を更新
            UpdateStackStartPointBhv();
        }

        // 引数以外のインデックスをすべて取得
        private List<FigureIndex> GetOtherFigureIndices(FigureIndex i)
        {
            return i switch
            {
                FigureIndex.Fox => new() { FigureIndex.Human },
                FigureIndex.Human => new() { FigureIndex.Fox },
                _ => throw new System.Exception("無効なインデックス番号です")
            };
        }

        // 引数のインデックスに対応するPlayerFigureを取得
        private PlayerFigure GetFigure(FigureIndex i)
        {
            return i switch
            {
                FigureIndex.Fox => figure1,
                FigureIndex.Human => figure2,
                _ => throw new System.Exception("無効なインデックス番号です")
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

        // oldIからnewIに形態変化
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

        // 変身する
        private void TransformBhv()
        {
            if (!InputGetter.Instance.Main_TransformClick.Get<bool>()) return;

            System.Action action = figureIndex switch
            {
                FigureIndex.Fox => () => ChangeFigure(FigureIndex.Fox, FigureIndex.Human),
                FigureIndex.Human => () => ChangeFigure(FigureIndex.Human, FigureIndex.Fox),
                _ => throw new System.Exception("無効なインデックス番号です")
            };

            action();
        }

        // 積み上げる場所を更新する
        private void UpdateStackStartPointBhv()
        {
            stackStartPoint.position = GetFigure(figureIndex).StackRoot.position;
        }

        // 慣性を消す(Rigidbodyの速度の、xとzを0にする)
        private void ResetInertiaBhv()
        {
            Vector3 vel = GetFigure(figureIndex).FigureRb.velocity;
            vel.x = 0; vel.z = 0;
            GetFigure(figureIndex).FigureRb.velocity = vel;
        }

        // プレイヤーをカメラの方向に回転させる
        private void TurnBhv()
        {
            PlayerFigure fgr = GetFigure(figureIndex);
            Vector3 lookAtLocal = fgr.FigureTf.position - freeLookCameraTf.position;
            Vector3 lookAtLocalXZ = new(lookAtLocal.x, 0, lookAtLocal.z);
            Vector3 lookAtXZ = fgr.FigureTf.position + lookAtLocalXZ;
            fgr.FigureTf.LookAt(lookAtXZ);

            // 他の形態を現在の形態にコンストレイン
            Quaternion toQua = GetFigure(figureIndex).FigureTf.rotation;
            foreach (FigureIndex e in GetOtherFigureIndices(figureIndex))
            {
                GetFigure(e).FigureTf.rotation = toQua;
            }
        }

        // 移動
        private void MoveBhv()
        {
            PlayerFigure fgr = GetFigure(figureIndex);
            if (fgr.FigureTf.position.y < -100) fgr.FigureTf.position = initPosition;  // 落下し過ぎたら、初期座標に戻す
            Vector2 moveValueInputted = InputGetter.Instance.Main_MoveValue2.Get<Vector2>();

            // 動いていない
            if (moveValueInputted == Vector2.zero)
            {
                fgr.Animator.SetBool("IsMoving", false);
                return;
            }
            // 動いている
            else
            {
                Vector3 moveValueLocalNormed = new Vector3(moveValueInputted.x, 0, moveValueInputted.y).normalized;
                Vector3 moveValueLocal = moveValueLocalNormed * (moveSpeed * Time.deltaTime);
                Vector3 moveValue = fgr.FigureTf.right * moveValueLocal.x + fgr.FigureTf.forward * moveValueLocal.z;
                fgr.FigureTf.position += moveValue;
                if (!stageBorder.IsIn(fgr.FigureTf.position)) fgr.FigureTf.position = prePosition;
                prePosition = fgr.FigureTf.position;

                // 他の形態を現在の形態にコンストレイン
                Vector3 toPos = GetFigure(figureIndex).FigureTf.position;
                foreach (FigureIndex e in GetOtherFigureIndices(figureIndex))
                {
                    GetFigure(e).FigureTf.position = toPos;
                }

                fgr.Animator.SetBool("IsMoving", true);
            }
        }

        // 突進
        private void ChargeBhv()
        {
            if (figureIndex != FigureIndex.Fox) return;

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

        // アイテム取得
        private void GetItemBhv()
        {
            if (figureIndex != FigureIndex.Fox) return;
        }

        // 交易
        private void TradeBhv()
        {
            if (figureIndex != FigureIndex.Human) return;
        }

        // スタミナゲージの親を更新する
        private void SetStaminaGaugeParentBhv()
        {
            PlayerFigure fgr = GetFigure(figureIndex);
            playerStamina.SetGaugeParent(fgr.FigureTf, fgr.StaminaGaugeRoot.localPosition);
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

        // スタミナを増やす。この処理でスタミナが最大に「なった」時のみ、trueを返す。
        internal bool IncreaseStamina(int diff)
        {
            if (!gaugeParentTf) return false;
            if (!gaugeTf) return false;
            if (!gaugeSr) return false;

            if (diff <= 0) throw new System.Exception("diffは正である必要があります");
            if (stamina == SO_Player.Entity.MaxStamina) return false;

            stamina += diff;

            UpdateGauge();

            return IsStaminaFull();
        }

        // スタミナを減らす。この処理でスタミナが最小に「なった」時のみ、trueを返す。
        internal bool DecreaseStamina(int diff)
        {
            if (!gaugeParentTf) return false;
            if (!gaugeTf) return false;
            if (!gaugeSr) return false;

            if (diff <= 0) throw new System.Exception("diffは正である必要があります");
            if (stamina == 0) return false;

            stamina -= diff;

            UpdateGauge();

            return IsStaminaZero();
        }

        // ゲージのスケールと色を更新する
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

        // スタミナが最大ならtrue、そうでないならfalse。
        internal bool IsStaminaFull() { return stamina == SO_Player.Entity.MaxStamina; }

        // スタミナが最小ならtrue、そうでないならfalse。
        internal bool IsStaminaZero() { return stamina == 0; }
    }

    internal static class Ex
    {
        // 区間X[a, b]を、区間Y[c, d]に線形マッピングしたとする。区間X内の値xについて、変換後の区間Yにおける値yを返す。
        internal static float Remap(float a, float b, float c, float d, float x)
        {
            float y = (x - a) * (d - c) / (b - a) + c;
            return y;
        }
    }
}