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

        WWW www = new WWW("http://localhost/BatalhaNaval/entrar.php", formularioEntrar);
        yield return www;

        string respostaServidor = www.text;
        Debug.Log("Resposta do Servidor: " + respostaServidor);

        if (respostaServidor == "-1")
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


                    Debug.Log("Usuário logado: " + jogadorData.usuario);
                    SceneManager.LoadScene(2);
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



