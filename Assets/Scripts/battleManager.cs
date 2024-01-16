using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum battleState {  START, PLAYERTURN, TARGETTING, ENEMYTURN, WON,LOST}
public class battleManager : MonoBehaviour
{
    public GameObject[] player;
    public GameObject[] enemy;

    public battleState state;
    public specialTypes specials;
    public Transform[] playerSpawn;
    public Transform[] enemySpawn;
    List<Character> enemyCharacter = new List<Character>();
    /*Character[] playerCharacter;*/
    List<Character> playerCharacter = new List<Character>();
    public int currentCharacter = 0;
    // Start is called before the first frame update
    public int maxEnergy;
    public int team1Energy;
    public int team2Energy;
    [SerializeField] public Slider team1EnergySlider;

    [SerializeField] public Text battleStatusText;

    [SerializeField] public Camera mainCamera;
    [SerializeField] public Transform ogCamPos;
    [SerializeField] public Transform targetCamOffset;

    [SerializeField] public Image playerUI;
    [SerializeField] public Image targettingUI;

    int target = 0;
    int typeOfAttack = 1;



    void Start()
    {
        state = battleState.START;
        team1Energy = maxEnergy;
        team1EnergySlider.value = maxEnergy;
        team1EnergySlider.maxValue = maxEnergy;
        UpdateSliders();
        StartCoroutine(BattleSetup());

    }
    private void UpdateSliders()
    {
        team1EnergySlider.value = team1Energy;
    }
    IEnumerator BattleSetup()
    {
        battleStatusText.text = "Setting up Battle";
        for (int i = 0; i < player.Length; i++)
        {
            GameObject playerGO = Instantiate(player[i], playerSpawn[i]);
            playerCharacter.Add(playerGO.GetComponent<Character>());
        }
        for (int i = 0; i < enemy.Length; i++)
        {
            GameObject enemyGO = Instantiate(enemy[i], enemySpawn[i]);
            enemyCharacter.Add(enemyGO.GetComponent<Character>());
        }

        yield return new WaitForSeconds(2f);

        state = battleState.PLAYERTURN;
                PlayerTurn();

        
    }

    void targettingCamera()
    {
        mainCamera.transform.position = enemyCharacter[target].transform.position + targetCamOffset.position;
    }
    void PlayerTurn()
    {
        target = 0;
        battleStatusText.text = player[currentCharacter].name.ToString() + "'s Turn";
        mainCamera.transform.position = playerSpawn[currentCharacter].transform.GetChild(0).transform.position;
        playerUI.gameObject.SetActive(true);
        targettingUI.gameObject.SetActive(false);
        Debug.Log("Player Turn");
    }


    public void OnAttackButton()
    {
        if(state != battleState.PLAYERTURN)
        {
            Debug.Log("Not player turn");
            return;
        }
        typeOfAttack = 1;
        state = battleState.TARGETTING;
        Targeting();



    }

    public void OnSpecialButton()
    {
        if (state != battleState.PLAYERTURN | manageEnergy(team1Energy, -1) == -1)
        {
            Debug.Log("Not player turn");
            return;
        }
        typeOfAttack = 2;
        state = battleState.TARGETTING;
        Targeting();

    }

    void Targeting()
    {
        battleStatusText.text = "Targetting " + enemyCharacter[target].name.ToString();
        targettingCamera();
        playerUI.gameObject.SetActive(false);
        targettingUI.gameObject.SetActive(true);
    }
    public void OnNextTargetButton()
    {
        if (state != battleState.TARGETTING)
        {
            return ;
        }
        int temp = target + 1;
        if (temp < enemyCharacter.Count)
        {
            target = temp;
        }
        battleStatusText.text = "Targetting " + enemyCharacter[target].name.ToString();
        targettingCamera();
    }

