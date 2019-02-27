using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using UnityEngine.Networking;

public class Countdown : NetworkBehaviour
{

    [SyncVar] public int timeLeft = 10;
    [SyncVar] public bool masterTimer = false;
    Countdown serverTimer;

    public Text text;
    public Button button;

    public int MAX_PLAYERS = 2;

    [SyncVar]
    public int readyPlayers = 0;

    public bool ready = false;
    
    // Start is called before the first frame update
    void Start()
    {

        /*
         * Setting all timer settings for client and server
         */

        // Host uses the timer and controls it
        if (isServer)
        {
            if (hasAuthority)
            {
                serverTimer = this;
                masterTimer = true;
            }
        }
        else
        {
            Countdown clientTimer = FindObjectOfType<Countdown>();
            if (clientTimer.masterTimer)
            {
                serverTimer = clientTimer;
            }
        }
        
        /*
         * ClientRPC can only be accessed from Server.
         */
         
        // Client
        if (isClient && !isServer)
        {
            Debug.Log("In client...");
            CmdAddListener();
        }

        // Server
        if (isServer)
        {
            Debug.Log("In server...");
            RpcAddListener();
        }

        Time.timeScale = 1;
    }

    [ClientRpc]
    void RpcSyncReadyPlayers(int players)
    {
        readyPlayers = players;
    }

    private void CmdAddListener()
    {
        button.onClick.AddListener(RoundStart);
    }

    private void RpcAddListener()
    {
        button.onClick.AddListener(RoundStart);
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer && !ready)
        {
            RpcSyncReadyPlayers(readyPlayers);
            ready = true;
        }
    }

    [ClientRpc]
    void RpcSyncTimeLeft(int seconds)
    {
        timeLeft = seconds;
    }

    void RoundStart()
    {
        Debug.Log("You clicked ready button!");

        readyPlayers++;
        Debug.Log("Presses: " + readyPlayers);

        if (readyPlayers == MAX_PLAYERS)
        {
            StartCoroutine("LoseTime");
        }
        Destroy(button.gameObject);
    }

    IEnumerator LoseTime()
    {
        Debug.Log("LoseTime has started!");
        while(timeLeft > 0)
        {
            yield return new WaitForSeconds(1);

            // Server timer controls time
            if (masterTimer)
            {
                timeLeft--;
                text.text = "Planning Time Left: " + timeLeft;
                Debug.Log("Time Left: " + timeLeft);
            }

            // Everyone updates their own time accordingly
            if (hasAuthority)
            {
                if (serverTimer)
                {
                    timeLeft = serverTimer.timeLeft;
                }
                else
                {
                    Countdown clientTimer = FindObjectOfType<Countdown>();
                    if (clientTimer.masterTimer)
                    {
                        serverTimer = clientTimer;
                    }
                    Debug.Log("Client Time Left: " + serverTimer.timeLeft);
                }
            }
        }
    }
}
