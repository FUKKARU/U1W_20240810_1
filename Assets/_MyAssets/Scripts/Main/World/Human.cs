using Main.Player;
using SO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Trade
{
    public class Human : MonoBehaviour
    {
        TextMeshProUGUI tradeRateText;
        PlayerCollect playerCollectScript;
        Button tradeButton;
        int appleRate;
        int kinokoRate;
        GameObject tradeUI;

        SO_HierarchyPath hierarchyPathSO;
        SO_Tag tagSO;
        SO_Human humanSO;

        void Awake()
        {
            hierarchyPathSO = SO_HierarchyPath.Entity;
            tagSO = SO_Tag.Entity;
            humanSO = SO_Human.Entity;

            GameObject Canvas = GameObject.FindGameObjectWithTag(tagSO.CanvasTag);
            tradeUI = Canvas.transform.Find(hierarchyPathSO.TradeUI).gameObject;
            tradeRateText = tradeUI.transform.Find(hierarchyPathSO.TradeRateText).GetComponent<TextMeshProUGUI>();
            tradeButton = tradeUI.transform.Find(hierarchyPathSO.TradeButton).GetComponent<Button>();

            playerCollectScript = GameObject.FindGameObjectWithTag(tagSO.PlayerTag).transform.Find("PlayerCollect").GetComponent<PlayerCollect>();


            do
            {
                appleRate = Random.Range(humanSO.AppleRateMin, humanSO.AppleRateMax);
                kinokoRate = Random.Range(humanSO.KinokoRateMin, humanSO.AppleRateMax);
            } while (appleRate + kinokoRate > 10);

        }

        void TradeAburaage()
        {
            tradeRateText.text = humanSO.EndMessage;
            tradeButton.interactable = false;
        }

        public void OpenTrade()
        {
            tradeUI.SetActive(true);
            tradeRateText.text = "Š”         :ƒŠƒ“ƒS: " + playerCollectScript.appleNum + "ƒLƒmƒR:" + playerCollectScript.kinokoNum + "\nŒğˆÕƒŒ[ƒg:ƒŠƒ“ƒS: " + appleRate + "ƒLƒmƒR: " + kinokoRate;
            tradeButton.interactable =
                playerCollectScript.kinokoNum < kinokoRate || playerCollectScript.appleNum < appleRate
                ? false : true;
            tradeButton.onClick.RemoveAllListeners();
            tradeButton.onClick.AddListener(TradeAburaage);
            tradeButton.onClick.AddListener(() => playerCollectScript.GetAburaage(appleRate, kinokoRate));
        }
    }
}