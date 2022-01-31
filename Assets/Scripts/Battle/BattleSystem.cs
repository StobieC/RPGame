using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum BattleState
{
    Start,
    ActionSelection,
    MoveSelection,
    EnemyMove,
    PerformMove,
    Busy,
    BattleOver,
}
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] DialogBoxBattle dialogBox;

    BattleState state;
    int currentAction;
    int currentMove;

    public event Action<bool> OnBattleOver;


    public void StartBattle()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup();
        enemyUnit.Setup();
        playerHud.SetData(playerUnit.foe);
        enemyHud.SetData(enemyUnit.foe);

        dialogBox.SetMoves(playerUnit.foe.Moves);
        
        StartCoroutine(dialogBox.TypeDialog($"A wild {enemyUnit.foe.foeBase.FoeName} has appeared"));
        yield return new WaitForSeconds(2f);


        ActionSelection();
    }

    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        StartCoroutine(dialogBox.TypeDialog("Choose an action"));
        dialogBox.EnableActionSelector(true);
        dialogBox.EnableDialogText(true);
        dialogBox.EnableMoveSelector(false);

    }

    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        OnBattleOver(won);
    }

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionState();
        } else if (state == BattleState.MoveSelection)
        {
            HandleMoveState();
        }
    }

    void HandleActionState()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 1)
                ++currentAction;
        } else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
            {
                --currentAction;
            }
        }

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                //Fight
                MoveSelection();
            } else if (currentAction == 1)
            {
                //Run
            }
        }
       
    }

    void HandleMoveState()
    {

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("down Pressed");
            if (currentMove < playerUnit.foe.Moves.Count - 2)
                currentMove +=2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > playerUnit.foe.Moves.Count -2)
            {
                Debug.Log("up Pressed");
                currentMove -= 2;
           }
        } else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerUnit.foe.Moves.Count -1)
            {
                ++currentMove;
            }

        } else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
           if (currentMove != 0)
            {
                --currentMove;
            }
        }

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.foe.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("X Pressed");
            state = BattleState.ActionSelection;
            ActionSelection();
        }

        if (Input.GetKeyDown(KeyCode.Z)) {
            Debug.Log("Z Pressed");
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());


        }
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator PerformPlayerMove()
    {

        state = BattleState.PerformMove;
        var move = playerUnit.foe.Moves[currentMove];
        yield return dialogBox.TypeDialog($"{playerUnit.foe.foeBase.name} used {move.Base.MoveName}");

        playerUnit.PlayAttackAnimation();

        yield return new WaitForSeconds(0.5f);
        enemyUnit.PlayHitAnimation();

        if (move.Base.Category == MoveCategory.Status)
        {
            Debug.Log("Stat Buff");
            var effects = move.Base.Effects;
            if (effects.Boosts != null)
            {
                if (move.Base.Target == MoveTarget.Self)
                    playerUnit.foe.ApplyBoosts(effects.Boosts);
                else
                    enemyUnit.foe.ApplyBoosts(effects.Boosts);
            }
        } else
        {
            // Take damage and assign isFainted to true if enemy is defeated
            DamageDetails damageDetails = enemyUnit.foe.TakeDamage(move, playerUnit.foe);
            // yield return enemyHud.UpdateHP();
            enemyHud.setHealth();
            yield return ShowDamageDetails(damageDetails);

        }

        if (enemyUnit.foe.HP <= 0)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.foe.foeBase.FoeName} is defeated");
            yield return new WaitForSeconds(0.5f);
            enemyUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1f);
            OnBattleOver(true);
        } else
        {
            StartCoroutine(EnemyMove());
        }
    }

    
    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;
        var move = enemyUnit.foe.GetRandomMove();

        yield return dialogBox.TypeDialog($"{enemyUnit.foe.foeBase.name} used {move.Base.MoveName}");
        enemyUnit.PlayAttackAnimation();

        if (move.Base.Category == MoveCategory.Status)
        {

            var effects = move.Base.Effects;
            if (effects.Boosts != null)
            {
                if (move.Base.Target == MoveTarget.Self)
                    enemyUnit.foe.ApplyBoosts(effects.Boosts);
                else
                    playerUnit.foe.ApplyBoosts(effects.Boosts);
            }
        }
        else {
            DamageDetails damageDetails = playerUnit.foe.TakeDamage(move, enemyUnit.foe);
            yield return new WaitForSeconds(0.5f);
            playerUnit.PlayHitAnimation();
            //yield return playerHud.UpdateHP();
            yield return new WaitForSeconds(1f);
            playerHud.setHealth();
            yield return ShowDamageDetails(damageDetails);


        }

      

        if (playerUnit.foe.HP <= 0)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.foe.foeBase.FoeName} is defeated");
            yield return new WaitForSeconds(0.5f);
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1f);
            OnBattleOver(false);
        }
        else
        {
            ActionSelection();
        }
    }
    

    /*
    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;
        var move = enemyUnit.foe.GetRandomMove();

        yield return dialogBox.TypeDialog($"{enemyUnit.foe.foeBase.name} used {move.Base.MoveName}");
        yield return RunMove(enemyUnit, playerUnit, move);
        if (state == BattleState.PerformMove)
             ActionSelection();
        
    }
    */

    //Refactor for seperate enemymove and playermove function
    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        state = BattleState.PerformMove;
       
        yield return dialogBox.TypeDialog($"{playerUnit.foe.foeBase.name} used {move.Base.MoveName}");

        sourceUnit.PlayAttackAnimation();

        yield return new WaitForSeconds(0.5f);
        targetUnit.PlayHitAnimation();

        // Take damage and assign isFainted to true if enemy is defeated
        DamageDetails damageDetails = targetUnit.foe.TakeDamage(move, playerUnit.foe);
        // yield return enemyHud.UpdateHP();
       // enemyHud.setHealth();
        targetUnit.Hud.setHealth();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{targetUnit.foe.foeBase.FoeName} is defeated");
            yield return new WaitForSeconds(0.5f);
            targetUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1f);
            CheckForBattleOver(targetUnit);
        }
    }

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            BattleOver(false);
        } else
        {
            BattleOver(true);
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
        {
            yield return dialogBox.TypeDialog("A critical hit!");
        }
    }


}
