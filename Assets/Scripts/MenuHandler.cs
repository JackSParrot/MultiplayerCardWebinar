using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    public UserDataSO UserData;
    public OpponentSO Opponent;

    public GameObject Loading;
    public GameObject PvPButton;

    public InputField NameInput;
    public Text UserNameText;
    public GameObject UserNameButton;

    void Start()
    {
        if (string.IsNullOrEmpty(UserData.PlayerName))
        {
            NameInput.gameObject.SetActive(true);
            UserNameButton.SetActive(true);
            UserNameText.gameObject.SetActive(false);
        }
        else
        {
            NameInput.gameObject.SetActive(false);
            UserNameButton.SetActive(false);
            UserNameText.gameObject.SetActive(true);
            UserNameText.text = UserData.PlayerName;
        }
        StartCoroutine(LoadOpponentCoroutine());
        Debug.Log(UserData.PlayerId);
    }

    IEnumerator LoadOpponentCoroutine()
    {
        Loading.SetActive(true);
        PvPButton.SetActive(false);

        const string url = "#your_load_url_here#";
        var fullUrl = url + $"?uid={UserData.PlayerId}";
        var request = new UnityWebRequest(fullUrl, "GET", new DownloadHandlerBuffer(), null);
        request.redirectLimit = 50;
        request.timeout = 60;

        request.SendWebRequest();

        while (!request.isDone)
        {
            Loading.transform.Rotate(transform.forward, 90f * Time.deltaTime);
            yield return null;
        }

        try
        {
            var raw = request.downloadHandler.text;
            Debug.Log(raw);
            var split = raw.Split(':');
            var name = split[0];
            var deck = split[1].Split(',');
            Opponent.OpponentDeck.Clear();
            foreach(var c in deck)
            {
                Opponent.OpponentDeck.Add(int.Parse(c));
            }
            Opponent.OpponentName = name;
            Loading.SetActive(false);
            PvPButton.SetActive(true);
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
    }
 
    public void PlayAI()
    {
        Opponent.OpponentName = "Opponent";
        Opponent.OpponentDeck.Clear();
        for(int i = 0; i < 8; ++i)
        {
            Opponent.OpponentDeck.Add(UnityEngine.Random.Range(0, 20));
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void EditDeck()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }

    public void PvP()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void SaveName()
    {
        if(NameInput.text.Length < 3)
        {
            Debug.LogError("the name must be 3 letters or more");
            return;
        }
        UserData.PlayerName = NameInput.text;
        NameInput.gameObject.SetActive(false);
        UserNameButton.SetActive(false);
        UserNameText.gameObject.SetActive(true);
        UserNameText.text = UserData.PlayerName;
        UserData.Save();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            UserData.Reset();
        }
    }
}
