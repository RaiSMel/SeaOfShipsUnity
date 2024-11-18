using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class DadosPerfil : MonoBehaviour
{
    public TextMeshProUGUI Vitorias;
    public TextMeshProUGUI Derrotas;
    public TextMeshProUGUI NaviosAfundados;
    public TextMeshProUGUI PartidasJogadas;

    private void Start()
    {
        Vitorias.text = JogadorLogado.jogadorLogado.PartidasGanhas.ToString();
        Derrotas.text = JogadorLogado.jogadorLogado.PartidasPerdidas.ToString();
        NaviosAfundados.text = JogadorLogado.jogadorLogado.BarcosAfundados.ToString();
        PartidasJogadas.text = JogadorLogado.jogadorLogado.PartidasParticipadas.ToString();
    }

}
