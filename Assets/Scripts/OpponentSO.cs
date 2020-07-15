using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="OpponentSO", menuName ="Cards/Opponent", order=2)]
public class OpponentSO : ScriptableObject
{
    public string OpponentName;
    public List<int> OpponentDeck = new List<int>();
}
