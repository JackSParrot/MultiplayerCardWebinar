using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardView : MonoBehaviour
{
    public Text PlayerHealth = null;
    public Text EnemyHealth = null;
    public Text PlayerName = null;
    public Text OpponentName = null;

    public Transform[] PlayerRackHolders;
    public Transform[] EnemyRackHolders;
    public Transform[] PlayerPlayzoneHolders;
    public Transform[] EnemyPlayzoneHolders;
    public GameObject AttackButton = null;
    public GameObject GameOverPopup = null;

    public CardCollectionSO Collection;
    public UserDataSO UserData;
    public OpponentSO Opponent;

    CardStats[] _playerRack = new CardStats[3];
    CardStats[] _enemyRack = new CardStats[3];

    CardStats[] _playerPlayzone = new CardStats[3];
    CardStats[] _enemyPlayzone = new CardStats[3];

    int _enemyHealth = 50;
    int _playerHealth = 50;
    bool _isPlayerTurn = true;
    int _previousId = -1;

    List<int> _playerPool = new List<int>();
    List<int> _enemyPool = new List<int>();

    void Start()
    {
        PlayerName.text = string.IsNullOrEmpty(UserData.PlayerName) ? "Me" : UserData.PlayerName;
        OpponentName.text = string.IsNullOrEmpty(Opponent.OpponentName) ? "Anonymous" : Opponent.OpponentName;

        if (UserData.PlayerDeck.Count < 8)
        {
            UserData.PlayerDeck.Clear();
            PlayerPrefs.SetString("Deck", "0,1,2,3,4,5,6,7");
        }

        var tmp = new List<int>();
        tmp.AddRange(UserData.PlayerDeck);
        _playerPool.Clear();
        while (tmp.Count > 0)
        {
            var choice = Random.Range(0, tmp.Count);
            _playerPool.Add(tmp[choice]);
            tmp.RemoveAt(choice);
        }
        tmp.AddRange(Opponent.OpponentDeck);
        _enemyPool.Clear();
        while (tmp.Count > 0)
        {
            var choice = Random.Range(0, tmp.Count);
            _enemyPool.Add(tmp[choice]);
            tmp.RemoveAt(choice);
        }
        FillRack(_playerRack, _playerPool);
        SetupZone(PlayerRackHolders, _playerRack);
        FillRack(_enemyRack, _enemyPool);
        SetupZone(EnemyRackHolders, _enemyRack);
        CleanupHolders(PlayerPlayzoneHolders);
        CleanupHolders(EnemyPlayzoneHolders);
        PlayerHealth.text = _playerHealth.ToString();
        EnemyHealth.text = _enemyHealth.ToString();
        GameOverPopup.SetActive(false);
    }

    void CleanupHolders(Transform[] collection)
    {
        for (int i = 0; i < collection.Length; ++i)
        {
            if (collection[i].childCount > 1)
            {
                Destroy(collection[i].GetChild(1).gameObject);
            }
        }
    }

    void SetupZone(Transform[] holders,  CardStats[] cards)
    {
        CleanupHolders(holders);
        for (int i = 0; i < cards.Length; ++i)
        {
            if (cards[i] != null)
            {
                var card = Instantiate(Collection.Prefab, Vector3.zero, Quaternion.identity).GetComponent<CardView>();
                card.SetData(cards[i], holders == EnemyPlayzoneHolders);
                card.Idx = i;
                card.transform.SetParent(holders[i], false);
            }
        }
    }

    void FillRack(CardStats[] rack, List<int> deck, bool limited = false)
    {
        for (int i = 0; i < rack.Length; ++i)
        {
            if(rack[i] == null)
            {
                rack[i] = new CardStats(Collection.Cards[deck[0]]);
                deck.Add(deck[0]);
                deck.RemoveAt(0);
                if (limited) return;
            }
        }
    }

    public void OnSlotPressed(int id)
    {
        if (!AttackButton.activeSelf) return;

        bool isPlayerRack(int n) { return n >= 0 && n <= 2; }
        bool isPlayerPlayzone(int n) { return n >= 3 && n <= 5; }
        bool isEnemyPlayzone(int n) { return n >= 6 && n <= 8; }
        bool isEnemyRack(int n) { return n >= 9 && n <= 11; }
        if (_isPlayerTurn && isPlayerRack(id))
        {
            if(_playerRack[id] != null)
            {
                _previousId = id;
            }
            else
            {
                _previousId = -1;
            }
        }
        else if (!_isPlayerTurn && isEnemyRack(id))
        {
            if (_enemyRack[id-9] != null)
            {
                _previousId = id;
            }
            else
            {
                _previousId = -1;
            }
        }
        else if (_isPlayerTurn && isPlayerRack(_previousId) && isPlayerPlayzone(id))
        {
            _playerPlayzone[id - 3] = _playerRack[_previousId];
            _playerRack[_previousId] = null;
            SetupZone(PlayerRackHolders, _playerRack);
            SetupZone(PlayerPlayzoneHolders, _playerPlayzone);
            _previousId = -1;
        }
        else if(_isPlayerTurn && isEnemyRack(_previousId) && isEnemyPlayzone(id))
        {
            _enemyPlayzone[id - 6] = _enemyRack[_previousId-9];
            _enemyRack[_previousId-9] = null;
            SetupZone(EnemyRackHolders, _enemyRack);
            SetupZone(EnemyPlayzoneHolders, _enemyPlayzone);
            _previousId = -1;
        }
        else
        {
            _previousId = -1;
        }
    }

    public void Attack()
    {
        StartCoroutine(PlayAttackCoroutine());
    }

    List<CardView> GetCardsFromHolders(Transform[] holders)
    {
        var retVal = new List<CardView>();
        for(int i = 0; i < holders.Length; ++i)
        {
            var card = holders[i].GetComponentInChildren<CardView>();
            if(card != null) retVal.Add(card);
        }
        return retVal;
    }

    void AttackTo(int slot)
    {
        if(_isPlayerTurn)
        {
            if(_playerPlayzone[slot] == null)
            {
                return;
            }
            if(_enemyPlayzone[slot] == null)
            {
                _enemyHealth = Mathf.Max(0, _enemyHealth - _playerPlayzone[slot].Card.Attack);
                EnemyHealth.text = _enemyHealth.ToString();
            }
            else
            {
                _enemyPlayzone[slot].CurrentHealth -= _playerPlayzone[slot].Card.Attack;
                if (_enemyPlayzone[slot].CurrentHealth <= 0)
                {
                    _enemyPlayzone[slot] = null;
                }
                SetupZone(EnemyPlayzoneHolders, _enemyPlayzone);
            }
        }
        else
        {
            if (_enemyPlayzone[slot] == null)
            {
                return;
            }
            if (_playerPlayzone[slot] == null)
            {
                _playerHealth = Mathf.Max(0, _playerHealth - _enemyPlayzone[slot].Card.Attack);
                PlayerHealth.text = _playerHealth.ToString();
            }
            else
            {
                _playerPlayzone[slot].CurrentHealth -= _enemyPlayzone[slot].Card.Attack;
                if (_playerPlayzone[slot].CurrentHealth <= 0)
                {
                    _playerPlayzone[slot] = null;
                }
                SetupZone(PlayerPlayzoneHolders, _playerPlayzone);
            }
        }
    }

    IEnumerator PlayAttackCoroutine()
    {
        AttackButton.SetActive(false);
        var cards = _isPlayerTurn ? GetCardsFromHolders(PlayerPlayzoneHolders) : GetCardsFromHolders(EnemyPlayzoneHolders);
        bool working = true;
        switch(cards.Count)
        {
            case 0:
                working = false;
                break;
            case 1:
                cards[0].PlayAttack(() => AttackTo(cards[0].Idx), () => working = false);
                break;
            case 2:
                cards[0].PlayAttack(() => AttackTo(cards[0].Idx), () => cards[1].PlayAttack(() => AttackTo(cards[1].Idx), () => working=false));
                break;
            case 3:
                cards[0].PlayAttack(() => AttackTo(cards[0].Idx),
                    () => cards[1].PlayAttack(() => AttackTo(cards[1].Idx),
                    () => cards[2].PlayAttack(() => AttackTo(cards[2].Idx), () => working = false)));
                break;
        }
        while(working)
        {
            yield return null;
        }
        if(_playerHealth <= 0 || _enemyHealth <= 0)
        {
            yield return new WaitForSeconds(0.5f);
            GameOverPopup.SetActive(true);
            yield break;
        }
        var rack = _isPlayerTurn ? _playerRack : _enemyRack;
        var deck = _isPlayerTurn ? _playerPool : _enemyPool;
        var placeholders = _isPlayerTurn ? PlayerRackHolders : EnemyRackHolders;
        FillRack(rack, deck, true);
        SetupZone(placeholders, rack);
        _isPlayerTurn = !_isPlayerTurn;
        AttackButton.SetActive(_isPlayerTurn);
        if(!_isPlayerTurn)
        {
            StartCoroutine(PlayEnemyTurn());
        }
    }

    IEnumerator PlayEnemyTurn()
    {
        for(int i = 0; i < _enemyPlayzone.Length; ++i)
        {
            if(_enemyPlayzone[i] == null)
            {
                for(int r = 0; r < _enemyRack.Length; ++r)
                {
                    if(_enemyRack[r] != null)
                    {
                        _enemyPlayzone[i] = _enemyRack[r];
                        _enemyRack[r] = null;
                        break;
                    }
                }
                SetupZone(EnemyPlayzoneHolders, _enemyPlayzone);
                SetupZone(EnemyRackHolders, _enemyRack);
                yield return new WaitForSeconds(0.5f);
            }
        }
        Attack();
    }

    public void GameOverPressed()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
