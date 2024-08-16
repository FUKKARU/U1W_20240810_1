using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/SO_HierarchyPath", fileName = "SO_HierarchyPath")]
    public class SO_HierarchyPath : ScriptableObject
    {
        #region
        public const string PATH = "SO_HierarchyPath";

        private static SO_HierarchyPath _entity = null;
        public static SO_HierarchyPath Entity
        {
            get
            {
                if (_entity == null)
                {
                    _entity = Resources.Load<SO_HierarchyPath>(PATH);

                    if (_entity == null)
                    {
                        Debug.LogError(PATH + " not found");
                    }
                }

                return _entity;
            }
        }
        #endregion

        [SerializeField, Header("キャンバスからトレードUIへ")] string tradeUI;
        public string TradeUI => tradeUI;

        [SerializeField, Header("トレードからトレードレートテキストへ")] string tradeRateText;
        public string TradeRateText => tradeRateText;

        [SerializeField, Header("トレードからトレードボタンへ")] string tradeButton;
        public string TradeButton => tradeButton;

        [SerializeField, Header("キャンバスからトレードするか否かを示すUIへ")] string tradeOrNot;
        public string TradeOrNot => tradeOrNot;

        [SerializeField, Header("キャンバスから交易開始ボタンへ")] string initiateTradeButon;
        public string InitiateTradeButon => initiateTradeButon;
    }
}