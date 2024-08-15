using System.Collections.Generic;
using UnityEngine;
using General;

namespace Main.Border
{
    [ExecuteInEditMode]
    public sealed class Border : MonoBehaviour
    {
        #region

        public static Border Instance { get; private set; } = null;

        private void OnEnable()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying) return;
#endif

            if (!Instance)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying) return;
#endif

            pinList = null;
            pins = null;
            lineRenderer = null;
            Instance = null;
        }

        #endregion

        [SerializeField, Header("�s���̐e��Transform(���v���Ɉ͂����ƁI)")] private Transform pins;
        [SerializeField, Header("LineRenderer�R���|�[�l���g")] private LineRenderer lineRenderer;
        [Space(25)]
        [SerializeField, Header("LineRenderer�R���|�[�l���g���A�N�e�B�u�ɂ���")] private bool isLineRendererActive;
        [SerializeField, Header("���̑���")] private float thin;

        private List<Transform> pinList = new();

        private void Update()
        {
            if (!pins) throw new System.Exception($"{nameof(pins)}���ݒ肳��Ă��܂���");
            if (!lineRenderer) throw new System.Exception($"{nameof(lineRenderer)}���ݒ肳��Ă��܂���");

#if UNITY_EDITOR
            bool _isLineRendererActive = isLineRendererActive;
#else
            bool _isLineRendererActive = false;
#endif

            lineRenderer.enabled = _isLineRendererActive;
            foreach (Transform e in pins)
                e.gameObject.SetActive(_isLineRendererActive);

            if (!_isLineRendererActive) return;

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
        public bool IsIn(Vector2 pos)
        {
            if (pinList == null) return false;
            if (pinList.Count <= 2) return false;

            for (int i = 0; i < pinList.Count; i++)
            {
                Vector2 from = pinList[i].position.XOZ2XY();
                Vector2 to = ((i < pinList.Count - 1) ? pinList[i + 1].position : pinList[0].position).XOZ2XY();

                Vector2 axisBase = to - from;
                Vector2 axisTarget = pos - from;

                if ((axisBase, axisTarget).Cross() > 0)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// �͈͂̒��Ɋ܂܂�Ă��邩�ǂ������ׂ�(y���𖳎�����)
        /// </summary>
        public bool IsIn(Vector3 pos)
        {
            return IsIn(pos.XOZ2XY());
        }
    }
}