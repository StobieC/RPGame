using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] EnemyBase _base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHud hud;

    public Foe foe { get; set; }
    public bool IsPlayerUnit
    {
        get { return isPlayerUnit; }
    }

    public BattleHud Hud
    {
        get { return hud; }
    }
    Image Image;
    Vector3 originalPos;
    Color originalColor;

    private void Awake()
    {
        Image = GetComponent<Image>();
        originalPos = Image.transform.localPosition;
        originalColor = Image.color;
    }

    public void Setup()
    {
       foe = new Foe(_base, level);

        if(isPlayerUnit)
        {
            Image.sprite = foe.foeBase.FrontSprite;
        }
        else
        {
            Image.sprite = foe.foeBase.BackSprite;
        }
        Image.color = originalColor;

        PlayerEnterAnimation();
    }

    public void PlayerEnterAnimation()
    {
        if (isPlayerUnit)
            Image.transform.localPosition = new Vector3(-550f, originalPos.y);
        else
            Image.transform.localPosition = new Vector3(530f, originalPos.y);

        Image.transform.DOLocalMoveX(originalPos.x, 1f);
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();

        if (isPlayerUnit)
            sequence.Append(Image.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
        else
            sequence.Append(Image.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));

        sequence.Append(Image.transform.DOLocalMoveX(originalPos.x, 0.25f));

    }

    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(Image.DOColor(Color.gray, 0.1f));
        sequence.Append(Image.DOColor(originalColor, 0.1f));
    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(Image.transform.DOLocalMoveY(originalPos.y - 100f, 0.75f));
        sequence.Append(Image.DOFade(0f, 0.25f));
    }
}
