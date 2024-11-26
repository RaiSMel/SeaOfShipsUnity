using System;
using UnityEngine;


public class JogadorLogado : MonoBehaviour
{
    public static JogadorLogado jogadorLogado;
    public string ID;
    public string Usuario;
    public string Email;
    public int Moeda;
    public string TipoJogador;
    public string DataCadastro;
    public int PartidasParticipadas;
    public int PartidasGanhas;
    public int PartidasPerdidas;
    public int BarcosAfundados;
    public string perks;
    public bool visitante = true;

    private void Awake()
    {
        if (jogadorLogado == null)
        {
            jogadorLogado = this;
        }
        DontDestroyOnLoad(this.gameObject);

    }
    public void SetValores(Jogador jogador)
    {

        ID = jogador.id_jogador;
        Usuario = jogador.usuario;
        Email = jogador.email;
        Moeda = jogador.moeda;
        TipoJogador = jogador.tipojogador;
        DataCadastro = jogador.data;
        PartidasParticipadas = jogador.partidas_jogadas;
        PartidasGanhas = jogador.vitorias;
        PartidasPerdidas = jogador.derrotas;
        BarcosAfundados = jogador.barcos_afundados;
    }
}

[Serializable]
public class Jogador
{
    public string id_jogador;
    public string usuario;
    public string email;
    public int moeda;
    public string tipojogador;
    public string data;
    public int partidas_jogadas;
    public int vitorias;
    public int derrotas;
    public int barcos_afundados;
}


