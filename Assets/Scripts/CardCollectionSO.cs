using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardCollectionSO", menuName = "Cards/Collection")]
public class CardCollectionSO : ScriptableObject
{
    public GameObject Prefab;
    public List<CardData> Cards = new List<CardData>();
}
