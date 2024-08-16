using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/SO_Tag", fileName = "SO_Tag")]
    public class SO_Tag : ScriptableObject
    {
        #region
        public const string PATH = "SO_Tag";

        private static SO_Tag _entity = null;
        public static SO_Tag Entity
        {
            get
            {
                if (_entity == null)
                {
                    _entity = Resources.Load<SO_Tag>(PATH);

                    if (_entity == null)
                    {
                        Debug.LogError(PATH + " not found");
                    }
                }

                return _entity;
            }
        }
        #endregion

        [SerializeField, Header("プレイヤー")] private string playerTag;
        public string PlayerTag => playerTag;

        [SerializeField, Header("人間")] private string humanTag;
        public string HumanTag => humanTag;

        [SerializeField, Header("木")] private string treeTag;
        public string TreeTag => treeTag;

        [SerializeField, Header("リンゴ")] private string appleTag;
        public string AppleTag => appleTag;

        [SerializeField, Header("キノコ")] private string kinokoTag;
        public string KinokoTag => kinokoTag;

        [SerializeField, Header("スポーン可能範囲")] private string spawnRangeTag;
        public string SpawnRangeTag => spawnRangeTag;

        [SerializeField, Header("キャンバス")] private string canvasTag;
        public string CanvasTag => canvasTag;
    }
}