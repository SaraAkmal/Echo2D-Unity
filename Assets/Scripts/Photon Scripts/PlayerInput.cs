using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{
    private const string PlayerPrefsNameKey = "PlayerName";
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button continueButton;

    private void Start()
    {
        SetUpInputField();
    }

    private void Update()
    {
    }

    private void SetUpInputField()
    {
        if (!PlayerPrefs.HasKey(PlayerPrefsNameKey)) return;
        var defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);
        nameInputField.text = defaultName;
    }


    public void CheckJoinButton()
    {
        if (nameInputField.text != "" && nameInputField.text != null)
            continueButton.interactable = true;
        else
            continueButton.interactable = false;
    }

    public void SavePlayerName()
    {
        var playerName = nameInputField.text;
        PhotonNetwork.NickName = playerName;
        PlayerPrefs.SetString(PlayerPrefsNameKey, playerName);
    }
}