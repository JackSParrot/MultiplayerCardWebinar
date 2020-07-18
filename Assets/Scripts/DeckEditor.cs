using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckEditor : MonoBehaviour
{
    public List<Transform> Holders = new List<Transform>();
    public CardCollectionSO Collection;

    public Text SelectedText;
    List<int> _selected = new List<int>();
    public UserDataSO UserData;

    void Start()
    {
        _selected.Clear();
        _selected.AddRange(UserData.PlayerDeck);
        for(int i = 0; i < Holders.Count; ++i)
        {
            var bg = Holders[i].Find("PlaceholderBG");
            if(bg != null)
            {
                bg.gameObject.SetActive(_selected.Contains(i));
            }
        }
        for(int i = 0; i < Collection.Cards.Count && i < Holders.Count; ++i)
        {
            var card = Instantiate(Collection.Prefab, Vector3.zero, Quaternion.identity).GetComponent<CardView>();
            var parent = Holders[i].Find("Card");
            card.transform.SetParent(parent, false);
            card.SetData(new CardStats(Collection.Cards[i]), false);
        }
        SelectedText.text = $"{_selected.Count}/8";
    }

    public void CardTapped(int idx)
    {
        var border = Holders[idx].Find("PlaceholderBG");
        if(border != null)
        {
            var selected = border.gameObject.activeSelf;
            if (_selected.Count >= 8 && !selected) return;

            if (selected) _selected.Remove(idx);
            else _selected.Add(idx);
            border.gameObject.SetActive(!selected);
        }
        SelectedText.text = $"{_selected.Count}/8";
    }

    public void BackToMenu()
    {
        if(_selected.Count == 8)
        {
            UserData.SetDeck(_selected);
            UserData.Save();
        }
        else
        {
            Debug.Log("Deck not saved it needs to be 8 cards");
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
