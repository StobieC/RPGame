using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Foe", menuName = "Foes/Create new foe")]
public class EnemyBase : ScriptableObject
{

    [SerializeField] string foeName;
    [TextArea]
    [SerializeField] string description;
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [SerializeField] Type type1;
    [SerializeField] Type type2;

    //base stats
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defence;
    [SerializeField] int spAttack;
    [SerializeField] int spDefence;
    [SerializeField] int speed;

    [SerializeField] List<LearnableMoves> learnableMoves;

    public string FoeName { get { return foeName; } }
    public string Description { get { return description; } }
    public Sprite FrontSprite { get { return frontSprite; } }
    public Sprite BackSprite { get { return backSprite; } }
    public Type Type1 { get { return type1; } }
    public Type Type2 { get { return type2; } }

    public int MaxHp { get { return maxHp; } }

    public int Attack { get { return attack; } }

    public int Defence { get { return defence; } }

    public int SpAttack { get { return spAttack; } }

    public int SpDefence { get { return spDefence; } }

    public int Speed { get { return speed; } }

    public List<LearnableMoves> LearnableMoves { get { return learnableMoves; } } 
}

[System.Serializable]
public class LearnableMoves {
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase MoveBase { get { return moveBase; } }
    public int Level { get {return level; }}
}


public enum Type
{
    None,
    Normal,
    Fire,
    Water,
    Grass,
    Electric,
    Poison,
    Flying,
    Ghost,
    Dragon
}

public enum Stat
{
    Attack, 
    Defense,
    SpAttack, 
    SpDefence,
    Speed
}

public class TypeChart { 

     //TODO add effectiveness chart for determnining weaknesses / strengths of attacks

}

