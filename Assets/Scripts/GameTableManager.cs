using FishNet;
using FishNet.Connection;
using FishNet.Object;
using TMPro;
using UnityEngine;

public class TableGameManager : NetworkBehaviour
{
    [Header("UI Connections")]
    public GameObject gameCanvas;
    public TextMeshProUGUI promptText;

    [Header("Settings")]
    public float interactDistance = 3.0f;

    [Header("Dependencies")]
    public JankenManager jankenManager;

    private NetworkConnection player1Ready;
    private NetworkConnection player2Ready;
    private Transform localPlayerTransform;
    private bool isLocalPlayerReady = false;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (gameCanvas) gameCanvas.SetActive(false);
        if (promptText) promptText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (gameCanvas.activeSelf) 
        {
            promptText.gameObject.SetActive(false);
            return;
        }

        if(PlayerController.LocalPlayer == null) return;

        float distance = Vector3.Distance(transform.position, PlayerController.LocalPlayer.position);

        if (distance <= interactDistance)
        {
            promptText.gameObject.SetActive(true);

            if (!isLocalPlayerReady)
            {
                promptText.text = "Press E to Ready Up";
                if (Input.GetKeyDown(KeyCode.E))
                {
                    isLocalPlayerReady = true;
                    SetPlayerReady();
                }
            }
            else
            {
                promptText.text = "Waiting for the other player...";
            }
        }
        else
        {
            promptText.gameObject.SetActive(false);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReady(NetworkConnection caller = null)
    {
        if (player1Ready == null) 
        {
            player1Ready = caller;
        }
        else if (player1Ready != caller && player2Ready == null) 
        {
            player2Ready = caller;
        }

        if (player1Ready != null && player2Ready != null)
        {
            if (jankenManager != null)
            {
                jankenManager.ConfigurePlayers(player1Ready, player2Ready);
            }
            StartGame();
        }
    }

    [ObserversRpc]
    private void StartGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (promptText) promptText.gameObject.SetActive(false);
        if (gameCanvas) gameCanvas.SetActive(true);
    }
}