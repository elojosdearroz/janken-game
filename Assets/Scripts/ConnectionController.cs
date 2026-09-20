using FishNet.Object;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionController : NetworkBehaviour
{
    [Tooltip("Prefab personaje")]
    public GameObject characterPrefab;

    public override void OnStartClient()
    {
        base.OnStartClient();
        
        if (base.IsOwner)
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoad;
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        
        if (base.IsOwner)
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoad;
        }
    }

    private void OnSceneLoad(Scene escena, LoadSceneMode modo)
    {
        if (escena.name == "GameScene") 
        {
            CmdRequestCharacter();
        }
    }

    [ServerRpc]
    private void CmdRequestCharacter()
    {
        GameObject myCharacter = Instantiate(characterPrefab, Vector3.zero, Quaternion.identity);
        base.ServerManager.Spawn(myCharacter, base.Owner);
    }
}