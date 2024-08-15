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
        [SerializeField, Header("線の色")] private Color color;

        private List<Transform> pinList = new();

#if UNITY_EDITOR
        private void OnEnable()
        {
            if (IsPlaying())
            {
                if (!pins) throw new System.Exception($"{nameof(pins)}が設定されていません");
                if (!lineRenderer) throw new System.Exception($"{nameof(lineRenderer)}が設定されていません");

                if (isLineRendererActive) Debug.LogWarning($"{nameof(lineRenderer)}がアクティブです");

                UpdatePins(isLineRendererActive);
            }
            else
            {

            }
        }

        private void OnDisable()
        {
            if (IsPlaying())
            {
                pinList = null;
                pins = null;
                lineRenderer = null;
            }
            else
            {

            }
        }

        private void Update()
        {
            if (IsPlaying())
            {

            }
            else
            {
                if (!pins) throw new System.Exception($"{nameof(pins)}が設定されていません");
                if (!lineRenderer) throw new System.Exception($"{nameof(lineRenderer)}が設定されていません");

                UpdatePins(isLineRendererActive);
            }
        }
#else
        private void OnEnable()
        {
            if (!pins) throw new System.Exception($"{nameof(pins)}が設定されていません");
            if (!lineRenderer) throw new System.Exception($"{nameof(lineRenderer)}が設定されていません");

            UpdatePins(false);
        }

        private void OnDisable()
        {
            pinList = null;
            pins = null;
            lineRenderer = null;
        }

        private void Update()
        {

        }
#endif

#if UNITY_EDITOR
        private bool IsPlaying()
        {
            return UnityEditor.EditorApplication.isPlaying;
        }
#endif

        private void UpdatePins(bool _isLineRendererActive)
        {
            lineRenderer.enabled = _isLineRendererActive;
            foreach (Transform e in pins)
                e.gameObject.SetActive(_isLineRendererActive);

            if (!_isLineRendererActive) return;

            lineRenderer.material.color = color;

            int pinNum = pins.childCount;

            lineRenderer.startWidth = thin;
            lineRenderer.endWidth = thin;
            lineRenderer.positionCount = pinNum + 1;

            pinList = new();
            for (int i = 0; i < pinNum; i++)
                pinList.Add(pins.GetChild(i));

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
            if (pinList.Count <= 2) throw new System.Exception($"pinの数が2つ以下です");

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