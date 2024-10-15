using System;
using System.Collections;
using System.Text.RegularExpressions;
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

}
