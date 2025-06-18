using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetGameManager : MonoBehaviourPunCallbacks
{
    public float Timer { get; private set; }
    public bool IsOpponentPlayer => PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient;
    public string Opponent { get; private set; }
    public string Me { get; private set; }

    
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            Hashtable props = new Hashtable
            {
                {"LOADED_LEVEL", true}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            transform.GetChild(0).GetComponent<PlayerLocal>().IsLocal = true;
            transform.GetChild(0).GetComponent<PlayerLocal>().IsOpponent = IsOpponentPlayer;
            Me = PhotonNetwork.LocalPlayer.NickName;
            Opponent = PhotonNetwork.PlayerListOthers[0].NickName;
        }
        else
        {
            Timer = 1;

            transform.GetChild(0).GetComponent<PlayerLocal>().IsLocal = true;
            transform.GetChild(1).GetComponent<PlayerLocal>().IsLocal = true;

            Me = "Player 1";
            Opponent = "Player 2";
        }
    }

    
    void Update()
    {
        
    }

    public void ResetTime()
    {
        if (PhotonNetwork.IsConnected)
            Timer = 0;
        else
            Timer = 1;
    }

    #region Network
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if ((changedProps.ContainsKey("LOADED_LEVEL") && PlayersReady("LOADED_LEVEL")) || 
            (changedProps.ContainsKey("IS_READY") && PlayersReady("IS_READY")))
        {
            UpdateTimer();
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("START_TIME"))
        {
            object startTime;
            if (propertiesThatChanged.TryGetValue("START_TIME", out startTime))
            {
                Timer = 1 - (float)(PhotonNetwork.Time - (float)startTime); 
            }
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("CardGame-Lobby");
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        LeaveGame();
    }

    public void LeaveGame()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneManager.LoadScene("CardGame-Lobby");
        }
    }

    void UpdateTimer()
    {
        Hashtable props = new Hashtable
        {
            {"START_TIME", (float)PhotonNetwork.Time}
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    #endregion

    #region Listeners
    public bool PlayersReady(string key)
    {
        if (PhotonNetwork.IsConnected)
        {
            foreach(Player p in PhotonNetwork.PlayerList)
            {
                if (p.CustomProperties.ContainsKey(key))
                {
                    object ready;
                    if (p.CustomProperties.TryGetValue(key, out ready))
                    {
                        if (!(bool)ready)
                            return false;
                    }
                }
                else
                    return false;
            }
        }

        return true;
    }

    public int RoomAlea()
    {
        if (PhotonNetwork.IsConnected)
        {
            object deckId;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("DECK_ID", out deckId))
            {
                return (int)deckId;
                
            }

            throw new System.ArgumentException("No Deck Id found");
        }

        return Random.Range(1, 1000);
    }
    #endregion
}
