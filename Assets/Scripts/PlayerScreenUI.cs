using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PlayerScreenUI : MonoBehaviour
{
    [Header("General")]
    public UIManager uiManager;

    [Header("Screens")]
    public GameObject mainInfo;
    public GameObject firstAidInfo;

    [Header("Main screen info")]
    public TextMeshProUGUI nameText;

    public Slider playersHealthSlider;
    public Slider playersArmourSlider;
    public TextMeshProUGUI playersHealthText;
    public TextMeshProUGUI playersArmourText;

    public TextMeshProUGUI primaryCostText;
    public TextMeshProUGUI secondaryCostText;

    public TextMeshProUGUI tokenText;

    private void Start()
    {
        mainInfo.SetActive(true);
        firstAidInfo.SetActive(false);
    }

    public void DisplayPlayerInfo(int _playerHealthMax, int _playerHealthValue, int _playerArmourMax, int _playerArmourValue, string _playerName, int _primaryCost, int _secondaryCost, int _tokens)
    {
        UpdateHealthAndArmour(_playerHealthMax, _playerHealthValue, _playerArmourMax, _playerArmourValue);

        nameText.text                = "" + _playerName;
        tokenText.text               = "" + _tokens;

        primaryCostText.text         = "Primary\n" + _primaryCost;
        secondaryCostText.text       = "Secondary\n" + _secondaryCost;
    }

    public void UpdateHealthAndArmour(int _playerHealthMax, int _playerHealthValue, int _playerArmourMax, int _playerArmourValue)
    {
        playersHealthSlider.maxValue = _playerHealthMax;
        playersHealthSlider.value = _playerHealthValue;

        playersArmourSlider.maxValue = _playerArmourMax;
        playersArmourSlider.value = _playerArmourValue;

        playersHealthText.text = _playerHealthValue + "/" + _playerHealthMax;
        playersArmourText.text = _playerArmourValue + "/" + _playerArmourMax;
    }

    public void FirstAidPressed()
    {
        mainInfo.SetActive(false);
        firstAidInfo.SetActive(true);
    }

    public void ShowMainInfo()
    {
        mainInfo.SetActive(true);
        firstAidInfo.SetActive(false);
    }
}
