using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyStarter : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _criarPartida;
    [SerializeField] private GameObject _entrarPartida;
    [SerializeField] private MenuLobby _menuLobby;
    [SerializeField] private GameObject _menuJogar;
    public static LobbyStarter Instancia { get; private set; }
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
        _criarPartida.gameObject.SetActive(false);
        _entrarPartida.gameObject.SetActive(false);
        _menuLobby.gameObject.SetActive(false);
        menu.SetActive(true);
    }
    public override void OnJoinedRoom()
    {
        MudaMenu(_menuLobby.gameObject);
        //_menuLobby.photonView.RPC("AtualizaLista", RpcTarget.All);
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
}
