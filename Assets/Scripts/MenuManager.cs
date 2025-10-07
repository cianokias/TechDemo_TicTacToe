using System;
using System.Collections.Generic;
using System.Linq;
using TicTacToe;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Dropdown gameModeDropdown;
    [SerializeField] private TMP_Dropdown aiTypeDropdown;
    [SerializeField] private Toggle vsAIToggle;
    [SerializeField] private Toggle playerGoesFirstToggle;
    
    void Start()
    {
        PopulateDropdownFromEnum<GameModeType>(gameModeDropdown);
        PopulateDropdownFromEnum<AIType>(aiTypeDropdown);
        ReadValuesFromSetting();
    }
    
    private void PopulateDropdownFromEnum<T>(TMP_Dropdown dropdown) where T : Enum
    {
        dropdown.ClearOptions();
        var options = Enum.GetNames(typeof(T)).ToList();
        dropdown.AddOptions(options);
    }

    private void ReadValuesFromSetting()
    {
        gameModeDropdown.value = (int)GameSettings.GameMode;
        aiTypeDropdown.value = (int)GameSettings.AIType;
        vsAIToggle.isOn = GameSettings.VsAI;
        playerGoesFirstToggle.isOn = GameSettings.PlayerGoesFirst;
    }
    
    // For start button
    public void OnStartButtonClick()
    {
        GameSettings.GameMode = (GameModeType)gameModeDropdown.value;
        GameSettings.AIType = (AIType)aiTypeDropdown.value;
        GameSettings.VsAI = vsAIToggle.isOn;
        GameSettings.PlayerGoesFirst = playerGoesFirstToggle.isOn;
        SceneManager.LoadScene("MainGame");
    }
    
    // For vs ai toggle
    public void OnVsAIToggle(bool isOn)
    {
        aiTypeDropdown.gameObject.SetActive(isOn);
        playerGoesFirstToggle.gameObject.SetActive(isOn);
    }
    
}