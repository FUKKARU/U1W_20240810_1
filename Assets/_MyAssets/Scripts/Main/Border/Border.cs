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
        [SerializeField, Header("���̃}�e���A��")] private Material material;
        [SerializeField, Header("���̐F")] private Color color;

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
                _ => throw new System.Exception("�����Ȓl�ł�")
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
                _ => throw new System.Exception("�����Ȓl�ł�")
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
                _ => throw new System.Exception("�����Ȓl�ł�")
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
            // �����Inull���
            pins = null;
            lineRenderer = null;
            material = null;
        }

        private void UpdatePins()
        {
            // null�`�F�b�N
            if (!pins) throw new System.Exception($"{nameof(pins)}���ݒ肳��Ă��܂���");
            if (!lineRenderer) throw new System.Exception($"{nameof(lineRenderer)}���ݒ肳��Ă��܂���");

            // �r���h�f�[�^�܂��̓G�f�B�^�Ŏ��s���Ȃ�A������A�N�e�B�u�ɂ���I
            bool active = (GetState() == State.Editor_Editing) ? isLineRendererActive : false;

            // �G�f�B�^�Ŏ��s�������ǁA�A�N�e�B�u�ɂȂ��Ă����H�����́H
            if (GetState() == State.Editor_Playing && active)
                Debug.LogWarning($"{nameof(lineRenderer)}���A�N�e�B�u�ł�");

            // �����Ŏ��ۂɁA�A�N�e�B�u��Ԃ�ݒ�
            lineRenderer.enabled = active;
            foreach (Transform e in pins)
                e.gameObject.GetComponent<MeshRenderer>().enabled = active;

            int pinNum = pins.childCount;

            // �s���̃��X�g���X�V
            pinList.Clear();
            for (int i = 0; i < pinNum; i++)
                pinList.Add(pins.GetChild(i));

            // ��A�N�e�B�u�Ȃ�A���������͂��Ȃ��ėǂ�
            if (!active) return;

            // �}�e���A���ƐF�ݒ�
            Material mat = new(material) { color = color };
            lineRenderer.sharedMaterial = mat;

            // ����`�悷��
            lineRenderer.startWidth = thin;
            lineRenderer.endWidth = thin;
            lineRenderer.positionCount = pinNum + 1;
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
            if (pinList.Count <= 2) throw new System.Exception($"{pinList}�̗v�f�̐���2�ȉ��ł�");

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