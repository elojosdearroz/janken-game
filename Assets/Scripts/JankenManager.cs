using System.Collections;
using FishNet.Connection;
using FishNet.Object;
using TMPro;
using UnityEngine;

public class JankenManager : NetworkBehaviour
{
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI scoreText;
    public GameTableManager table;
    private NetworkConnection connectionP1;
    private NetworkConnection connectionP2;
    private int scoreP1 = 0;
    private int scoreP2 = 0;

    private bool isPhase2 = false;
    private int fakeChoiceP1 = -1;
    private int fakeChoiceP2 = -1;
    private int choiceP1 = -1;
    private int choiceP2 = -1;
    public void ConfigurePlayers(NetworkConnection p1, NetworkConnection p2)
    {
        connectionP1 = p1;
        connectionP2 = p2;

        scoreP1 = 0;
        scoreP2 = 0;

        ResetRoundState();
        UpdateScoreUI(0, 0);
    }

    private void ResetRoundState()
    {
        isPhase2 = false;
        fakeChoiceP1 = -1;
        fakeChoiceP2 = -1;
        choiceP1 = -1;
        choiceP2 = -1;
    }
    
    public void BotonPiedra() => SubmitPlay(0);
    public void BotonPapel()  => SubmitPlay(1);
    public void BotonTijera() => SubmitPlay(2);

    [ServerRpc(RequireOwnership = false)]
    public void SubmitPlay(int choice, NetworkConnection caller = null)
    {
        if (!isPhase2)
        {
            if (caller == connectionP1)
            {
                fakeChoiceP1 = choice;
                ShowResult($"Player 1 chose: {GetChoiceName(choice)}");
            }
            else if (caller == connectionP2)
            {
                fakeChoiceP2 = choice;
                ShowResult($"Player 2 chose: {GetChoiceName(choice)}");
            }
            else return;

            if (fakeChoiceP1 != -1 && fakeChoiceP2 != -1)
            {
                isPhase2 = true;
                ShowResult($"P1({GetChoiceName(fakeChoiceP1)}) vs P2({GetChoiceName(fakeChoiceP2)}).\nCONFIRM YOUR CHOSE");
            }
        }
        else
        {
            if (caller == connectionP1)
            {
                choiceP1 = choice;
                ShowResult("Player 1 has already chose");
            }
            else if (caller == connectionP2)
            {
                choiceP2 = choice;
                ShowResult("Player 2 has already chose");
            }
            else return;

            if (choiceP1 != -1 && choiceP2 != -1)
            {
                SolveGame();
            }
        }
    }

    private void SolveGame()
    {
        string roundMessage;
        if (choiceP1 == choiceP2)
        {
            roundMessage = "Round Tie!";
        } 
        else if ((choiceP1 == 0 && choiceP2 == 2) || 
                (choiceP1 == 1 && choiceP2 == 0) || 
                (choiceP1 == 2 && choiceP2 == 1))   
        {
            roundMessage = $"Player 1 wins the round!";
            scoreP1++;
        }
        else
        {
            roundMessage = $"Player 2 wins the round!";
            scoreP2++;
        }

        UpdateScoreUI(scoreP1, scoreP2);

        if (scoreP1 >= 3 || scoreP2 >= 3)
        {
            string finalMessage = (scoreP1 >= 3) ? "PLAYER 1 WINS THE MATCH!" : "PLAYER 2 WINS THE MATCH!";
            ShowResult(roundMessage + "\n" + finalMessage);
            
            StartCoroutine(EndMatchRoutine());
        }
        else
        {
            ShowResult(roundMessage);
            ResetRoundState();
        }
    }

    private IEnumerator EndMatchRoutine()
    {
        yield return new WaitForSeconds(3f);
        
        choiceP1 = -1;
        choiceP2 = -1;
        scoreP1 = 0;
        scoreP2 = 0;
        connectionP1 = null;
        connectionP2 = null;

        if (table != null)
        {
            table.ResetTableServer();
        }
    }

    private string GetChoiceName(int choice)
    {
        return choice switch
        {
            0 => "PIEDRA",
            1 => "PAPEL",
            2 => "TIJERA",
            _ => "DESCONOCIDO"
        };
    }

    [ObserversRpc]
    private void ShowResult(string result)
    {
        resultText.text = result;
    }

    [ObserversRpc]
    private void UpdateScoreUI(int s1, int s2)
    {
        if (scoreText != null)
        {
            scoreText.text = $"P1: {s1}  -  P2: {s2}";
        }
    }
}