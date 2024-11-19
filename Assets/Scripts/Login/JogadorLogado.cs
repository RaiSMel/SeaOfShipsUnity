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

        ID = jogador.ID;
        Usuario = jogador.usuario;
        Email = jogador.email;
        Moeda = jogador.moeda;
        TipoJogador = jogador.tipoJogador;
        DataCadastro = jogador.dataCadastro;
    }

    public void SetDadosJogador(DadosJogador dadosJogador)
    {
        PartidasParticipadas = dadosJogador.partidasParticipadas;
        PartidasGanhas = dadosJogador.partidasGanhas;
        PartidasPerdidas = dadosJogador.partidasPerdidas;
        BarcosAfundados = dadosJogador.barcosAfundados;
    }

}

[Serializable]
public class Jogador
{
    public string ID;
    public string usuario;
    public string email;
    public int moeda;
    public string tipoJogador;
    public string dataCadastro;
}

[Serializable]
public class DadosJogador
{
    public int partidasParticipadas;
    public int partidasGanhas;
    public int partidasPerdidas;
    public int barcosAfundados;
}

