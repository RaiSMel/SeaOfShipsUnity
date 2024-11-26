using System;
using System.Collections;
using System.Net;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Cadastro;

public class Cadastro : MonoBehaviour
{
    public TMP_InputField usuario;
    public TMP_InputField email;
    public TMP_InputField senha;
    public TMP_InputField confirmarSenha;
    public String tipoJogador;
    public Button submit;

    [System.Serializable]
    public class Jogador
    {
        public string usuario;
        public string email;
        public string senha; // Lembre-se de enviar a senha já hashada no frontend.
        public string data;
        public string tipojogador;
        public int moeda;
    }
    public bool ValidarEmail(string email)
    {
        string padraoEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, padraoEmail);
    }

    // Valida senha: pelo menos 8 caracteres, uma maiúscula, uma minúscula, um número e um caractere especial
    public bool ValidarSenha(string senha)
    {
        if (senha.Length < 8) return false;
        else return true;
    }

    // Valida nome de usuário: entre 3 e 15 caracteres e apenas letras e números
    public bool ValidarUsuario(string usuario)
    {
        string padraoUsuario = @"^[a-zA-Z0-9]{3,15}$";
        return Regex.IsMatch(usuario, padraoUsuario);
    }

    public void ChamarRegistrar()
    {
        StartCoroutine(Registrar());
    }
    IEnumerator Registrar()
    {

        if (!ValidarEmail(email.text) )
        {
            Debug.Log("Email é inválido!");
        }
        if (!ValidarSenha(senha.text))
        {
            Debug.Log("Senha é inválido!");
        }
        if(!ValidarUsuario(usuario.text))
        {
            Debug.Log("Usuário é inválido!");
        }

        if (!ValidarUsuario(usuario.text) || !ValidarSenha(senha.text) || !ValidarEmail(email.text))
        {
            StopCoroutine(Registrar());
        }
        else
        {

            if (senha.text == confirmarSenha.text)
            {

                var BASE_URL = "https://uskyzrghjpxtirnvzgnj.supabase.co/rest/v1/";
                var API_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVza3l6cmdoanB4dGlybnZ6Z25qIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzIwNjIzMDMsImV4cCI6MjA0NzYzODMwM30.YN_2pAFhua-qP1B6IbSGEM1S8_8KwVRWk90xM0zGS7s";


                Jogador novoJogador = new Jogador
                {
                    usuario = usuario.text,
                    email = email.text,
                    senha = senha.text,
                    data = System.DateTime.Now.ToString("yyyy-MM-dd"),
                    tipojogador = "Pirata",
                    moeda = 500
                };

                string jsonData = JsonUtility.ToJson(novoJogador);
                var endpoint = BASE_URL + "jogador";
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
                    SceneManager.LoadScene(0);
                }
                else
                {
                    Debug.LogError("Erro ao criar jogador: " + request.error);
                }
            }
        }
    }

}
