using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public enum battleState {  START, PLAYERTURN, TARGETTING, ENEMYTURN, WON,LOST}
public class battleManager : MonoBehaviour
{
    //Player Characters
    public GameObject[] player;
    public Transform[] playerSpawn;
    List<Character> playerCharacter = new List<Character>();
    private Character tempPlayer;
    private Character currentPlayer;

    //Enemy Character
    public GameObject[] enemy;
    public Transform[] enemySpawn;
    List<Character> enemyCharacter = new List<Character>();
    private Character tempEnemy;
    private Character currentEnemy;


    public int currentCharacter = 0;

    //ENUMS
    public battleState state;
    public specialTypes specials;
    

    // Energy Management
    public int maxEnergy;
    public int team1Energy;
    public int team2Energy;
    

    //Camera
    [SerializeField] public Camera mainCamera;
    [SerializeField] public Transform ogCamPos;
    [SerializeField] public Transform targetCamOffset;
    [SerializeField] public Transform playerCamOffset;


    //UI Elements
    [SerializeField] public Image playerUI;
    [SerializeField] public Image targettingUI;
    [SerializeField] public Slider team1EnergySlider;
    [SerializeField] public Text battleStatusText;

    // ATTACKS
    int target = 0;
    int typeOfAttack = 1;

    //Random
  

/*-----------Start/Setup------------------*/
    void Start()
    {
        state = battleState.START;
        team1Energy = maxEnergy;
        team1EnergySlider.value = maxEnergy;
        team1EnergySlider.maxValue = maxEnergy;
        UpdateSliders();
        StartCoroutine(BattleSetup());
        

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


 /*-----------Player Turn------------------*/

    void PlayerTurn()
    {
        currentPlayer = playerCharacter[currentCharacter];
        target = 0;
        battleStatusText.text = currentPlayer.name.ToString() + "'s Turn";
        
        mainCamera.transform.position = playerSpawn[currentCharacter].transform.position + playerCamOffset.position;
        playerUI.gameObject.SetActive(true);
        targettingUI.gameObject.SetActive(false);
        Debug.Log("Player Turn");
    }

    

    /*----------Buttons----------*/
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
        specials = currentPlayer.GetSpecialTypes();
        state = battleState.TARGETTING;
        Targeting();

    }

    /*----------Targetting----------*/

