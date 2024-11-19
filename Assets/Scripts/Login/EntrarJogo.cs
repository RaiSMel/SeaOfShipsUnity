using System;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntrarJogo : MonoBehaviour
{
    public TMP_Text emailStatus;
    public TMP_Text senhaStatus;
    public TMP_Text loginInvalido;
    bool verificarSenha = true, verificarEmail = true;
    public GameObject logarStatus;
    public TMP_InputField email;
    public TMP_InputField senha;

    public JogadorLogado _jogador;

    public EntrarJogo(JogadorLogado jogador)
    {
        _jogador = jogador;
    }

    public bool ValidarEmail(string email)
    {
        string padraoEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, padraoEmail);
    }

    public bool ValidarSenha(string senha)
    {
        return senha.Length >= 8;
    }

    public void ChamarEntrar()
    {
        StartCoroutine(EntrarNoJogo());
    }

    public void FecharPainelLoginStatus()
    {
        LoginPaineis.FecharLoginStatus(logarStatus);
    }
    
    public void PegarDadosPartida(string ID_Jogador)
    {
        StartCoroutine(pegarDados(ID_Jogador));
    }
    
    public IEnumerator pegarDados(string ID_Jogador)
    {
        WWWForm formIDJogador = new();
        Debug.Log(ID_Jogador);
        formIDJogador.AddField("ID_Jogador", Int32.Parse(ID_Jogador));
        WWW www = new WWW("http://seaships.infinityfreeapp.com/dadosPerfil.php", formIDJogador);
        yield return www;
        string respostaServidor = www.text;
        Debug.Log(respostaServidor);
        Debug.Log(respostaServidor);
        DadosJogador dadosJogador = JsonUtility.FromJson<DadosJogador>(respostaServidor);
        JogadorLogado.jogadorLogado.SetDadosJogador(dadosJogador);
        PegarPerks(ID_Jogador);
    }
    public void PegarPerks(string ID_Jogador)
    {
        StartCoroutine(RequestPerks(ID_Jogador));
    }
    public IEnumerator RequestPerks(string ID_Jogador)
    {
        WWWForm formIDJogador = new();
        Debug.Log(ID_Jogador);
        formIDJogador.AddField("ID_Jogador", Int32.Parse(ID_Jogador));
        WWW www = new WWW("http://seaships.infinityfreeapp.com/consultaPerks.php", formIDJogador);
        yield return www;
        JogadorLogado.jogadorLogado.perks = www.text;
        SceneManager.LoadScene(2);
    }

    public void Visitante()
    {
        JogadorLogado.jogadorLogado.SetValores(JsonUtility.FromJson<Jogador>("{\"status\":\"success\",\"ID\":\"24\",\"usuario\":\"\",\"email\":\"\",\"moeda\":\"500\",\"tipoJogador\":\"Padrao\",\"dataCadastro\":\"2024-11-19\"}"));
        JogadorLogado.jogadorLogado.Usuario = email.text;
        SceneManager.LoadScene(2);
    }

    IEnumerator EntrarNoJogo()
    {
        if (!ValidarEmail(email.text))
        {
            verificarEmail = false;
        }
        else verificarEmail = true;

        if (!ValidarSenha(senha.text))
        {
            verificarSenha = false;
        }
        else verificarSenha = true;

        if (!verificarEmail || !verificarSenha)
        {
            LoginPaineis.AbrirLoginStatus(senhaStatus, verificarSenha, emailStatus, verificarEmail, logarStatus, loginInvalido);
            yield break; // Sai da coroutine sem continuar.
        }

        WWWForm formularioEntrar = new WWWForm();
        formularioEntrar.AddField("Email", email.text);
        formularioEntrar.AddField("Senha", senha.text);

        WWW www = new WWW("http://seaships.infinityfreeapp.com/entrar.php", formularioEntrar);
        yield return www;
        string respostaServidor = www.text;

        if (respostaServidor.Contains("error"))
        {
            LoginPaineis.AbrirLoginStatus(senhaStatus, true, emailStatus, true, logarStatus, loginInvalido);
        }
        else
        {
            try
            {
                Jogador jogadorData = JsonUtility.FromJson<Jogador>(respostaServidor);

                if (jogadorData != null)
                {
                    JogadorLogado.jogadorLogado.SetValores(jogadorData);
                    PegarDadosPartida(jogadorData.ID);
                     
                    Debug.Log("Usuário logado: " + jogadorData.ID);
                }
                else
                {
                    Debug.LogError("Falha ao processar os dados do jogador.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Erro ao processar os dados do servidor: " + e.Message);
            }
        }
    }
}



