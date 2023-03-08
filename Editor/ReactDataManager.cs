using System;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

enum VisualScriptingVariables
{
    UserDataJSON,
    NFTCollectionData,
    UserOwnedNFTCollectionData,
    QuestCollectionData,
}

public class ReactDataManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void GetUserData(string gameObjectName, string functionName);

    [DllImport("__Internal")]
    private static extern void GetNFTCollectionData(string gameObjectName, string functionName);

    [DllImport("__Internal")]
    private static extern void GetUserOwnedNFTCollectionData(string gameObjectName, string functionName);

    [DllImport("__Internal")]
    private static extern void BuyNFT(string gameObjectName, string functionName1, string functionName2, int collectionID, int nftID, ulong nftCost);

    [DllImport("__Internal")]
    private static extern void SetCurrentNFT(string gameObjectName, string functionName1, int collectionID, int nftID);

    [DllImport("__Internal")]
    private static extern void SellNFT(int collectionID, int nftID, int nftCost);

    [DllImport("__Internal")]
    private static extern void GetQuestData(string gameObjectName, string functionName);

    [DllImport("__Internal")]
    private static extern void CompleteQuest(int questId, string gameObjectName, string functionName);


    public static ReactDataManager Instance { get; private set; }

    // Delegates & Events
    public delegate void GetUserDataCallback();
    public static event GetUserDataCallback OnGetUserDataCallback;

    public delegate void GetNFTCollectionDataCallback();
    public static event GetNFTCollectionDataCallback OnGetNFTCollectionDataCallback;

    public delegate void GetUserOwnedNFTCollectionDataCallback();
    public static event GetUserOwnedNFTCollectionDataCallback OnGetUserOwnedNFTCollectionDataCallback;

    public delegate void SetCurrentNFTSuccessCallback();
    public static event SetCurrentNFTSuccessCallback OnSetCurrentNFTSuccessCallback;
    public static event SetCurrentNFTSuccessCallback OnSetCurrentNFTFailCallback;

    // Lifecycle Methods
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Dispatch events from Unity to React app
    public void CallGetUserData(string gameObjectName, string functionName)
    {
        GetUserData(gameObjectName, functionName);
    }


    public void CallGetNFTCollectionData(string gameObjectName, string functionName)
    {
        GetNFTCollectionData(gameObjectName, functionName);
    }

    public void CallBuyNFT(string gameObjectName, string functionName1, string functionName2, int collectionID, int nftID, ulong nftCost)
    {
        Debug.Log("React Data Manager calling: " + gameObjectName + " , " + functionName1 + " , " + functionName2 + " , " + collectionID + " , " + nftID + " , " + nftCost);
        BuyNFT(gameObjectName, functionName1, functionName2, collectionID, nftID, nftCost);
    }

    public void CallGetUserOwnedNFTCollectionData(string gameObjectName, string functionName)
    {
        GetUserOwnedNFTCollectionData(gameObjectName, functionName);
    }

    public void CallSetCurrentNFT(string gameObjectName, string functionName, int collectionID, int nftID)
    {
        // Locally modify the currentNFT selected for display in the NFT wall
        UserData userData = (UserData)Variables.Application.Get(VisualScriptingVariables.UserDataJSON.ToString());
        userData.selectedLocalNft.Clear();
        userData.selectedLocalNft.Add(collectionID);
        userData.selectedLocalNft.Add(nftID);
        Variables.Application.Set(VisualScriptingVariables.UserDataJSON.ToString(), userData);

        SetCurrentNFT(gameObjectName, functionName, collectionID, nftID);
    }

    public void CallSellNFT(int collectionID, int nftID, int nftCost)
    {
        SellNFT(collectionID, nftID, nftCost);
    }

    public void CallGetQuestData(string gameObjectName, string functionName)
    {
        GetQuestData(gameObjectName, functionName);
    }

    public void CallCompleteQuest(int questId, string gameObjectName, string functionName)
    {
        Debug.Log("Calling Complete Quest from React Data Manager");

        CompleteQuest(questId, gameObjectName, functionName);

        Debug.Log("Called complete quest");
    }

    // Handle React callbacks
    public void GetUserData(string dataJSON)
    {
        // Deserialize the JSON data
        UserData userData = JsonUtility.FromJson<UserData>(dataJSON);

        // Save the data into visual scripting object
        Variables.Application.Set(VisualScriptingVariables.UserDataJSON.ToString(), userData);

        // Dispatch Unity delegate event
        OnGetUserDataCallback?.Invoke();
    }

    public void GetNFTCollectionData(string dataJSON)
    {
        // Deserialize the JSON data
        NFTCollectionData collection = JsonUtility.FromJson<NFTCollectionData>(dataJSON);

        // Save the data into visual scripting object
        if (collection != null)
        {
            Variables.Application.Set(VisualScriptingVariables.NFTCollectionData.ToString(), collection);
        }

        // Dispatch Unity delegate event
        OnGetNFTCollectionDataCallback?.Invoke();
    }

    public void GetUserOwnedNFTCollectionData(string dataJSON)
    {
        // Deserialize the JSON data
        NFTCollectionData collection = JsonUtility.FromJson<NFTCollectionData>(dataJSON);

        // Save the data into visual scripting object
        if (collection != null)
        {
            Variables.Application.Set(VisualScriptingVariables.UserOwnedNFTCollectionData.ToString(), collection);
        }

        // Dispatch Unity delegate event
        OnGetUserOwnedNFTCollectionDataCallback?.Invoke();
    }


    public void GetUserQuestsData(string questJSON)
    {
        //Deserialize the JSON quest data
        QuestCollectionData quests = JsonUtility.FromJson<QuestCollectionData>(questJSON);

        if (quests != null)
        {
            Variables.Application.Set(VisualScriptingVariables.QuestCollectionData.ToString(), quests);

            Debug.Log(quests.quests);
            QuestCollectionData qcd = (QuestCollectionData)Variables.Application.Get(VisualScriptingVariables.QuestCollectionData.ToString());

            Debug.Log("Quest array contains " + qcd.quests.Count + " items");

            Debug.Log("Quest Data from GetUserQuestsData method in Unity:" + Variables.Application.Get(VisualScriptingVariables.QuestCollectionData.ToString()));
        }

    }

    public void GetCurrentNFTModifyStatus(int status)
    {
        if (status == 1)
        {
            // Confirm the local selected NFT
            UserData userData = (UserData)Variables.Application.Get(VisualScriptingVariables.UserDataJSON.ToString());
            userData.currentNft.Clear();
            userData.currentNft.AddRange(userData.selectedLocalNft);
            Variables.Application.Set(VisualScriptingVariables.UserDataJSON.ToString(), userData);

            OnSetCurrentNFTSuccessCallback?.Invoke();
        }
        else
        {
            OnSetCurrentNFTFailCallback?.Invoke();
        }
    }

}