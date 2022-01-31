using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foe
{
    public EnemyBase foeBase { get; set; }
    public int level { get; set; }
    public int HP { get; set; }

    public List<Move> Moves { get; set; }

    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    public Foe(EnemyBase foeBase, int level)
    {
        this.foeBase = foeBase;
        this.level = level;
      
        //Generate Moves
        Moves = new List<Move>();
        foreach (var move in foeBase.LearnableMoves)
        {
            if (move.Level <= level)
            {
                Moves.Add(new Move(move.MoveBase));
            }

            if (Moves.Count >= 4)
            {
                break;
            }
        }

        CalculateStats();
        HP = MaxHp;

        StatBoosts = new Dictionary<Stat, int>() {
            { Stat.Attack, 0},
            { Stat.Defense, 0},
            { Stat.SpAttack, 0},
            { Stat.SpDefence, 0},
            { Stat.Speed, 0}
        };
    }

    int GetStat(Stat stat)
    {
        int statval = Stats[stat];

        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
        {
            Debug.Log($"Buffed {stat.ToString()} by {boost}");
            statval = Mathf.FloorToInt(statval * boostValues[boost]);
        }
        else 
        { 
            Debug.Log($"DeBuffed {stat.ToString()} by {boost}");
            statval = Mathf.FloorToInt(statval / boostValues[-boost]);
        }

        return statval;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);
            if (boost > 0)
                Debug.Log($"{stat} has been buffed to {StatBoosts[stat]}");
            else
                Debug.Log($"{stat} has been debuffed to {StatBoosts[stat]}");
        }
    }

    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }

    public int Defence
    {
        get { return GetStat(Stat.Defense); }
    }

    public int SpAttack
    {
        get { return GetStat(Stat.SpAttack); }
    }

    public int SpDefence
    {
        get { return GetStat(Stat.SpDefence); }
    }

    public int Speed
    {
        get { return GetStat(Stat.Speed); }

    }
    public int MaxHp
    {
        get;
        private set;
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((foeBase.Attack * level) / 100f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((foeBase.Defence * level) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((foeBase.SpAttack * level) / 100f) + 5);
        Stats.Add(Stat.SpDefence, Mathf.FloorToInt((foeBase.SpDefence * level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((foeBase.Speed * level) / 100f) + 5);

        MaxHp =  Mathf.FloorToInt((foeBase.MaxHp * level) / 100f) + 10;
    }

    public DamageDetails TakeDamage(Move move, Foe attacker)
    {
        float d = 0f;
        float criticalChance = 1f;
        if (Random.value * 100f <= 6.25f)
            criticalChance = 2f;

        var damageDetails = new DamageDetails() { Critical = criticalChance, Fainted = false };


        float randomModifier = Random.Range(0.85f, 1f) * criticalChance;
        float a = (2 * attacker.level + 10) / 250f;
        if (move.Base.Category == MoveCategory.Special)
        {
            d = a * move.Base.Power * ((float)attacker.SpAttack / SpDefence) + 2;
        } else
        {
            d = a * move.Base.Power * ((float)attacker.Attack / Defence) + 2;
        }
        
        int damage = Mathf.FloorToInt(d * randomModifier);

        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            damageDetails.Fainted = true;
        }

        return damageDetails;
    }

    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }

}

public class DamageDetails { 

    public bool Fainted { get; set; }

    public float Critical { get; set; }

    public float TypeEffectiveness { get; set; }
}

