using UnityEngine;

[System.Serializable]
public class CardData
{
    public string Title;
    public Sprite Image;
    public int Attack;
    public int Health;
}

public class CardStats
{
    public CardData Card;
    public int CurrentHealth;
    public CardStats(CardData data)
    {
        Card = data;
        CurrentHealth = data.Health;
    }
}
