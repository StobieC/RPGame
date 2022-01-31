using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text hpText;
    [SerializeField] Text lvlText;
    [SerializeField] Text nameText;
    [SerializeField] HpBar hpBar;

    Foe _foe;

    public void SetData(Foe foeBase)
    {
        _foe = foeBase;
        nameText.text = foeBase.foeBase.name;
        lvlText.text = "Lvl " + Convert.ToString(foeBase.level);
        hpText.text = $"{_foe.HP} / {_foe.MaxHp}";
       // hpBar.SetHp((float)foeBase.HP / foeBase.MaxHp);
    }

    public IEnumerator UpdateHP()
    {
        hpText.text = $"{_foe.HP} / {_foe.MaxHp}";
        yield return hpBar.setHPSmoothly((float)_foe.HP / _foe.MaxHp);
    }

    public void setHpBar()
    {
        hpBar.SetHp((float)_foe.HP / _foe.MaxHp);
    }

    public void setHealth()
    {
        hpText.text = $"{_foe.HP} / {_foe.MaxHp}";
    }

}
