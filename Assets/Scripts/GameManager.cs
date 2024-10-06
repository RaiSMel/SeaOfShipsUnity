using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Ships")]
    public GameObject[] ships;
    public EnemyScript enemyScript;
    private ShipScript shipScript;
    private List<int[]> enemyShips;
    private int shipIndex = 0;
    public List<TileScript> allTileScripts;
    private bool playerTurn = true;


    [Header("HUD")]
    public Button nextBtn;
    public Button rotateBtn;
    public Button replayBtn;
    public Text topText;
    public Text playerShipText;
    public Text enemyShipText;
    public Button retornarBtn;


    [Header("Objects")]
    public GameObject missilePrefab;
    public GameObject enemyMissilePrefab;
    public GameObject firePrefab;
    public GameObject woodDock;

    private bool setupComplete = false;

    private List<GameObject> playerFires = new List<GameObject>();
    private List<GameObject> enemyFires = new List<GameObject>();
    private List<GameObject> clickedTiles = new List<GameObject>();


    private int enemyShipCount = 5;
    public int playerShipCount = 5;



    // Start is called before the first frame update
    void Start()
    {
        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        nextBtn.onClick.AddListener(NextShipClicked);
        rotateBtn.onClick.AddListener(RotateClicked);
        replayBtn.onClick.AddListener(ReplayClicked);
        enemyShips = enemyScript.PlaceEnemyShips();
        retornarBtn.onClick.AddListener(ReturnToMainMenu);
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
                shipIndex++;
                shipScript = ships[shipIndex].GetComponent<ShipScript>();
                shipScript.FlashColor(Color.yellow);
            }
            else
            {
                rotateBtn.gameObject.SetActive(false);
                nextBtn.gameObject.SetActive(false);
                topText.text = "Selecione onde atirar";
                setupComplete = true;
                for (int i = 0; i < ships.Length; i++) ships[i].SetActive(false);
            }
        }
    }

    public void TileClicked(GameObject tile)
    {
        if (setupComplete && playerTurn)
        {
            if (!clickedTiles.Contains(tile))
            {
                Vector3 tilePos = tile.transform.position;
                tilePos.y += 15;
                playerTurn = false;
                Instantiate(missilePrefab, tilePos, missilePrefab.transform.rotation);

                // Adicione o tile à lista de tiles clicados
                clickedTiles.Add(tile);
            }
        }
        else if (!setupComplete)
        {
            PlaceShip(tile);
            shipScript.SetClickedTile(tile);
        }
    }

    private void PlaceShip(GameObject tile)
    {
        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        shipScript.ClearTileList();
        Vector3 newVec = shipScript.GetOffsetVec(tile.transform.position);
        ships[shipIndex].transform.localPosition = newVec;
    }

    void RotateClicked()
    {
        shipScript.RotateShip();
    }

    public void CheckHit(GameObject tile)
    {
        int tileNum = Int32.Parse(Regex.Match(tile.name, @"\d+").Value);
        int hitCount = 0;
        foreach (int[] tileNumArray in enemyShips)
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
                    enemyShipCount--;
                    enemyShipText.text = enemyShipCount.ToString();
                    topText.text = "Navio Afundado";
                    if (enemyShipCount == 0)
                    {
                        GameOver(1);
                    }
                    enemyFires.Add(Instantiate(firePrefab, tile.transform.position, Quaternion.identity));
                    tile.GetComponent<TileScript>().SetTileColor(1, new Color32(68, 0, 0, 255));
                    tile.GetComponent<TileScript>().SwitchColors(1);
                }
                else
                {
                    topText.text = "Acertou";
                    tile.GetComponent<TileScript>().SetTileColor(1, new Color32(255, 0, 0, 255));
                    tile.GetComponent<TileScript>().SwitchColors(1);
                }
                break;
            }
        }

        if (hitCount == 0)
        {
            tile.GetComponent<TileScript>().SetTileColor(1, new Color32(38, 57, 76, 255));
            tile.GetComponent<TileScript>().SwitchColors(1);
            topText.text = "Errou";
            Invoke("EndPlayerTurn", 1.0f);
        }
        else if (hitCount > 0)
        {
                playerTurn = true;
        }



    }

    public void EnemyHitPlayer(Vector3 tile, int tileNum, GameObject hitObj)
    {
        enemyScript.MissileHit(tileNum);
        tile.y += 0.2f;
        playerFires.Add(Instantiate(firePrefab, tile, Quaternion.identity));
        if (hitObj.GetComponent<ShipScript>().HitCheckSank())
        {
            enemyScript.SunkPlayer();
            playerShipCount--;
            playerShipText.text = playerShipCount.ToString();
            if (playerShipCount == 0)
            {
                GameOver(0);
            }
            
        }
    }

        private void EndPlayerTurn()
        {
            for (int i = 0; i < ships.Length; i++) ships[i].SetActive(true);
            foreach (GameObject fire in playerFires) fire.SetActive(true);
            foreach (GameObject fire in enemyFires) fire.SetActive(false);
            enemyShipText.text = enemyShipCount.ToString();
            topText.text = "Turno do Inimigo";
            enemyScript.NPCTurn();
            ColorAllTiles(0);
            playerTurn = false;
        }

        public void EndEnemyTurn()
        {
             for (int i = 0; i < ships.Length; i++) ships[i].SetActive(false);
             foreach (GameObject fire in playerFires) fire.SetActive(false);
             foreach (GameObject fire in enemyFires) fire.SetActive(true);
             playerShipText.text = playerShipCount.ToString();
             topText.text = "Selecione onde atirar";
             playerTurn = true;
             ColorAllTiles(1);
        }

        private void ColorAllTiles(int colorIndex)
        {
            foreach (TileScript tileScript in allTileScripts)
            {
                tileScript.SwitchColors(colorIndex);
            }
        }

        public void GameOver(int winner)
        {
            if (winner == 1)
            {
               SceneManager.LoadScene("YouWin");
            }
            else
            {
               SceneManager.LoadScene("YouLose");
            }
    }

        void ReturnToMainMenu()
        {
            SceneManager.LoadScene("Menu");

        }

        void ReplayClicked()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

 
}