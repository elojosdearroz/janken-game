using FishNet.Demo.Prediction.Rigidbodies;
using FishNet.Managing;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapManager : MonoBehaviour
{
    private static BootstrapManager instance;
    private void Awake() => instance = this;
    [SerializeField] private string menuName = "MenuScene";
    [SerializeField] private NetworkManager _networkManager;
    [SerializeField] private FishySteamworks.FishySteamworks _fishySteamworks;

    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;
    public static ulong CurrentLobbyID;

    public void GoMenu()
    {
        SceneManager.LoadScene(menuName, LoadSceneMode.Additive);
    }
    void Start()
    {

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        if (SteamManager.Initialized)
        {
            GoMenu();
        }
    }

    public static void CreateLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly,2);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        //Debug.Log("Lobby creada:" + callback.m_eResult.ToString());
        if(callback.m_eResult != EResult.k_EResultOK) return;

        CurrentLobbyID = callback.m_ulSteamIDLobby;
        SteamMatchmaking.SetLobbyData(new CSteamID(CurrentLobbyID), "HostAddress", SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(CurrentLobbyID), "name", SteamFriends.GetPersonaName().ToString() + "'s");
        _fishySteamworks.SetClientAddress(SteamUser.GetSteamID().ToString());
        _fishySteamworks.StartConnection(true);
        Debug.Log("Lobby creado correctamente");
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);

    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        CurrentLobbyID = callback.m_ulSteamIDLobby;

        MainMenuManager.LobbyEntered(SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "name"), _networkManager.IsServer);

        _fishySteamworks.SetClientAddress(SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "HostAddress"));
        _fishySteamworks.StartConnection(false);
    }

    public static void JoinByID(CSteamID steamID)
    {
        Debug.Log("En lobby con ID: " + steamID.m_SteamID);
        if (SteamMatchmaking.RequestLobbyData(steamID))
        {
            SteamMatchmaking.JoinLobby(steamID);
        }
        else
        {
            Debug.Log("Fallo al unirse a la lobby con ID: " + steamID.m_SteamID);
        }
    }

    public static void LeaveLobby()
    {
        SteamMatchmaking.LeaveLobby(new CSteamID(CurrentLobbyID));
        CurrentLobbyID = 0;

        instance._fishySteamworks.StopConnection(false);
        if (instance._networkManager.IsServer)
        {
            instance._fishySteamworks.StopConnection(true);
        }
    }
}
