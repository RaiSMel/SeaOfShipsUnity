using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Multiplayer : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject roomButtonPrefab; // Prefab do botão de sala
    [SerializeField] private Transform roomListContainer; // Container para a lista de salas
    [SerializeField] private Button joinRoomButton; // Botão para entrar na sala

    private Dictionary<string, GameObject> roomButtons = new Dictionary<string, GameObject>();
    private string selectedRoomName; // Nome da sala selecionada
    // Start is called before the first frame update
    [SerializeField] private GameObject _criarPartida;
    [SerializeField] private GameObject _entrarPartida;
    [SerializeField] private MenuLobby _menuLobby;
    [SerializeField] private GameObject _menuJogar;
    public static Multiplayer Instancia { get; private set; }
    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            gameObject.SetActive(false);
            return;
        }
        Instancia = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {

        PhotonNetwork.ConnectUsingSettings(); // Conecta ao Photon Cloud
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(); // Entra no lobby principal ao conectar com sucesso
        Debug.Log("Conectado ao servidor e entrando no lobby.");
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.NickName = "Rai";
        Debug.Log("Entrou no lobby.");
    }
    
    public void MudaMenu(GameObject menu)
    {
        _criarPartida.SetActive(false);
        _entrarPartida.SetActive(false);
        _menuLobby.gameObject.SetActive(false);
        menu.SetActive(true);
    }
    public override void OnJoinedRoom()
    {
        MudaMenu(_menuLobby.gameObject);
        _menuLobby.photonView.RPC("AtualizaLista", RpcTarget.All);
    }

    public string ObterListaDeJogadores()
    {
        var lista = "";
        foreach (var player in PhotonNetwork.PlayerList)
        {
            lista += player.NickName + "\n";
        }
        return lista;
    }

    public bool DonoDaSala()
    {
        return PhotonNetwork.IsMasterClient;
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // Limpa a lista para que seja atualizada com as salas disponíveis atuais
        foreach (var roomButton in roomButtons.Values)
        {
            Destroy(roomButton);
        }
        roomButtons.Clear();

        // Para cada sala disponível, cria um botão na interface
        foreach (RoomInfo roomInfo in roomList)
        {
            if (roomInfo.RemovedFromList)
            {
                // Remove a sala da lista se não for mais exibida
                if (roomButtons.ContainsKey(roomInfo.Name))
                {
                    Destroy(roomButtons[roomInfo.Name]);
                    roomButtons.Remove(roomInfo.Name);
                }
                continue;
            }

            // Cria o botão para a sala e define o nome
            GameObject newRoomButton = Instantiate(roomButtonPrefab, roomListContainer);
            newRoomButton.GetComponentInChildren<TextMeshProUGUI>().text = roomInfo.Name;

            // Salva a referência para poder excluir ou atualizar se necessário
            roomButtons[roomInfo.Name] = newRoomButton;

            // Adiciona uma função de seleção para o botão
            newRoomButton.GetComponent<Button>().onClick.AddListener(() => SelectRoom(roomInfo.Name));
        }
    }

    private void SelectRoom(string roomName)
    {
        selectedRoomName = roomName;
        Debug.Log("Sala selecionada: " + selectedRoomName);
    }
    public void JoinSelectedRoom()
    {

        if (!string.IsNullOrEmpty(selectedRoomName))
        {
            Debug.Log(selectedRoomName);
            PhotonNetwork.JoinRoom(selectedRoomName);
        }
    }
    public void CriarLobby()
    {
        RoomOptions salaOptions = new RoomOptions();
        salaOptions.MaxPlayers = 2;
        salaOptions.IsVisible = true;
        salaOptions.IsOpen = true;
        PhotonNetwork.CreateRoom("Room", salaOptions);
    }
    public void LeftRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    [PunRPC]
    public void ComecaJogo(string cena)
    {
        PhotonNetwork.LoadLevel(cena);
    }
    public void ChamaJogo(string cena)
    {
        photonView.RPC("ComecaJogo", RpcTarget.All, cena);
    }
    //CODIGO NOSSO
}