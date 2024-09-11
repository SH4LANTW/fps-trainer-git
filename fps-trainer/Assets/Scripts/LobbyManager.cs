using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using WebSocketSharp.Server;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; } //Singleton

    [Header("LobbyCreating Attributes")]
    [SerializeField] private TMP_InputField createLobbyNameInput;
    [SerializeField] private TMP_InputField createLobbyMaxPlayerInput;
    public string joinedLobbyId;

    [Header("LobbyScreens")]
    [SerializeField] private GameObject lobbyListParent;
    [SerializeField] private GameObject lobbyCreationParent;
    [SerializeField] private GameObject profileNameParent;
    [SerializeField] private GameObject joinedLobbyParent;

    [Header("LobbyList Attributes")]
    [SerializeField] private Transform lobbyContentParent;
    [SerializeField] private Transform lobbyItemPrefab;

    [Header("Profile Setup")]
    [SerializeField] private TMP_InputField profileNameInput;
    private string playerName;
    private Player playerData;

    [Header("Joined Lobby")]
    [SerializeField] private Transform playerItemPrefab;
    [SerializeField] private Transform playerContentParent;
    [SerializeField] private TextMeshProUGUI joinedLobbyName;
    [SerializeField] private GameObject lobbyStartButton;

    [Header("RelayManager")]
    [SerializeField] private RelayManager relayManager;

    private async void Start()
    {
        Instance = this; //Singleton

        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public void CreateProfile()
    {
        playerName = profileNameInput.text;

        //custom playerdataobject, input playerName
        PlayerDataObject playerDataObjectName = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName);
        //local playerData, id = authenticated playerId, data = custom PlayerDataObject "Name"
        playerData = new Player(id: AuthenticationService.Instance.PlayerId,
                                data: new Dictionary<string, PlayerDataObject> { { "Name", playerDataObjectName } });

        profileNameParent.SetActive(false);
        lobbyListParent.SetActive(true);
        ShowLobbies();
    }
    private async void ShowLobbies() //Update Lobbies
    {
        while (Application.isPlaying && !lobbyCreationParent.activeInHierarchy && !joinedLobbyParent.activeInHierarchy)
        {
            Debug.LogError("Refreshing Lobbies");
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            foreach (Transform t in lobbyContentParent) 
            {
                Destroy(t.gameObject);
            }

            foreach (Lobby lobby in queryResponse.Results)
            {
                Transform newLobbyItem = Instantiate(lobbyItemPrefab, lobbyContentParent);
                newLobbyItem.GetComponent<LobbyButton>().lobbyId = lobby.Id;                //Send find lobby Id to buttonUI
                newLobbyItem.GetComponent<LobbyButton>().needPassword = lobby.HasPassword; //Send find lobby needPassword buttonUI
                newLobbyItem.GetChild(0).GetComponent<TextMeshProUGUI>().text = lobby.Name;
                newLobbyItem.GetChild(1).GetComponent<TextMeshProUGUI>().text = lobby.Players.Count + "/" + lobby.MaxPlayers;
            }
            await Task.Delay(1000);
        }
    }

    public async void CreateLobby()
    {
        if (!int.TryParse(createLobbyMaxPlayerInput.text, out int maxPlayer)) //check if input maxPlayer is valid
        {
            Debug.LogWarning("Incorrect player count");
            return;
        }

        Lobby createdLobby = null;

        CreateLobbyOptions options = new CreateLobbyOptions();
        options.IsPrivate = false;
        options.Player = playerData;

        DataObject dataObjectJoinCode = new DataObject(DataObject.VisibilityOptions.Public, string.Empty); //new custom dataobject, value = emptystring
        options.Data = new Dictionary<string, DataObject> { { "JoinCode", dataObjectJoinCode } };//set option Custom variable "joinCode" when creating lobby

        try
        {
            createdLobby = await LobbyService.Instance.CreateLobbyAsync(createLobbyNameInput.text, maxPlayer, options);//use custom option start lobby
            joinedLobbyId = createdLobby.Id;
            UpdateLobbyInfo(); //refresh lobby info inside lobby screen
            lobbyCreationParent.SetActive(false);  //turn off lobby create screen
            EnterLobby(); //turn on lobby screen
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
        LobbyHeartbeat(createdLobby);
    }
    private async void LobbyHeartbeat(Lobby lobby)
    {
        while (true)
        {
            if (lobby == null)
            {
                return;
            }
            await LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);

            await Task.Delay(15 * 1000);
        }
    }

    public async void JoinLobby(string lobbyId, bool needPassword)
    {

        try
        {
            await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, new JoinLobbyByIdOptions { Player = playerData });

            joinedLobbyId = lobbyId;

            UpdateLobbyInfo(); //refresh lobby info inside lobby screen
            EnterLobby(); //turn on lobby screen
            lobbyListParent.SetActive(false); //turn off lobbies list screen
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    private bool isJoined = false;

    private async void UpdateLobbyInfo()
    {
        while (Application.isPlaying)
        {
            if (string.IsNullOrEmpty(joinedLobbyId))
            {
                return;
            }

            Lobby lobby = await Lobbies.Instance.GetLobbyAsync(joinedLobbyId);
            joinedLobbyName.text = lobby.Name;

            if (!isJoined && lobby.Data["JoinCode"].Value != string.Empty)  //if host pressed start & lobby updated with new joinCode
            {
                await relayManager.StartClientWithRealy(lobby.Data["JoinCode"].Value); //start client relay with that joinCode
                isJoined = true;
                joinedLobbyParent.SetActive(false);
                return;
            }

            if (AuthenticationService.Instance.PlayerId == lobby.HostId)
            {
                lobbyStartButton.SetActive(true);
            }
            else
            {
                lobbyStartButton.SetActive(false);
            }

            foreach (Transform t in playerContentParent)
            {
                Destroy(t.gameObject);
            }

            foreach (Player player in lobby.Players)
            {
                Transform newPlayerItem = Instantiate(playerItemPrefab, playerContentParent);
                newPlayerItem.GetChild(0).GetComponent<TextMeshProUGUI>().text = player.Data["Name"].Value;
                newPlayerItem.GetChild(1).GetComponent<TextMeshProUGUI>().text = (lobby.HostId == player.Id) ? "Host" : "Guest";
            }

            await Task.Delay(1000);
        }
    }
    public async void LobbyStart()
    {
        Lobby lobby = await Lobbies.Instance.GetLobbyAsync(joinedLobbyId); //get lobby set Maxplayer
        string JoinCode = await relayManager.StartHostWithRelay(lobby.MaxPlayers); //start Host relay, pass in maxplayer, return JoinCode
        isJoined = true; //host started
        await Lobbies.Instance.UpdateLobbyAsync(joinedLobbyId, new UpdateLobbyOptions //send joinCode to lobby custom data "JoinCode"
        { Data = new Dictionary<string, DataObject> { { "JoinCode", new DataObject(DataObject.VisibilityOptions.Public, JoinCode) } } });

        lobbyListParent.SetActive(false); //turn off all UI
        joinedLobbyParent.SetActive(false);
    }
    public void EnterLobbyCreation()
    {
        lobbyCreationParent.SetActive(true);
        lobbyListParent.SetActive(false);
    }
    public void ExitLobbyCreation()
    {
        lobbyCreationParent.SetActive(false);
        lobbyListParent.SetActive(true);
        ShowLobbies();
    }
    public void EnterLobby()
    {
        joinedLobbyParent.SetActive(true);

    }
}
