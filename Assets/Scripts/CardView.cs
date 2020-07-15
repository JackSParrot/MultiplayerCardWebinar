using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    public Text Title;
    public Text Attack;
    public Text Health;
    public Image Portrait;

    public int Idx = -1;
    bool _isOpponent = false;

    public void SetData(CardStats dataata, bool isOpponent)
    {
        _isOpponent = isOpponent;
        Title.text = dataata.Card.Title;
        Portrait.sprite = dataata.Card.Image;
        Attack.text = dataata.Card.Attack.ToString();
        Health.text = dataata.CurrentHealth.ToString();
    }

    public void PlayAttack(Action onHit, Action OnFinish)
    {
        StartCoroutine(PlayAttackCoroutine(onHit, OnFinish));
    }

    IEnumerator PlayAttackCoroutine(Action onHit, Action OnFinish)
    {
        var pos = transform.position;
        var distance = Screen.height * 0.15f;
        float elapsed = 0.5f;
        float sign = _isOpponent ? -1f : 1f;
        while(elapsed > 0f)
        {
            yield return null;
            transform.position = transform.position + Vector3.up * distance * Time.deltaTime * sign;
            elapsed -= Time.deltaTime;
        }
        onHit?.Invoke();
        while (elapsed < 0.5f)
        {
            yield return null;
            transform.position = transform.position - Vector3.up * distance * Time.deltaTime * sign;
            elapsed += Time.deltaTime;
        }
        transform.position = pos;
        OnFinish?.Invoke();
    }
}
