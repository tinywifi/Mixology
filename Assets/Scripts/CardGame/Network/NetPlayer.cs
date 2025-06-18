using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class NetPlayer : MonoBehaviourPunCallbacks
{
    public bool IsConnected => PhotonNetwork.IsConnected;
    public bool IsReady { get; private set; }

    public struct PlayerMove
    {
        public string moveID;
        public float time;
        public int from;
        public int to;
    }

    PlayerLocal local;
    Queue<PlayerMove> moves;


    
    void Start()
    {
        local = GetComponent<PlayerLocal>();
        moves = new Queue<PlayerMove>();
        IsReady = true;
    }

    
    void Update()
    {
        
    }

    bool IsValid(Player player)
    {
        if (!local.IsLocal && player != PhotonNetwork.LocalPlayer)
            return true;

        return false;
    }

    #region Actions
    public void RegisterSlot(int stack, int slot)
    {
        Debug.Log("Register slot");

        slot = (slot + 1) % 2; 

        PlayerMove move = new PlayerMove() { moveID = "MOVE_SLOT", time = (float)PhotonNetwork.Time, from = stack, to = slot };

        moves.Enqueue(move);

        if (moves.Count == 1)
        {
            SendPlayerMove();
        }
    }

    public void RegisterStack(int stack, int goal)
    {
        PlayerMove move = new PlayerMove() { moveID = "MOVE_STACK", time = (float)PhotonNetwork.Time, from = stack, to = goal };

        moves.Enqueue(move);

        if (moves.Count == 1)
        {
            SendPlayerMove();
        }
    }

    public void SendPlayerReady(bool ready)
    {
        if (!IsConnected)
            return;

        Hashtable props = new Hashtable
        {
            {"IS_READY", ready}
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    void SendPlayerMove()
    {
        if (moves.Count == 0)
            return;

        PlayerMove move = moves.Dequeue();
        Debug.Log("Send");
        Hashtable props = new Hashtable()
            {
                {move.moveID, new Vector2(){ x = move.from, y = move.to} },
                {"TIMESTAMP", move.time }
            };

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
    #endregion

    #region Network

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        

        if (targetPlayer == PhotonNetwork.LocalPlayer && local.IsLocal && moves.Count > 0)
            SendPlayerMove();

        if (!IsValid(targetPlayer))
            return;

        if (changedProps.ContainsKey("IS_READY"))
        {
            local.SetReady();
        }

        if (changedProps.ContainsKey("MOVE_SLOT"))
        {
            object direction;
            changedProps.TryGetValue("MOVE_SLOT", out direction);

            Vector2 moveDir = (Vector2)direction;
            Debug.Log("New dir " + moveDir);

            local.OnClick((int)moveDir.x);
            local.ClickSlot((int)moveDir.y);
        }

        if (changedProps.ContainsKey("MOVE_STACK"))
        {
            object direction;
            changedProps.TryGetValue("MOVE_STACK", out direction);

            Vector2 moveDir = (Vector2)direction;
            Debug.Log("New dir " + moveDir);

            local.OnClick((int)moveDir.x);
            local.OnClick((int)moveDir.y);
        }
    }

    #endregion




}
