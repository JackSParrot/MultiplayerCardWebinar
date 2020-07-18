using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "UserDataSO", menuName = "Cards/UserData")]
public class UserDataSO : ScriptableObject
{
    [SerializeField]
    string _playerId = "";

    [SerializeField]
    string _playerName = "";

    [SerializeField]
    List<int> _playerDeck = new List<int>();

    public string PlayerId
    {
        get
        {
            if(string.IsNullOrEmpty(_playerId))
            {
                if (!PlayerPrefs.HasKey("ID") || string.IsNullOrEmpty(PlayerPrefs.GetString("ID")))
                {
                    int seconds = (int)((DateTime.UtcNow - new DateTime(2000, 1, 1)).TotalMilliseconds % int.MaxValue);
                    PlayerPrefs.SetString("ID", "u" + seconds.ToString() + UnityEngine.Random.Range(0, 1000).ToString());
                }
                _playerId = PlayerPrefs.GetString("ID");
            }
            return _playerId;
        }
        set
        {
            _playerId = value;
            PlayerPrefs.SetString("ID", _playerId);
        }
    }

    public string PlayerName
    {
        get
        {
            if (string.IsNullOrEmpty(_playerName))
            {
                if (PlayerPrefs.HasKey("Name"))
                {
                    _playerName = PlayerPrefs.GetString("Name");
                }
            }
            return _playerName;
        }
        set
        {
            _playerName = value;
            PlayerPrefs.SetString("Name", _playerName);
        }
    }

    public List<int> PlayerDeck
    {
        get
        {
            if (_playerDeck.Count < 1)
            {
                if (PlayerPrefs.HasKey("Deck"))
                {
                    var numbers = PlayerPrefs.GetString("Deck").Split(',');
                    if(numbers.Length > 1)
                    {
                        foreach (var n in numbers)
                        {
                            _playerDeck.Add(int.Parse(n));
                        }
                    }
                }
                else
                {
                    for(int i = 0; i < 8; ++i)
                    {
                        _playerDeck.Add(i);
                    }
                    Save();
                }
            }
            return _playerDeck;
        }
    }

    public void SetDeck(List<int> newDeck)
    {
        _playerDeck.Clear();
        _playerDeck.AddRange(newDeck);
    }

    void OnEnable()
    {
        _playerDeck = new List<int>();
        _playerId = "";
        _playerName = "";
    }

    public void Reset()
    {
        _playerName = "";
        PlayerPrefs.SetString("Name", _playerName);
        _playerDeck.Clear();
        PlayerPrefs.SetString("Deck", "");
        _playerId = "";
        PlayerPrefs.SetString("ID", _playerId);
    }

    public void Save()
    {
        if(_playerDeck.Count > 7)
        {
            var str = "";
            foreach(var n in PlayerDeck)
            {
                str += "," + n.ToString();
            }
            str = str.Remove(0, 1);
            PlayerPrefs.SetString("Deck", str);
            Debug.Log("Saved: " + str);

            const string url = "http://link_a_tu_save_user";
            var fullUrl = url + $"?uid={PlayerId}&name={PlayerName}&deck={str}";
            var request = new UnityWebRequest(fullUrl, "GET", new DownloadHandlerBuffer(), null);
            request.redirectLimit = 50;
            request.timeout = 60;
            request.SendWebRequest().completed += hand => Debug.Log((hand as UnityWebRequestAsyncOperation).webRequest.downloadHandler.text);
        }
    }
}
