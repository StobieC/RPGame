using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour, Interactable
{

    [SerializeField] TextMessage npcTextMessage;
    [SerializeField] MoveDirection moveDirection;
    [SerializeField] List<Vector2> movePattern;
    [SerializeField] float waitTime;
    public float moveSpeed = 5.0f;
    float idleTimer = 0f;
    NPCState state = NPCState.Idle;

    int currentMovePattern = 0;

    private Character character;
    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void Interact()
    {
        StartCoroutine(DialogManager.Instance.ShowDialog(npcTextMessage));
        //StartCoroutine(character.Move(new Vector2(0, 100)));
        Debug.Log("Interact With NPC");
    }

    private void Update()
    {
        if (!DialogManager.Instance.IsShowing)
        {
            //Handle animation update
            if (state == NPCState.Idle)
            {
                idleTimer += Time.deltaTime;
                if (idleTimer > waitTime)
                {
                    if (movePattern.Count > 0)
                    {
                        StartCoroutine(Walk());
                    }
                    idleTimer = 0f;
                }
            }
        }
    }

    IEnumerator Walk()
    {
        Debug.Log($"{currentMovePattern} {movePattern.Count}");
        state = NPCState.Walking;

        yield return character.Move(movePattern[currentMovePattern], null, true);

        if (currentMovePattern + 1 == movePattern.Count)
        {
            currentMovePattern = 0;
        } else
        {
            currentMovePattern++;
        }

        state = NPCState.Idle;

    }



}

public enum MoveDirection
{
    North, South, East, West
}

public enum NPCState { Idle, Walking }
