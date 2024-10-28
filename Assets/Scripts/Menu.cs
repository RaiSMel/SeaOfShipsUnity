using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class Menu : MonoBehaviourPunCallbacks
{
    [SerializeField] private MenuCriar _menuJogar;
    [SerializeField] private MenuEntrada _menuEntrada;
    [SerializeField] private MenuLobby _menuLobby;
    [SerializeField] private MenuCriar _menuCriar;

    public override void OnConnectedToMaster()
    {
    }
    public override void OnJoinedRoom()
    {
        MudaMenu(_menuLobby.gameObject);
        _menuLobby.photonView.RPC("AtualizaLista", RpcTarget.All);
    }

    public void MudaMenu(GameObject menu)
    {

        _menuCriar.gameObject.SetActive(false);
        _menuEntrada.gameObject.SetActive(false);
        _menuLobby.gameObject.SetActive(false);

        menu.SetActive(true);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _menuLobby.AtualizaLista();
    }

    public void SairDoLobby()
    {
        GestorDeRede.Instancia.SairDoLobby();
        MudaMenu(_menuJogar.gameObject);
    }

    public void ComecaJogo(string nomeCena)
    {
        GestorDeRede.Instancia.photonView.RPC("ComecaJogo", RpcTarget.All, nomeCena);

    }
}
