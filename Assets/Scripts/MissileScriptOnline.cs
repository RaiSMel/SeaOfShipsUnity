using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MissileScriptOnline : MonoBehaviourPunCallbacks
{
    private GameManagerOnline gameManageronline;
    public GameObject targetTileLocation;
    void Start()
    {
        gameManageronline = GameObject.Find("GameManagerOnline").GetComponent<GameManagerOnline>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 hitPosition = collision.contacts[0].point; // Obtém a posição da colisão

        if (collision.gameObject.CompareTag("Ship"))
        {
            Destroy(gameObject);
            gameManageronline.photonView.RPC("Colisao", RpcTarget.Others);
            gameManageronline.photonView.RPC("LancarBomba", RpcTarget.Others);
            gameManageronline.CheckHit(collision.gameObject, hitPosition);

        }
        else
        {
            Destroy(gameObject);
            gameManageronline.photonView.RPC("Colisao", RpcTarget.Others);
            gameManageronline.photonView.RPC("LancarBomba", RpcTarget.Others);
            gameManageronline.CheckHit(collision.gameObject, hitPosition);
        }
    }

}
