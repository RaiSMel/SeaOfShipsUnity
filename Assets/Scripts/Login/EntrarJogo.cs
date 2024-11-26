using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
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
    
 

    public void Visitante()
    {
        JogadorLogado.jogadorLogado.SetValores(JsonUtility.FromJson<Jogador>("{\"status\":\"success\",\"ID\":\"24\",\"usuario\":\"\",\"email\":\"\",\"moeda\":\"500\",\"tipoJogador\":\"Padrao\",\"dataCadastro\":\"2024-11-19\"}"));
        JogadorLogado.jogadorLogado.Usuario = email.text;
        SceneManager.LoadScene(2);
    }

    IEnumerator EntrarNoJogo()
    {

        var BASE_URL = "https://uskyzrghjpxtirnvzgnj.supabase.co/rest/v1/";
        var API_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVza3l6cmdoanB4dGlybnZ6Z25qIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzIwNjIzMDMsImV4cCI6MjA0NzYzODMwM30.YN_2pAFhua-qP1B6IbSGEM1S8_8KwVRWk90xM0zGS7s";

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



        // Adicionando o header Authorization
        string endpoint = BASE_URL + "jogador_stat_summary"; // Exemplo: tabela orders no schema ecommerce

        // Configurar o UnityWebRequest
        UnityWebRequest request = UnityWebRequest.Get(endpoint);

        // Adicionar cabe�alhos
        request.SetRequestHeader("apiKey", API_KEY); // Autentica��o
        request.SetRequestHeader("Accept-Profile", "jogador_stats");
        request.url += $"?email=eq.{email.text}&senha=eq.{senha.text}";
        yield return request.SendWebRequest();

        try
        {
            
            ProcessResponse(request.downloadHandler.text);

        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro ao processar os dados do servidor: " + e.Message);
        }

    }

    IEnumerator PegarPerks(string id_jogador)
    {
        var BASE_URL = "https://uskyzrghjpxtirnvzgnj.supabase.co/rest/v1/";
        var API_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVza3l6cmdoanB4dGlybnZ6Z25qIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzIwNjIzMDMsImV4cCI6MjA0NzYzODMwM30.YN_2pAFhua-qP1B6IbSGEM1S8_8KwVRWk90xM0zGS7s";

        string endpoint = BASE_URL + $"jogadorperks?id_jogador=eq.{id_jogador}";
        UnityWebRequest request = UnityWebRequest.Get(endpoint);
        request.SetRequestHeader("apiKey", API_KEY); // Autentica��o
        request.SetRequestHeader("Accept-Profile", "public");
        yield return request.SendWebRequest();
        ProcessJson(request.downloadHandler.text);
        
    }

    void ProcessJson(string json)
    {
        // Adiciona a chave de "items" ao JSON para compatibilidade com JsonUtility
        string wrappedJson = $"{{\"items\":{json}}}";

        JogadorPerkList perkList = JsonUtility.FromJson<JogadorPerkList>(wrappedJson);

        List<int> idPerks = new List<int>();
        foreach (var perk in perkList.items)
        {
            idPerks.Add(perk.id_perks);
        }

        JogadorLogado.jogadorLogado.perks = string.Join(";", idPerks);
        SceneManager.LoadScene(2);
    }

    public void ProcessResponse(string json)
    {
        // Remover os escapes
        string unescapedJson = Regex.Unescape(json);

        // Remover os colchetes para tratar como array
        if (unescapedJson.StartsWith("[") && unescapedJson.EndsWith("]"))
        {
            unescapedJson = unescapedJson.Substring(1, unescapedJson.Length - 2); // Remove os colchetes
        }

        // Desserializar o JSON (converter para objeto Jogador)
        Jogador jogador = JsonUtility.FromJson<Jogador>(unescapedJson);

        if (jogador != null)
        {
            Debug.Log(jogador);
            JogadorLogado.jogadorLogado.SetValores(jogador);
            
            StartCoroutine(PegarPerks(jogador.id_jogador));

        }

        else
        {
            Debug.LogError("Erro ao desserializar o JSON para Jogador.");
        }
    }

}


[System.Serializable]
public class JogadorPerk
{
    public int id_jogadorperks;
    public int id_jogador;
    public int id_perks;
}

[System.Serializable]
public class JogadorPerkList
{
    public JogadorPerk[] items;
}