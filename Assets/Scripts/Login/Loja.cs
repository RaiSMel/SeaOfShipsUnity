using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loja : MonoBehaviour
{
    public TextMeshProUGUI Moedas;
    string[] Perks = JogadorLogado.jogadorLogado.perks.Split(";");
    public GameObject BtnFileira;
    public GameObject BtnDuasVezes;
    public GameObject BtnEscudo;

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
        StartCoroutine(RequestCompraPerks(JogadorLogado.jogadorLogado.ID, ID_Perk));
    }

    public IEnumerator RequestCompraPerks(string ID_Jogador, string ID_Perk)
    {
        WWWForm formIDJogador = new();
        Debug.Log(ID_Jogador);
        formIDJogador.AddField("ID_Jogador", Int32.Parse(ID_Jogador));
        formIDJogador.AddField("ID_Perk", Int32.Parse(ID_Perk));
        WWW www = new WWW("http://localhost/BatalhaNaval/comprarPerks.php", formIDJogador);
        yield return www;
        Array.Resize(ref Perks, Perks.Length + 1);
        Perks[Perks.Length - 1] = ID_Perk;
        JogadorLogado.jogadorLogado.perks = string.Join(";", Perks);
        verificarPerks();
    }


}
