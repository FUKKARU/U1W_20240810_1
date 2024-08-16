using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/SO_Human", fileName = "SO_Human")]
    public class SO_Human : ScriptableObject
    {
        #region
        public const string PATH = "SO_Human";

        private static SO_Human _entity = null;
        public static SO_Human Entity
        {
            get
            {
                if (_entity == null)
                {
                    _entity = Resources.Load<SO_Human>(PATH);

                    if (_entity == null)
                    {
                        Debug.LogError(PATH + " not found");
                    }
                }

                return _entity;
            }
        }
        #endregion

        [SerializeField, Header("交易終了時のメッセージ")] private string endMessage;
        public string EndMessage => endMessage;

        [SerializeField, Header("キノコ交易レート最大数")] private int  kinokoRateMax;
        public int  KinokoRateMax => kinokoRateMax;

        [SerializeField, Header("キノコ交易レート最小数")] private int kinokoRateMin;
        public int KinokoRateMin => kinokoRateMin;

        [SerializeField, Header("リンゴ交易レート最大数")] private int appleRateMax;
        public int AppleRateMax => appleRateMax;

        [SerializeField, Header("リンゴ交易レート最小数")] private int appleRateMin;
        public int AppleRateMin => appleRateMin;

    }
}