using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour {
    public static UiController Instance;

    public Text DebugText;

    Stack<string> CurrentDataStack;

    private void Awake()
    {
        Instance = this;
        CurrentDataStack = new Stack<string>();
        TcpServerConnection.MessageRecieved += CheckData;
    }

    private void OnDestroy()
    {
        TcpServerConnection.MessageRecieved -= CheckData;
    }
    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (CurrentDataStack.Count > 0)
        {
            DebugUIText(CurrentDataStack.Pop());
        }
	}
    public void CheckData(string txtValue)
    {
        CurrentDataStack.Push(txtValue);
    }

    public void DebugUIText(string txtValue)
    {
        DebugText.text += "\n"+txtValue;
    }
    public void ClearDebugUIText()
    {
        DebugText.text = "";
    }
}
