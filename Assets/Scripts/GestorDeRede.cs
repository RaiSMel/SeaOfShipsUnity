using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class GestorDeRede : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject roomButtonPrefab; // Prefab do botão de sala
    [SerializeField] private Transform roomListContainer; // Container para a lista de salas
    [SerializeField] private Button joinRoomButton; // Botão para entrar na sala
    private Dictionary<string, GameObject> roomButtons = new Dictionary<string, GameObject>();
    private string selectedRoomName; // Nome da sala selecionada


    public const int MAXPLAYERS = 2;
    public static GestorDeRede Instancia { get; private set; }

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            gameObject.SetActive(false);
            return;
        }
        Instancia = this;
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(); // Entra no lobby principal ao conectar com sucesso
        Debug.Log("Conectado ao servidor e entrando no lobby.");
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

    public override void OnJoinedLobby()
    {
        PhotonNetwork.NickName = JogadorLogado.jogadorLogado.Usuario;
        Debug.Log("Entrou no lobby.");
    }

    public void CriaSala(string nomeSala)
    {
        PhotonNetwork.CreateRoom(nomeSala);
    }

    public void EntraSala(string nomeSala)
    {
        PhotonNetwork.JoinRoom(nomeSala);
    }


    public void MudaNick(string nickname)
    {
        PhotonNetwork.NickName = nickname;
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
    public void SairDoLobby()
    {
        PhotonNetwork.LeaveRoom();
    }

    [PunRPC]
    public void ComecaJogo(string nomeCena)
    {
        PhotonNetwork.LoadLevel(nomeCena);
    }

}
