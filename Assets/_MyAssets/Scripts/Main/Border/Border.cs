using System.Collections.Generic;
using UnityEngine;

namespace Main.Border
{
    [ExecuteInEditMode]
    internal sealed class Border : MonoBehaviour
    {
        [SerializeField, Header("�s���̐e��Transform(���v���Ɉ͂����ƁI)")] private Transform pins;
        [SerializeField, Header("LineRenderer�R���|�[�l���g")] private LineRenderer lineRenderer;
        [Space(25)]
        [SerializeField, Header("LineRenderer�R���|�[�l���g���A�N�e�B�u�ɂ���")] private bool isLineRendererActive;
        [SerializeField, Header("���̑���")] private float thin;
        [SerializeField, Header("���̐F")] private Color color;

        private List<Transform> pinList = new();

#if UNITY_EDITOR
        private void OnEnable()
        {
            if (IsPlaying())
            {
                if (!pins) throw new System.Exception($"{nameof(pins)}���ݒ肳��Ă��܂���");
                if (!lineRenderer) throw new System.Exception($"{nameof(lineRenderer)}���ݒ肳��Ă��܂���");

                if (isLineRendererActive) Debug.LogWarning($"{nameof(lineRenderer)}���A�N�e�B�u�ł�");

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
                if (!pins) throw new System.Exception($"{nameof(pins)}���ݒ肳��Ă��܂���");
                if (!lineRenderer) throw new System.Exception($"{nameof(lineRenderer)}���ݒ肳��Ă��܂���");

                UpdatePins(isLineRendererActive);
            }
        }
#else
        private void OnEnable()
        {
            if (!pins) throw new System.Exception($"{nameof(pins)}���ݒ肳��Ă��܂���");
            if (!lineRenderer) throw new System.Exception($"{nameof(lineRenderer)}���ݒ肳��Ă��܂���");

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
        /// �͈͂̒��Ɋ܂܂�Ă��邩�ǂ������ׂ�(y���𖳎�����)
        /// </summary>
        internal bool IsIn(Vector2 pos)
        {
            if (pinList == null) throw new System.Exception($"{pinList}��null�ł�");
            if (pinList.Count <= 2) throw new System.Exception($"pin�̐���2�ȉ��ł�");

            float th = 0;
            for (int i = 0; i < pinList.Count; i++)
            {
                Vector2 fromPinPos = pinList[i].position.XOZ2XY();
                Vector2 toPinPos = ((i < pinList.Count - 1) ? pinList[i + 1].position : pinList[0].position).XOZ2XY();

                Vector2 fromVec = fromPinPos - pos;
                Vector2 toVec = toPinPos - pos;

                float dth = Mathf.Acos(Vector2.Dot(toVec.normalized, fromVec.normalized));  // ��]�p�̐�Βl�A[0, ��]
                if ((fromVec, toVec).Cross() < 0) dth *= -1;

                th += dth;
            }

            return Mathf.Abs(th) >= 0.01f;
        }

        /// <summary>
        /// �͈͂̒��Ɋ܂܂�Ă��邩�ǂ������ׂ�(y���𖳎�����)
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