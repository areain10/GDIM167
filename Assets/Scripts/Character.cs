using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum specialTypes { DAMAGE,DEBUFFDMG,BUFFDMG,DEBUFFDEFENSE,BUFFDEFENSE,HEAL}
public class Character : MonoBehaviour
{
    // Start is called before the first frame update
    public specialTypes specialType;
    public int unitLevel;
    public int baseDamage;
    public int damage;
    public int maxHealth;
    public int health;
    public float baseDefense;
    public float defense;
    public Slider healthSlider;


    
    private void Start()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
        damage = baseDamage;

    }
    
    public specialTypes GetSpecialTypes()
    {
        return specialType;
    }
    public bool takeDamage(int dmg)
    {
        int dmgtakenRounded = Mathf.RoundToInt(dmg * defense);
        health -= dmgtakenRounded;
        DmgPopup.Create(transform.position, dmgtakenRounded,specialTypes.DAMAGE);
        healthSlider.value = health;

        if (health <= 0 )
        {
            return true;
        }
        return false;
    }

    public void heal(int healing)
    {
        health += healing;
        DmgPopup.Create(transform.position, healing, specialTypes.HEAL);
        if (health >= maxHealth)
        {
            health = maxHealth;
            Debug.Log(health);
            healthSlider.value = health;
        }
        healthSlider.value = health;
    }

    public void removeBuffsandDebuffs()
    {
        damage = baseDamage;
        defense = baseDefense;
    }
    public virtual void buffDmg(float multiplier)
    {
        damage = Mathf.RoundToInt(multiplier*damage);
        
    }
    public virtual void debuffDefence(float multiplier)
    {
        defense = multiplier*defense;
    }
   
}
