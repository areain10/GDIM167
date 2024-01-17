using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum specialTypes { DAMAGE,DEBUFFDMG,BUFFDMG,DEBUFFDEFENSE,BUFFDEFENSE,HEAL}
public enum elements { ELECTRIC,WATER,FIRE,TOXIC,NANITES,EMP,PHYSICAL}
public class Character : MonoBehaviour
{
    // Start is called before the first frame update
    public string CharacterName;
    public specialTypes specialType;
    public elements elementalType;
    public int baseDamage;
    public int damage;
    public int maxHealth;
    public int health;
    public float baseDefense;
    public float defense;
    public Slider healthSlider;

    float effective = 1.2f;
    float ineffective = 0.8f;

    private void Start()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
        damage = baseDamage;

    }
    
    public string getName()
    {
        return CharacterName;
    }
    public elements getElementTypeSpecial()
    {
        return elementalType;
    }
    public virtual elements getElementTypeNormal()
    {
        return elementalType;
    }
    public specialTypes GetSpecialTypes()
    {
        return specialType;
    }
    public bool takeDamage(int dmg,elements dmgElement)
    {
        int dmgtakenRounded = Mathf.RoundToInt(dmg -(defense * dmgMultiplier(elementalType,dmgElement)));
        
        health -= dmgtakenRounded;
        DmgPopup.Create(transform.position, dmgtakenRounded,specialTypes.DAMAGE);
        healthSlider.value = health;
        switch (dmgMultiplier(elementalType, dmgElement))
        {
            case 1.2f: DmgPopup.Effective(transform.position, "INEFFECTIVE"); break;
            case 0.8f: DmgPopup.Effective(transform.position, "EFFECTIVE"); break;
            default: break;

        }
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
   
    public float dmgMultiplier(elements victimElement,elements instigatorElement)
    {
        
        switch(victimElement)
        {
            case elements.ELECTRIC:
                if(instigatorElement == elements.WATER)
                {
                    return effective;
                }
                else if(instigatorElement == elements.EMP)
                {
                    return ineffective;
                }
                else
                {
                    return 1f;
                }
                
            case elements.WATER:
                if (instigatorElement == elements.FIRE)
                {
                    return effective;
                }
                else if (instigatorElement == elements.ELECTRIC)
                {
                    return ineffective;
                }
                else
                {
                    return 1f;
                }
            case elements.FIRE:
                if (instigatorElement == elements.TOXIC)
                {
                    return effective;
                }
                else if (instigatorElement == elements.WATER)
                {
                    return ineffective;
                }
                else
                {
                    return 1f;
                }
            case elements.TOXIC:
                if (instigatorElement == elements.NANITES)
                {
                    return effective;
                }
                else if (instigatorElement == elements.FIRE)
                {
                    return ineffective;
                }
                else
                {
                    return 1f;
                }
            case elements.NANITES:
                if (instigatorElement == elements.EMP)
                {
                    return effective;
                }
                else if (instigatorElement == elements.TOXIC)
                {
                    return ineffective;
                }
                else
                {
                    return 1f;
                }
            case elements.EMP:
                if (instigatorElement == elements.ELECTRIC)
                {
                    return effective;
                }
                else if (instigatorElement == elements.NANITES)
                {
                    return ineffective;
                }
                else
                {
                    return 1f;
                }
            case elements.PHYSICAL:
            
                return 1f;
            
        }
        return 0f;
    }
}
