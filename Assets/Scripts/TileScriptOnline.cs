using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TileScriptOnline : MonoBehaviour
{
    GameManagerOnline gameManageronline;
    Ray ray;
    RaycastHit hit;

    Color32[] hitColor = new Color32[2];

    void Start()
    {
        gameManageronline = GameObject.Find("GameManagerOnline").GetComponent<GameManagerOnline>();
        hitColor[0] = gameObject.GetComponent<MeshRenderer>().material.color;
        hitColor[1] = gameObject.GetComponent<MeshRenderer>().material.color;
    }

    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0) && hit.collider.gameObject.name == gameObject.name)
            {
                    gameManageronline.TileClicked(hit.collider.gameObject);
            }
            
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Missile"))
        {

            gameManageronline.photonView.RPC("Colisao", RpcTarget.Others);
            gameManageronline.photonView.RPC("LancarBomba", RpcTarget.Others);
            
        }

    }

    public void SetTileColor(int index, Color32 color)
    {
        hitColor[index] = color;
    }

    public void SwitchColors(int colorIndex)
    {
        GetComponent<Renderer>().material.color = hitColor[colorIndex];
    }
}