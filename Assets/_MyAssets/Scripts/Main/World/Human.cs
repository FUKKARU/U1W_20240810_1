using General;
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
        [SerializeField] SoundPlayer soundPlayer;

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
            soundPlayer.Play(SoundType.General_ClickSE);

            tradeRateText.text = humanSO.EndMessage;
            tradeButton.interactable = false;
        }

        public void OpenTrade()
        {
            soundPlayer.Play(SoundType.General_ClickSE);

            tradeUI.SetActive(true);
            tradeRateText.text = "èäéùêî         :ÉäÉìÉS: " + playerCollectScript.appleNum + "ÉLÉmÉR:" + playerCollectScript.kinokoNum + "\nåà’ÉåÅ[Ég:ÉäÉìÉS: " + appleRate + "ÉLÉmÉR: " + kinokoRate;
            tradeButton.interactable =
                playerCollectScript.kinokoNum < kinokoRate || playerCollectScript.appleNum < appleRate
                ? false : true;
            tradeButton.onClick.RemoveAllListeners();
            tradeButton.onClick.AddListener(TradeAburaage);
            tradeButton.onClick.AddListener(() => playerCollectScript.GetAburaage(appleRate, kinokoRate));
        }
    }
}