using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    // Start is called before the first frame update
    
    public int unitLevel;
    public int damage;
    public int baseDamage;
    public int maxHealth;
    public int health;
    public int baseDefense;
    public int defense;
    public Slider healthSlider;

    private void Start()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
        damage = baseDamage;

    }
    public bool takeDamage(int dmg)
    {
        health -= dmg*defense;
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
        if (health >= maxHealth)
        {
            health = maxHealth;
        }
        healthSlider.value = health;
    }

    public void removeBuffsandDebuffs()
    {
        damage = baseDamage;
        defense = baseDefense;
    }
    public void buffDmg(int multiplier)
    {
        damage = multiplier*damage;
    }
   
}
