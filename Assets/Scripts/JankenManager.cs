using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using TMPro;
using UnityEngine;

public class JankenManager : NetworkBehaviour
{
    public TextMeshProUGUI resultText;
    private int choiceP1 = -1;
    private int choiceP2 = -1;
    private NetworkConnection connectionP1;
    public void BotonPiedra() { SubmitPlay(0); }
    public void BotonPapel() { SubmitPlay(1); }
    public void BotonTijera() { SubmitPlay(2); }

    [ServerRpc(RequireOwnership = false)]
    public void SubmitPlay(int choice, NetworkConnection caller = null)
    {
        if (connectionP1 == null) 
        {
            connectionP1 = caller;
            choiceP1 = choice;
            ShowResult("Jugador 1 está listo. Esperando al rival...");
        }
        else if (connectionP1 == caller)
        {
            choiceP1 = choice;
        }
        else 
        {
            choiceP2 = choice;
        }
        if (choiceP1 != -1 && choiceP2 != -1)
        {
            SolveGame();
        }
    }

    private void SolveGame()
    {
        string finalMessage;

        if(choiceP1 == choiceP2)
        {
            finalMessage = "ES UN EMPATE";
        } else if  ((choiceP1 == 0 && choiceP2 == 2) ||
                    (choiceP1 == 1 && choiceP2 == 0) || 
                    (choiceP1 == 2 && choiceP2 == 1))  
        {
            finalMessage = "¡GANO DEL JUGADOR 1!";
        }
        else
        {
            finalMessage = "¡GANO DEL JUGADOR 2!";
        }

        ShowResult(finalMessage);
        choiceP1 = -1;
        choiceP2 = -1;
        connectionP1 = null;
    }

    [ObserversRpc]
    private void ShowResult(string result)
    {
        resultText.text = result;
    }
}