    public void OnPreviousTargetButton()
    {
        if (state != battleState.TARGETTING)
        {
            return;
        }
        int temp = target - 1;
        if (temp >=0)
        {
            target = temp;
        }
        battleStatusText.text = "Targetting " + enemyCharacter[target].name.ToString();
        targettingCamera();
    }
    public void OnConfirmAttackButton()
    {

        switch (typeOfAttack)
        {
            
            case 1:
                if (manageEnergy(team1Energy, 1) > maxEnergy)
                {
                    team1Energy = maxEnergy;
                }
                else
                {
                    team1Energy += 1;
                }
                StartCoroutine(PlayerAttack(1, target));break;

            case 2: 
                team1Energy = manageEnergy(team1Energy, -1); 
          
                StartCoroutine(PlayerAttack(2, target)); 
                break;

        }
    }
    IEnumerator EnemyTurn()
    {
        battleStatusText.text = "Enemy Attacks";
        mainCamera.transform.position = ogCamPos.position;
        Debug.Log("EnemyTurn");
        bool isdead = playerCharacter[0].takeDamage(enemyCharacter[0].damage);
        yield return new WaitForSeconds(2f);

        if (isdead)
        {
            state = battleState.LOST;
            EndBattle();

        }
        state = battleState.PLAYERTURN;
        currentCharacter = 0;
        removeAllBuffsFromPlayer();
        PlayerTurn();
    }

    IEnumerator PlayerAttack(int attack,int enemy)
    {
        targettingUI.gameObject.SetActive(false);
        bool isdead = false;
        
        switch (attack)
        {
            case 1: isdead = enemyCharacter[enemy].takeDamage(playerCharacter[currentCharacter].damage); battleStatusText.text = "Player attacks " + attack.ToString(); break;
                case 2:
                switch (playerCharacter[currentCharacter].GetSpecialTypes())
                {
                    case specialTypes.DAMAGE:
                        isdead = enemyCharacter[enemy].takeDamage(playerCharacter[currentCharacter].gameObject.GetComponent<Warrior>().specialDmg);
                        battleStatusText.text = playerCharacter[currentCharacter].name.ToString() + " Special Attacks " + enemyCharacter[enemy].name.ToString();
                        break;
                    case specialTypes.DEBUFFDEFENSE:
                        enemyCharacter[enemy].debuffDefence(playerCharacter[currentCharacter].gameObject.GetComponent<Debuffer>().debuffMultiplier);
                        battleStatusText.text = playerCharacter[currentCharacter].name.ToString()+" debuff " + enemyCharacter[enemy].name.ToString();
                        break;
                    case specialTypes.HEAL:
                        float lowestHealth = 9999;
                        int index = 0;
                        for (int i = 0;i<playerCharacter.Count;i++)
                        {
                            float healthratio = playerCharacter[i].health / playerCharacter[i].maxHealth;
                            if (healthratio < lowestHealth)
                            {
                                
                                lowestHealth = healthratio;
                                Debug.Log(lowestHealth);
                                index = i;
                            }
                        }
                        mainCamera.transform.position = playerSpawn[index].transform.GetChild(0).transform.position;
                        playerCharacter[index].heal(playerCharacter[currentCharacter].gameObject.GetComponent<Healer>().Healing);
                        battleStatusText.text = playerCharacter[currentCharacter].name.ToString() + " heals " + playerCharacter[index].ToString();
                        break;
                }
                break;
                
        }
        Debug.Log(isdead);
        UpdateSliders();
        yield return new WaitForSeconds(2f);

        if (isdead)
        {
            state = battleState.WON;
            EndBattle();


        }
        else
        {


            currentCharacter += 1;
            Debug.Log(currentCharacter);
            if (currentCharacter >= playerCharacter.Count)
            {
                state = battleState.ENEMYTURN;


                StartCoroutine(EnemyTurn());
            }
            else
            {
                state = battleState.PLAYERTURN;
                PlayerTurn();
            }
        }
    }

    void EndBattle()
    {
        battleStatusText.text = "Battle has ended";
        Application.Quit();
    }

    public void removeAllBuffsFromPlayer()
    {
        foreach(Character character in playerCharacter)
        {
            character.removeBuffsandDebuffs();
        }
    }
    public int manageEnergy(int energy, int energyChange)
    {
        int temp = energy + energyChange;

        if (temp < 0)
        {
            return -1;
        }
        return temp;
    }
}
