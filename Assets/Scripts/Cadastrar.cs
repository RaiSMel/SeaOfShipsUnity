using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Cadastro : MonoBehaviour
{
    public TMP_InputField usuario;
    public TMP_InputField email;
    public TMP_InputField senha;
    public TMP_InputField confirmarSenha;
    public String tipoJogador;
    public Button submit;

    public void ChamarRegistrar()
    {
        StartCoroutine(Registrar());
    }
    IEnumerator Registrar()
    {
        if (senha.text == confirmarSenha.text)
        {
            WWWForm formularioCadastro = new();

            formularioCadastro.AddField("Usuario", usuario.text);
            formularioCadastro.AddField("Email", email.text);
            formularioCadastro.AddField("Senha", senha.text);
            formularioCadastro.AddField("TipoJogador", "");

            WWW www = new("http://localhost/BatalhaNaval/cadastrar.php", formularioCadastro);
            yield return www;
            if (www.text == "0")
            {
                Debug.Log("Seu usuário foi cadastrado com sucesso");
                SceneManager.LoadScene(0);
            }
            else if (www.text == "4")
            {
                Debug.Log("E-mail já cadastrado");
            }
            else
            {
                Debug.Log("Erro");
            }

        }
        else
        {
            Debug.Log("Senhas não estão iguais!");
        }

    }

}
