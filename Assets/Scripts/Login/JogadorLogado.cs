using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
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

