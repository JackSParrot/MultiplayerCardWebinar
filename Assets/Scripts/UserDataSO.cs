using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "UserDataSO", menuName = "Cards/UserData")]
public class UserDataSO : ScriptableObject
{
    [SerializeField]
    string _playerId = "";
    public string PlayerId
    {
        get
        {
            if(string.IsNullOrEmpty(_playerId))
            {
                if (!PlayerPrefs.HasKey("ID") || PlayerPrefs.GetString("ID") == "")
                {
                    int seconds = (int)((DateTime.UtcNow - new DateTime(2000, 1, 1)).TotalMilliseconds % int.MaxValue);
                    PlayerPrefs.SetString("ID", "u" + seconds.ToString());
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

    [SerializeField]
    string _playerName = "";
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

    [SerializeField]
    List<int> _playerDeck = new List<int>();
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
            }
            return _playerDeck;
        }
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

            const string url = "#your_save_url_here#";
            var fullUrl = url + $"?uid={PlayerId}&name={ PlayerName}&deck={str}";
            var request = new UnityWebRequest(fullUrl, "GET", new DownloadHandlerBuffer(), null);
            request.redirectLimit = 50;
            request.timeout = 60;
            request.SendWebRequest().completed += hand => Debug.Log((hand as UnityWebRequestAsyncOperation).webRequest.downloadHandler.text);
        }
    }
}
