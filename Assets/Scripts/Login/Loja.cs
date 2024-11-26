using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loja : MonoBehaviour
{
    public TextMeshProUGUI Moedas;
    string[] Perks = JogadorLogado.jogadorLogado.perks.Split(";");
    public GameObject BtnFileira;
    public GameObject BtnDuasVezes;
    public GameObject BtnEscudo;

    [System.Serializable]
    class perkJogadores
    {
        public string id_jogador;
        public string id_perks;
    }

    [System.Serializable]
    class MoedasCompra
    {
        public int moeda;
    };
    private void Start()
    {
        AtualizarMoedas();
        verificarPerks();
    }
    public void AtualizarMoedas()
    {
        Moedas.text = JogadorLogado.jogadorLogado.Moeda.ToString();
    }
    public void ComprarMoeda(int QtdMoeda)
    {
        JogadorLogado.jogadorLogado.Moeda = JogadorLogado.jogadorLogado.Moeda + QtdMoeda;
        if (!JogadorLogado.jogadorLogado.visitante)
        {
            StartCoroutine(ComprarMoedas());
        }
        AtualizarMoedas();
    }

    public void verificarPerks()
    {
        foreach  (string Perk in Perks)
        {
            if(Perk == "1")
            {
                BtnDuasVezes.GetComponent<Button>().interactable = false;
            }
            else if(Perk == "2")
            {
                BtnFileira.GetComponent<Button>().interactable = false;
            }
            else if(Perk == "3")
            {
                BtnEscudo.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void ComprarPerk( string ID_Perk)
    {
        if (JogadorLogado.jogadorLogado.perks == "")
        {
            JogadorLogado.jogadorLogado.perks = ID_Perk;
        }
        else
        {
            Array.Resize(ref Perks, Perks.Length + 1);
            Perks[Perks.Length - 1] = ID_Perk;
            JogadorLogado.jogadorLogado.perks = string.Join(";", Perks);
            verificarPerks();
        }
        if (!JogadorLogado.jogadorLogado.visitante)
        {
            StartCoroutine(enviarCompra(ID_Perk));
        }
    }
    IEnumerator ComprarMoedas()
    {
        var BASE_URL = "https://uskyzrghjpxtirnvzgnj.supabase.co/rest/v1/";
        var API_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVza3l6cmdoanB4dGlybnZ6Z25qIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzIwNjIzMDMsImV4cCI6MjA0NzYzODMwM30.YN_2pAFhua-qP1B6IbSGEM1S8_8KwVRWk90xM0zGS7s";


        MoedasCompra JogadorPerk = new MoedasCompra
        {
            moeda = JogadorLogado.jogadorLogado.Moeda
        };

        string jsonData = JsonUtility.ToJson(JogadorPerk);
        var endpoint = BASE_URL + $"jogador?id_jogador=eq.{JogadorLogado.jogadorLogado.ID}";
        UnityWebRequest request = new UnityWebRequest(endpoint, "PATCH");
        request.SetRequestHeader("apiKey", API_KEY); // Autentica��o
        request.SetRequestHeader("Accept-Profile", "public");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Envia a requisição
        yield return request.SendWebRequest();

        // Processa a resposta
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Jogador criado com sucesso! Resposta: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Erro ao criar jogador: " + request.error);
        }
    }
    IEnumerator enviarCompra(string ID_Perk) 
    { 

        var BASE_URL = "https://uskyzrghjpxtirnvzgnj.supabase.co/rest/v1/";
        var API_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVza3l6cmdoanB4dGlybnZ6Z25qIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzIwNjIzMDMsImV4cCI6MjA0NzYzODMwM30.YN_2pAFhua-qP1B6IbSGEM1S8_8KwVRWk90xM0zGS7s";


        perkJogadores JogadorPerk = new perkJogadores
        {
            id_jogador = JogadorLogado.jogadorLogado.ID,
            id_perks = ID_Perk
        };

        string jsonData = JsonUtility.ToJson(JogadorPerk);
        var endpoint = BASE_URL + "jogadorperks";
        UnityWebRequest request = new UnityWebRequest(endpoint, "POST");
        request.SetRequestHeader("apiKey", API_KEY); // Autentica��o
        request.SetRequestHeader("Accept-Profile", "public");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Envia a requisição
        yield return request.SendWebRequest();

        // Processa a resposta
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Jogador criado com sucesso! Resposta: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Erro ao criar jogador: " + request.error);
        }
      }
}
