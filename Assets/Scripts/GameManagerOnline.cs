using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class GameManagerOnline : MonoBehaviourPunCallbacks
{
    [Header("Ships")]
    public GameObject[] ships;
    private ShipScriptOnline shipScript;
    private List<int[]> playerShips = new List<int[]>();
    private List<int[]> ServerShips = new List<int[]>();
    private List<int[]> InputShips = new List<int[]>();
    private int shipIndex = 0;
    public List<TileScriptOnline> allTileScripts;


    [Header("HUD")]
    public Button nextBtn;
    public Button rotateBtn;
    public Text topText;
    public Text NextTextBtn;
    public Text playerShipText;
    public Text turnTimerText;


    [Header("Objects")]
    public GameObject missilePrefab;
    public GameObject firePrefab;
    public GameObject woodDock;

    private bool setupComplete = false;
    private bool clickable = true;
    private bool turn = false;

    private List<GameObject> playerFires = new List<GameObject>();
    private List<GameObject> clickedTiles = new List<GameObject>();
    private List<GameObject> clickedTiles2 = new List<GameObject>();


    public int playerShipCount = 5;

    private int _jogadoresEmJogo = 0;
    private Player _photonPlayer;
    private int _id;
    private int jogadorId = PhotonNetwork.LocalPlayer.ActorNumber;
    private List<GameManagerOnline> _jogadores;
    private bool playersReady = false;

    private int currentPlayerTurn = 0;

    private bool _jogadorInicializado = false;

    private bool bombaLancada = false;
    private bool colisaoBomba = false;

    private bool goagain = false;

    [SerializeField] private string _localizacaoPrefab;

    public List<GameManagerOnline> Jogadores { get => _jogadores; set => _jogadores = value; }

    public static GameManagerOnline Instancia { get; set; }

    private float turnTime = 15f;
    private bool timerActive = false;

    private GameAudioManager audioManager;


    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            gameObject.SetActive(false);
            return;
        }
        Instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        turnTimerText.gameObject.SetActive(false);
        photonView.RPC("AdicionaJogador", RpcTarget.AllBuffered);
        _jogadores = new List<GameManagerOnline>();
        shipScript = ships[shipIndex].GetComponent<ShipScriptOnline>();
        nextBtn.onClick.AddListener(NextShipClicked);
        rotateBtn.onClick.AddListener(RotateClicked);
        audioManager = FindObjectOfType<GameAudioManager>();
    }

    void Update()
    {
        if (timerActive)
        {
            UpdateTurnTimerText();

        }
    }


    [PunRPC]
    private IEnumerator StartTurnTimer()
    {
        // O corpo do método permanece o mesmo
        while (timerActive)
        {
            yield return new WaitForSeconds(1f);
            turnTime -= 1f;
            if (turnTime <= 0)
            {
                if (currentPlayerTurn == jogadorId - 1)
                {
                    // Tempo esgotado, mude o turno automaticamente
                    timerActive = false;
                    photonView.RPC("Colisao", RpcTarget.All);
                    photonView.RPC("LancarBomba", RpcTarget.All);
                    photonView.RPC("AlterTurn", RpcTarget.All);
                    photonView.RPC("SwitchTurn", RpcTarget.All, jogadorId);
                }

            }
        }
    }

    [PunRPC]
    private void AlterTurn()
    {
        currentPlayerTurn = (currentPlayerTurn + 1) % Jogadores.Count;
    }

    private void UpdateTurnTimerText()
    {
        // Atualize o texto na tela com o tempo restante
        turnTimerText.text = Mathf.CeilToInt(turnTime).ToString();
    }

    [PunRPC]
    private void SwitchTurn(int id)
    {
        if (!AmbosJogadoresProntos())
        {
            topText.text = "Aguardando";
            photonView.RPC("Colisao", RpcTarget.All);
            photonView.RPC("LancarBomba", RpcTarget.All);
        }
        else
        {
            if (goagain)
            {
                turnTimerText.gameObject.SetActive(true);
                timerActive = true;
                turnTime = 15f;
                photonView.RPC("StartTurnTimer", RpcTarget.All);
                photonView.RPC("Colisaofalso", RpcTarget.All);
                photonView.RPC("LancarBombafalso", RpcTarget.All);
                turn = true;
                photonView.RPC("SetShips", RpcTarget.All, ServerShips.ToArray());
                for (int i = 0; i < ships.Length; i++) ships[i].SetActive(false);
                foreach (GameObject fire in playerFires) fire.SetActive(false);
                topText.text = "Selecione onde atirar";
            }
            else
            {
                bombaLancada = false;
                colisaoBomba = false;
                if (currentPlayerTurn == jogadorId - 1)
                {
                    turnTimerText.gameObject.SetActive(true);
                    timerActive = true;
                    turnTime = 15f;
                    photonView.RPC("StartTurnTimer", RpcTarget.All);
                    turn = true;
                    photonView.RPC("SetShips", RpcTarget.All, ServerShips.ToArray());
                    for (int i = 0; i < ships.Length; i++) ships[i].SetActive(false);
                    foreach (GameObject fire in playerFires) fire.SetActive(false);
                    topText.text = "Selecione onde atirar";
                    ColorTilesByPlayer(currentPlayerTurn);

                }
                else
                {
                    turnTime = 15f;
                    timerActive = false;
                    turnTimerText.gameObject.SetActive(false);
                    turn = false;
                    for (int i = 0; i < ships.Length; i++) ships[i].SetActive(true);
                    foreach (GameObject fire in playerFires) fire.SetActive(true);
                    topText.text = "Turno do Inimigo";
                    ColorTilesByPlayer(currentPlayerTurn);
                }
            }
        }

    }



    public void SpawnMissile(Vector3 position)
    {
        photonView.RPC("SpawnMissileRPC", RpcTarget.All, position);

    }

    [PunRPC]
    public void Colisao()
    {
        colisaoBomba = true;
    }

    [PunRPC]
    public void Colisaofalso()
    {
        colisaoBomba = false;
    }


    [PunRPC]
    public void LancarBomba()
    {

        bombaLancada = true;
    }

    [PunRPC]
    public void LancarBombafalso()
    {

        bombaLancada = false;
    }

    [PunRPC]
    private bool AmbosJogadoresProntos()
    {
        return bombaLancada && colisaoBomba;
    }

    [PunRPC]
    private void SpawnMissileRPC(Vector3 position)
    {

        GameObject missile = Instantiate(missilePrefab, position, Quaternion.identity);


    }

    [PunRPC]
    private void AdicionaJogador()
    {
        _jogadoresEmJogo++;
        if (_jogadoresEmJogo == PhotonNetwork.PlayerList.Length)
        {
            if (!_jogadorInicializado)
            {
                CriaJogador();
                _jogadorInicializado = true;
            }

        }
    }


    [PunRPC]
    private void SendShips()
    {
        photonView.RPC("ReceiveOpponentShips", RpcTarget.Others, playerShips.ToArray());
    }

    [PunRPC]
    private void UpadateSetShips()
    {
        photonView.RPC("SetShips", RpcTarget.All, ServerShips.ToArray());
    }


    [PunRPC]
    private void ReceiveOpponentShips(int[][] opponentShipsArray)
    {
        ServerShips.Clear();
        foreach (int[] shipPositions in opponentShipsArray)
        {
            ServerShips.Add(shipPositions);
        }
    }

    [PunRPC]
    private void SetShips(int[][] opponentShipsArray)
    {
        InputShips.Clear();
        foreach (int[] shipPositions in opponentShipsArray)
        {
            InputShips.Add(shipPositions);
        }
    }

    [PunRPC]
    private void Inicializa(Player player)
    {
        _photonPlayer = player;
        _id = player.ActorNumber;
        GameManagerOnline.Instancia.Jogadores.Add(this);
        Debug.Log("Jogador " + _id + " inicializado.");
        Debug.Log("Jogador criado. Total de jogadores: " + Jogadores.Count);
    }

    [PunRPC]
    private void CriaJogador()
    {
        photonView.RPC("Inicializa", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    private void JogadorPronto()
    {
        playersReady = true;
    }

    public void AddPlayerShip(int[] shipPositions)
    {
        playerShips.Add(shipPositions);
    }

    private void NextShipClicked()
    {
        if (!shipScript.OnGameBoard())
        {
            shipScript.FlashColor(Color.red);
        }
        else
        {
            if (shipIndex <= ships.Length - 2)
            {
                int[] shipPositions = shipScript.GetTileNumbers();
                AddPlayerShip(shipPositions);
                shipIndex++;
                shipScript = ships[shipIndex].GetComponent<ShipScriptOnline>();
                shipScript.FlashColor(Color.yellow);
            }
            else
            {
                int[] shipPositions = shipScript.GetTileNumbers();
                AddPlayerShip(shipPositions);
                SendShips();
                photonView.RPC("JogadorPronto", RpcTarget.Others);
                clickable = false;
                if (!playersReady)
                {
                    topText.text = "Aguardando Jogador Finalizar";
                    photonView.RPC("Colisao", RpcTarget.Others);
                    photonView.RPC("LancarBomba", RpcTarget.Others);
                    photonView.RPC("JogadorPronto", RpcTarget.Others);
                    rotateBtn.gameObject.SetActive(false);
                    nextBtn.gameObject.SetActive(false);
                    setupComplete = true;

                }
                else
                {
                    rotateBtn.gameObject.SetActive(false);
                    nextBtn.gameObject.SetActive(false);
                    setupComplete = true;
                    clickable = false;
                    photonView.RPC("Colisao", RpcTarget.Others);
                    photonView.RPC("LancarBomba", RpcTarget.Others);
                    photonView.RPC("SwitchTurn", RpcTarget.All, jogadorId);

                }

            }
        }
    }


    public void TileClicked(GameObject tile)
    {
        List<GameObject> currentPlayerClickedTiles = clickedTiles;
        List<GameObject> opponentClickedTiles = clickedTiles2;

        if (setupComplete && turn)
        {
            if (!currentPlayerClickedTiles.Contains(tile) && !opponentClickedTiles.Contains(tile))
            {
                turn = false;
                Vector3 tilePos = tile.transform.position;
                tilePos.y += 15;
                SpawnMissile(tilePos);
                timerActive = false;
                if (jogadorId == 1)
                {
                    clickedTiles.Add(tile);
                }
                else if (jogadorId == 2)
                {
                    clickedTiles2.Add(tile);
                }
            }
        }
        else if (!setupComplete && clickable)
        {
            PlaceShip(tile);
            shipScript.SetClickedTile(tile);
        }
    }

    [PunRPC]
    private void ColorTilesByPlayer(int playerIndex)
    {
        int colorIndex = playerIndex;
        List<GameObject> currentPlayerClickedTiles = (playerIndex == 1) ? clickedTiles : clickedTiles2;

        foreach (TileScriptOnline tileScript in allTileScripts)
        {
            tileScript.SwitchColors(colorIndex);
        }
    }

    private void PlaceShip(GameObject tile)
    {
        shipScript = ships[shipIndex].GetComponent<ShipScriptOnline>();
        shipScript.ClearTileList();
        Vector3 newVec = shipScript.GetOffsetVec(tile.transform.position);
        ships[shipIndex].transform.localPosition = newVec;


    }

    void RotateClicked()
    {
        shipScript.RotateShip();
    }

    public void CheckHit(GameObject tile, Vector3 hitPosition)
    {
        int hitCount = 0;
        if (tile.CompareTag("Ship"))
        {
            hitCount++;
            Vector3 fireOffset = new Vector3(0f, -1f, 0f);
            Vector3 firePosition = hitPosition + fireOffset;
            playerFires.Add(Instantiate(firePrefab, firePosition, Quaternion.identity));
            topText.text = "Acertou";

            List<AudioClip> audiosSelecionados = new List<AudioClip>
            {
                audioManager.podeNaoParecer,
                audioManager.acertoMizeravi,
                audioManager.eNoisVelho

            };
        
            audioManager.PlayRandomAudio(audiosSelecionados);

            if (currentPlayerTurn == jogadorId - 1)
            {
                goagain = true;
            }
            else
            {
                goagain = false;

            }
            photonView.RPC("SwitchTurn", RpcTarget.All, jogadorId);
            if (tile.GetComponent<ShipScriptOnline>().HitCheckSank())
            {
                if (currentPlayerTurn == jogadorId - 1)
                {
                    playerShipCount--;
                    playerShipText.text = playerShipCount.ToString();
                    if (playerShipCount == 0)
                    {
                        photonView.RPC("GameOver", RpcTarget.All);
                    }

                }

            }
        }
        else
        {
            int tileNum = Int32.Parse(Regex.Match(tile.name, @"\d+").Value);
            foreach (int[] tileNumArray in InputShips)
            {
                if (tileNumArray.Contains(tileNum))
                {
                    for (int i = 0; i < tileNumArray.Length; i++)
                    {
                        if (tileNumArray[i] == tileNum)
                        {
                            tileNumArray[i] = -5;
                            hitCount++;
                        }
                        else if (tileNumArray[i] == -5)
                        {
                            hitCount++;
                        }
                    }
                    if (hitCount == tileNumArray.Length)
                    {
                        if (currentPlayerTurn == jogadorId - 1)
                        {
                            playerShipCount--;
                            playerShipText.text = playerShipCount.ToString();
                            topText.text = "Navio Afundado";
                            if (playerShipCount == 0)
                            {
                                photonView.RPC("GameOver", RpcTarget.All);
                            }

                        }
                        tile.GetComponent<TileScriptOnline>().SetTileColor(currentPlayerTurn, new Color32(68, 0, 0, 255));
                        tile.GetComponent<TileScriptOnline>().SwitchColors(currentPlayerTurn);
                        if (currentPlayerTurn == jogadorId - 1)
                        {
                            goagain = true;
                        }
                        else
                        {
                            goagain = false;

                        }
                        photonView.RPC("SwitchTurn", RpcTarget.All, jogadorId);
                    }
                    else
                    {
                        List<AudioClip> audiosSelecionados = new List<AudioClip>
                        {
                            audioManager.podeNaoParecer,
                            audioManager.acertoMizeravi,
                            audioManager.eNoisVelho

                        };

                        audioManager.PlayRandomAudio(audiosSelecionados);
                        topText.text = "Acertou";
                        tile.GetComponent<TileScriptOnline>().SetTileColor(currentPlayerTurn, new Color32(255, 0, 0, 255));
                        tile.GetComponent<TileScriptOnline>().SwitchColors(currentPlayerTurn);
                        if (currentPlayerTurn == jogadorId - 1)
                        {
                            goagain = true;
                        }
                        else
                        {
                            goagain = false;

                        }
                        photonView.RPC("SwitchTurn", RpcTarget.All, jogadorId);
                    }
                    break;
                }
            }

            if (hitCount == 0)
            {
                tile.GetComponent<TileScriptOnline>().SetTileColor(currentPlayerTurn, new Color32(38, 57, 76, 255));
                tile.GetComponent<TileScriptOnline>().SwitchColors(currentPlayerTurn);

                List<AudioClip> audiosSelecionados = new List<AudioClip>
                {
                    audioManager.pobre,
                    audioManager.taTriste,
                    audioManager.gp,
                    audioManager.areYou,
                    audioManager.errou

                };

                audioManager.PlayRandomAudio(audiosSelecionados);
                
                topText.text = "Errou";
                goagain = false;
                currentPlayerTurn = (currentPlayerTurn + 1) % Jogadores.Count;
                photonView.RPC("SwitchTurn", RpcTarget.All, jogadorId);
            }
        }
    }


    private void ColorAllTiles(int colorIndex)
    {
        foreach (TileScriptOnline tileScript in allTileScripts)
        {
            tileScript.SwitchColors(colorIndex);
        }
    }

    [PunRPC]
    public void GameOver()
    {
        if (currentPlayerTurn == jogadorId - 1)
        {
            SceneManager.LoadScene("YouWin");
            audioManager.PlayGanhamos();
        }
        else
        {
            SceneManager.LoadScene("YouLose");
            audioManager.PlayNaoAcredito();

        }
    }

}
