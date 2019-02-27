using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DrawLine : NetworkBehaviour
{
    public GameObject linePrefab;
    public GameObject currentLine;

    public LineRenderer lineRender;
    public List<Vector2> mousePositions;

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Button Pressed!");
            CmdDrawOnServer();
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 tempMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Vector2.Distance(tempMousePosition, mousePositions[mousePositions.Count - 1]) > .1f)
            {
                CmdUpdateLineOnServer(tempMousePosition);
            }
        }
    }

    [Command]
    private void CmdDrawOnServer()
    {
        CreateLine();
        RpcDrawOnClient();
    }

    [ClientRpc]
    private void RpcDrawOnClient()
    {
        CreateLine();
    }

    [Command]
    private void CmdUpdateLineOnServer(Vector2 newMousePosition)
    {
        UpdateLine(newMousePosition);
        RpcUpdateLineOnClient(newMousePosition);
    }

    [ClientRpc]
    private void RpcUpdateLineOnClient(Vector2 newMousePosition)
    {
        UpdateLine(newMousePosition);
    }

    void CreateLine()
    {
        currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        lineRender = currentLine.GetComponent<LineRenderer>();
        mousePositions.Clear();
        mousePositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        mousePositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        lineRender.SetPosition(0, mousePositions[0]);
        lineRender.SetPosition(1, mousePositions[1]);
    }

    void UpdateLine(Vector2 newMousePosition)
    {
        mousePositions.Add(newMousePosition);
        lineRender.positionCount++;
        lineRender.SetPosition(lineRender.positionCount - 1, newMousePosition);
    }
}
