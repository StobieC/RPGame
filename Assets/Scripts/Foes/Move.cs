using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public Move(MoveBase moveBase)
    {
        Base = moveBase;
        PP = Base.PP;
        isSpecial = moveBase.IsSpecial;
    }

    public MoveBase Base { get; set; }
    public int PP { get; set; }

    public bool isSpecial { get; set; }


}
