using Main.Player;
using SO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Main.UI
{
    public class CloseButton : MonoBehaviour
    {
        SO_Tag tagSO;
        void Awake()
        {
            tagSO = SO_Tag.Entity;
            Button closeButton = GetComponent<Button>();
            PlayerCollect playerCollect = GameObject.FindGameObjectWithTag(tagSO.PlayerTag).transform.Find("Figures/Figure_1").GetComponent<PlayerCollect>();
            closeButton.onClick.AddListener(playerCollect.OnIneractFalse);
        }
    }
}