    void Targeting()
    {
        
        
        if(typeOfAttack == 2 && specials == specialTypes.HEAL)
        {
            StartCoroutine(targettingCamera(playerCharacter[target].transform.position + targetCamOffset.position));
            battleStatusText.text = "Targetting " + playerCharacter[target].name.ToString();
        }
        else
        {
            StartCoroutine(targettingCamera(enemyCharacter[target].transform.position + targetCamOffset.position));
            battleStatusText.text = "Targetting " + enemyCharacter[target].name.ToString();
        }
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
        
        if (typeOfAttack == 2 && specials == specialTypes.HEAL)
        {
            StartCoroutine(targettingCamera(playerCharacter[target].transform.position + targetCamOffset.position));
            battleStatusText.text = "Targetting " + playerCharacter[target].name.ToString();
        }
        else
        {
            StartCoroutine(targettingCamera(enemyCharacter[target].transform.position + targetCamOffset.position));

            battleStatusText.text = "Targetting " + enemyCharacter[target].name.ToString();

        }
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
        if (typeOfAttack == 2 && specials == specialTypes.HEAL)
        {
            StartCoroutine(targettingCamera(playerCharacter[target].transform.position + targetCamOffset.position));
            battleStatusText.text = "Targetting " + playerCharacter[target].name.ToString();
        }
        else
        {
            StartCoroutine(targettingCamera(enemyCharacter[target].transform.position + targetCamOffset.position));
            battleStatusText.text = "Targetting " + enemyCharacter[target].name.ToString();
        }
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
    /*----------Player Attack*----------*/
    IEnumerator PlayerAttack(int attack, int enemy)
    {
        targettingUI.gameObject.SetActive(false);
        bool isdead = false;
        tempPlayer = playerCharacter[currentCharacter];
        tempEnemy = enemyCharacter[enemy];
        switch (attack)
        {
            case 1: isdead = tempEnemy.takeDamage(tempPlayer.damage,tempPlayer.getElementTypeNormal()); battleStatusText.text = "Player attacks " + attack.ToString(); break;
            case 2:
                switch (playerCharacter[currentCharacter].GetSpecialTypes())
                {
                    case specialTypes.DAMAGE:
                        isdead = tempEnemy.takeDamage(tempPlayer.gameObject.GetComponent<Warrior>().specialDmg, tempPlayer.getElementTypeSpecial());
                        battleStatusText.text = tempPlayer.name.ToString() + " Special Attacks " + tempEnemy.name.ToString();
                        break;
                    case specialTypes.DEBUFFDEFENSE:
                        tempEnemy.debuffDefence(tempPlayer.gameObject.GetComponent<Debuffer>().debuffMultiplier);
                        battleStatusText.text = tempPlayer.name.ToString() + " debuff " + tempEnemy.name.ToString();
                        break;
                    case specialTypes.HEAL:
                        
                        playerCharacter[target].heal(tempPlayer.gameObject.GetComponent<Healer>().Healing);
                        battleStatusText.text = tempPlayer.name.ToString() + " heals " + playerCharacter[target].ToString();
                        break;
                }
                break;

        }
        Debug.Log(isdead);
        UpdateSliders();
        yield return new WaitForSeconds(2f);

        if (isdead)
        {
            enemyCharacter[enemy].gameObject.SetActive(false);
            enemyCharacter.RemoveAt(enemy);


        }
        if(enemyCharacter.Count <= 0)
        {
            state = battleState.WON;
            EndBattle();
        }
        
        currentCharacter += 1;
        Debug.Log(currentCharacter);
        if (currentCharacter >= playerCharacter.Count)
        {
            state = battleState.ENEMYTURN;

            currentCharacter = 0;
            StartCoroutine(EnemyTurn());
        }
        else
        {
            state = battleState.PLAYERTURN;
            PlayerTurn();
        }
        
    }

/*---------------------------ENEMY TURN-----------------------------------*/
    IEnumerator EnemyTurn()
    {
        target = UnityEngine.Random.Range(0,playerCharacter.Count);
        battleStatusText.text = enemyCharacter[currentCharacter].name.ToString() + "'s Turn";
        mainCamera.transform.position = enemyCharacter[currentCharacter].transform.position + targetCamOffset.position;
        Debug.Log("EnemyTurn");
        
        yield return new WaitForSeconds(2f);
        mainCamera.transform.position = playerCharacter [target].transform.position + targetCamOffset.position;
        StartCoroutine(EnemyAttack(UnityEngine.Random.Range(1,3), target));
    }

    IEnumerator EnemyAttack(int attack, int player)
    {
        Debug.Log("Attack:" + attack + " Player: " + 1);
        bool isdead = false;
        tempEnemy = enemyCharacter[currentCharacter];
        tempPlayer = playerCharacter[player];
        switch (attack)
        {
            case 1: isdead = tempPlayer.takeDamage(tempEnemy.damage,tempEnemy.getElementTypeNormal()); battleStatusText.text = tempEnemy.name.ToString() + "attacks " + playerCharacter[player].ToString(); break;
            case 2:
                switch (tempEnemy.GetSpecialTypes())
                {
                    case specialTypes.DAMAGE:
                        isdead = tempPlayer.takeDamage(tempEnemy.gameObject.GetComponent<Warrior>().specialDmg, tempEnemy.getElementTypeSpecial());
                        battleStatusText.text = tempEnemy.name.ToString() + " Special Attacks " + tempPlayer.name.ToString();
                        break;
                    case specialTypes.DEBUFFDEFENSE:
                        tempPlayer.debuffDefence(tempEnemy.gameObject.GetComponent<Debuffer>().debuffMultiplier);
                        battleStatusText.text = tempEnemy.name.ToString() + " debuff " + tempPlayer.name.ToString();
                        break;
                    case specialTypes.HEAL:
                        float lowestHealth = 9999;
                        int index = 0;
                        for (int i = 0; i < enemyCharacter.Count; i++)
                        {
                            float healthratio = enemyCharacter[i].health / enemyCharacter[i].maxHealth;
                            if (healthratio < lowestHealth)
                            {

                                lowestHealth = healthratio;
                                Debug.Log(lowestHealth);
                                index = i;
                            }
                        }
                        mainCamera.transform.position = playerSpawn[index].transform.GetChild(0).transform.position;
                        enemyCharacter[index].heal(tempEnemy.gameObject.GetComponent<Healer>().Healing);
                        battleStatusText.text = tempEnemy.name.ToString() + " heals " + enemyCharacter[index].ToString();
                        break;
                }
                break;

        }
        Debug.Log(isdead);
        UpdateSliders();
        yield return new WaitForSeconds(2f);

        if (isdead)
        {
            playerCharacter[player].gameObject.SetActive(false);
            playerCharacter.RemoveAt(player);


        }
        if (enemyCharacter.Count <= 0)
        {
            state = battleState.LOST;
            EndBattle();
        }
        
        


        currentCharacter += 1;
            
        if (currentCharacter >= enemyCharacter.Count)
        {
            state = battleState.PLAYERTURN;

            currentCharacter = 0;
            PlayerTurn();
        }
        else
        {
            state = battleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        
    }







    /*--------------Functions------------*/
    private void UpdateSliders()
    {
        team1EnergySlider.value = team1Energy;
    }

    IEnumerator targettingCamera(Vector3 destination)
    {
        float totalMovementTime = 0.5f;
        float currentMovementTime = 0f;
        Vector3 startPos = mainCamera.transform.position;
        while (currentMovementTime<= 1)
        {
            currentMovementTime += Time.deltaTime/totalMovementTime;
            mainCamera.transform.position = Vector3.Lerp(startPos, destination, Mathf.SmoothStep(0f, 1f, currentMovementTime));
            yield return null;
            
        }
        
       
    }

    void EndBattle()
    {
        battleStatusText.text = "Battle has ended";
        Application.Quit();
    }

    public void removeAllBuffsFromPlayer()
    {
        foreach (Character character in playerCharacter)
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
