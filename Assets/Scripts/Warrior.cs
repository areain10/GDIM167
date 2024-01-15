using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Character
{
    // Start is called before the first frame update
    public int specialDmg;
    public Warrior()
    {

    }
    public override void buffDmg(float multiplier)
    {
        damage = Mathf.RoundToInt(multiplier * damage);
        specialDmg = Mathf.RoundToInt(multiplier *damage);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
