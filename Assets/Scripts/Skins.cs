using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Skins : MonoBehaviour
{
    public GameObject [] Ships;
    public Mesh padrao;
    public Material padraoMat;
    public Mesh marinha;
    public Material marinhaMat;

    private void Start()
    {
        Ships = GameObject.FindGameObjectsWithTag("Ship");
        if(JogadorLogado.jogadorLogado.TipoJogador == "Padrao")
        {
            foreach (GameObject i in Ships)
            {
                        i.GetComponent<MeshFilter>().mesh = padrao;
                        i.GetComponent<MeshRenderer>().material = padraoMat;
                }
        }
        else{
            foreach (GameObject i in Ships)
            {
                i.GetComponent<MeshFilter>().mesh = marinha;
                i.GetComponent<MeshRenderer>().material = marinhaMat;
            }
        }
        }
        
    

    // Start is called before the first frame update

}
