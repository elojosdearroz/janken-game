using FishNet.Connection;
using FishNet.Object;
using TMPro;

public class JankenManager : NetworkBehaviour
{
    public TextMeshProUGUI resultText;
    private int choiceP1 = -1;
    private int choiceP2 = -1;
    private NetworkConnection connectionP1;
    private NetworkConnection connectionP2;

    public void ConfigurePlayers(NetworkConnection p1, NetworkConnection p2)
    {
        connectionP1 = p1;
        connectionP2 = p2;
    }
    public void BotonPiedra() => SubmitPlay(0);
    public void BotonPapel()  => SubmitPlay(1);
    public void BotonTijera() => SubmitPlay(2);

    [ServerRpc(RequireOwnership = false)]
    public void SubmitPlay(int choice, NetworkConnection caller = null)
    {
        if (caller == connectionP1)
        {
            choiceP1 = choice;
            ShowResult("Jugador 1 eligió. Esperando al rival...");
        }
        else if (caller == connectionP2)
        {
            choiceP2 = choice;
            ShowResult("Jugador 2 eligió. Esperando al rival...");
        }
        else return;

        if (choiceP1 != -1 && choiceP2 != -1)
        {
            SolveGame();
        }
    }

    private void SolveGame()
    {
        if (choiceP1 == choiceP2)
        {
            ShowResult("ES UN EMPATE");
        } 
        else if ((choiceP1 == 0 && choiceP2 == 2) || 
                (choiceP1 == 1 && choiceP2 == 0) || 
                (choiceP1 == 2 && choiceP2 == 1))   
        {
            ShowResult("¡GANÓ EL JUGADOR 1!");
        }
        else
        {
            ShowResult("¡GANÓ EL JUGADOR 2!");
        }

        choiceP1 = -1;
        choiceP2 = -1;
    }

    [ObserversRpc]
    private void ShowResult(string result)
    {
        resultText.text = result;
    }
}