using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] 
    PlayerController playerController;
    [SerializeField]
    BattleSystem battleSystem;
    [SerializeField]
    Camera mainCamera;
    GameState state;
    // Start is called before the first frame update
    void Start()
    {
        playerController.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == GameState.Roaming)
        {
            playerController.HandleUpdate();
        } else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
    }

    void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(false);

        battleSystem.StartBattle();
    }

    void EndBattle(bool outcome)
    {
        state = GameState.Roaming;
        battleSystem.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
    }
 }

public enum GameState { Roaming, Battle, Inventory}
