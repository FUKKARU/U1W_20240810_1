using System.Collections.Generic;
using UnityEngine;

namespace Main.Border
{
    [ExecuteInEditMode]
    internal sealed class Border : MonoBehaviour
    {
        [SerializeField, Header("ピンの親のTransform(時計回りに囲うこと！)")] private Transform pins;
        [SerializeField, Header("LineRendererコンポーネント")] private LineRenderer lineRenderer;
        [Space(25)]
        [SerializeField, Header("LineRendererコンポーネントをアクティブにする")] private bool isLineRendererActive;
        [SerializeField, Header("線の太さ")] private float thin;
        [SerializeField, Header("線のマテリアル")] private Material material;
        [SerializeField, Header("線の色")] private Color color;

        private readonly List<Transform> pinList = new();

        private enum State
        {
            Editor_Editing,
            Editor_Playing,
            Build
        }

        private void OnEnable()
        {
            System.Action action = GetState() switch
            {
                State.Editor_Editing => () => General.Ex.Pass(),
                State.Editor_Playing => () => UpdatePins(),
                State.Build => () => UpdatePins(),
                _ => throw new System.Exception("無効な値です")
            };
            action();
        }

        private void OnDisable()
        {
            System.Action action = GetState() switch
            {
                State.Editor_Editing => () => General.Ex.Pass(),
                State.Editor_Playing => () => Dispose(),
                State.Build => () => Dispose(),
                _ => throw new System.Exception("無効な値です")
            };
            action();
        }

        private void Update()
        {
            System.Action action = GetState() switch
            {
                State.Editor_Editing => () => UpdatePins(),
                State.Editor_Playing => () => General.Ex.Pass(),
                State.Build => () => General.Ex.Pass(),
                _ => throw new System.Exception("無効な値です")
            };
            action();
        }

        private State GetState()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying) return State.Editor_Playing;
            else return State.Editor_Editing;
#else
            return State.Build;
#endif
        }

        private void Dispose()
        {
            // 明示的null代入
            pins = null;
            lineRenderer = null;
            material = null;
        }

        private void UpdatePins()
        {
            // nullチェック
            if (!pins) throw new System.Exception($"{nameof(pins)}が設定されていません");
            if (!lineRenderer) throw new System.Exception($"{nameof(lineRenderer)}が設定されていません");

            // ビルドデータまたはエディタで実行中なら、強制非アクティブにする！
            bool active = (GetState() == State.Editor_Editing) ? isLineRendererActive : false;

            // エディタで実行中だけど、アクティブになっているよ？いいの？
            if (GetState() == State.Editor_Playing && active)
                Debug.LogWarning($"{nameof(lineRenderer)}がアクティブです");

            // ここで実際に、アクティブ状態を設定
            lineRenderer.enabled = active;
            foreach (Transform e in pins)
                e.gameObject.GetComponent<MeshRenderer>().enabled = active;

            int pinNum = pins.childCount;

            // ピンのリストを更新
            pinList.Clear();
            for (int i = 0; i < pinNum; i++)
                pinList.Add(pins.GetChild(i));

            // 非アクティブなら、ここから先はやらなくて良い
            if (!active) return;

            // マテリアルと色設定
            Material mat = new(material) { color = color };
            lineRenderer.sharedMaterial = mat;

            // 線を描画する
            lineRenderer.startWidth = thin;
            lineRenderer.endWidth = thin;
            lineRenderer.positionCount = pinNum + 1;
            for (int i = 0; i < pinNum; i++)
                lineRenderer.SetPosition(i, pinList[i].position);
            lineRenderer.SetPosition(pinNum, pinList[0].position);
        }

        /// <summary>
        /// 範囲の中に含まれているかどうか調べる(y軸を無視する)
        /// </summary>
        internal bool IsIn(Vector2 pos)
        {
            if (pinList == null) throw new System.Exception($"{pinList}がnullです");
            if (pinList.Count <= 2) throw new System.Exception($"{pinList}の要素の数が2つ以下です");

            float th = 0;
            for (int i = 0; i < pinList.Count; i++)
            {
                Vector2 fromPinPos = pinList[i].position.XOZ2XY();
                Vector2 toPinPos = ((i < pinList.Count - 1) ? pinList[i + 1].position : pinList[0].position).XOZ2XY();

                Vector2 fromVec = fromPinPos - pos;
                Vector2 toVec = toPinPos - pos;

                float dth = Mathf.Acos(Vector2.Dot(toVec.normalized, fromVec.normalized));  // 回転角の絶対値、[0, π]
                if ((fromVec, toVec).Cross() < 0) dth *= -1;

                th += dth;
            }

            return Mathf.Abs(th) >= 0.01f;
        }

        /// <summary>
        /// 範囲の中に含まれているかどうか調べる(y軸を無視する)
        /// </summary>
        internal bool IsIn(Vector3 pos)
        {
            return IsIn(pos.XOZ2XY());
        }
    }

    internal static class Ex
    {
        internal static Vector2 XOZ2XY(this Vector3 v)
        {
            return new(v.x, v.z);
        }

        internal static Vector3 XY2XOZ(this Vector2 v)
        {
            return new(v.x, 0, v.y);
        }

        internal static float Cross(this (Vector2 a, Vector2 b) v)
        {
            return v.a.x * v.b.y - v.a.y * v.b.x;
        }
    }
}