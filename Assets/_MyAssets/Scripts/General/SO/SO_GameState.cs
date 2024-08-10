using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/SO_GameState", fileName = "SO_GameState")]
    public class SO_GameState : ScriptableObject
    {
        #region
        public const string PATH = "SO_GameState";

        private static SO_GameState _entity = null;
        public static SO_GameState Entity
        {
            get
            {
                if (_entity == null)
                {
                    _entity = Resources.Load<SO_GameState>(PATH);

                    if (_entity == null)
                    {
                        Debug.LogError(PATH + " not found");
                    }
                }

                return _entity;
            }
        }
        #endregion

        [Header("�𑜓x(���~�c)")] public Vector2Int Resolution;
        [Header("�t���X�N���[���ɂ��邩")] public bool IsFullScreen;
        [Header("Vsync���I���ɂ��邩")] public bool IsVsyncOn;
        [Header("Vsync���I�t�̏ꍇ�A�^�[�Q�b�g�t���[�����[�g")] public byte TargetFrameRate;
    }
}