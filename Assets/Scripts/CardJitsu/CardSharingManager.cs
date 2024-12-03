using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;


public class CardSharingManager : NetworkBehaviour
{
    [SerializeField] private string buttonPath = "PlayerOne/PrefabCanvas/PlayTurn";
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button disconnectButton;
    [SerializeField] private GameObject cardSharingManagerPrefab;
    [SerializeField] private GameObject playerOne; // Reference to the PlayerOne GameObject
    [SerializeField] private GameObject deckOverlay; // Reference to the PlayerOne GameObject
    [SerializeField] private GameObject discardOverlay; // Reference to the PlayerOne GameObject
    [SerializeField] public TextMeshProUGUI hostWin; 
    [SerializeField] public TextMeshProUGUI clientWin;
    [SerializeField] public TextMeshProUGUI winScreen; 
    [SerializeField] public TextMeshProUGUI loseScreen;  
    [SerializeField] private GameManager2 gm; // Reference to the GameManager GameObject

    private List<GameObject> sharedCardCopies = new List<GameObject>();
    private Dictionary<(int cardId, ulong clientId), Card> displayedCards = new Dictionary<(int, ulong), Card>();
    
    private Dictionary<int, Card> cardLookup = new Dictionary<int, Card>();
    private List<CardData> selectedCards = new List<CardData>();
    private NetworkList<CardData> sharedCards;
    
    private int cardDisplayOrder = 0;
    private int updateCount = 0;
    public int hW = 0;
    public int cW = 0;


    public Card[] allCards;
    public List<GameObject> powerSlots = new List<GameObject>();

    

    private void Awake()
    {
        sharedCards = new NetworkList<CardData>();
        sharedCards.OnListChanged += OnSharedCardsUpdated;

        GameObject buttonObject = GameObject.Find(buttonPath);
        if (buttonObject != null)
        {
            confirmButton = buttonObject.GetComponent<Button>();
            if (confirmButton != null)
            {
                confirmButton.onClick.AddListener(OnConfirmButtonPressed);
                buttonObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("Button not found at path: " + buttonPath);
        }
    }

    /*public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsClient && !IsHost)
        {
            // For clients, initialize and subscribe to the NetworkList change event
            sharedCards.OnListChanged += OnSharedCardsUpdated;
        }
    }*/

    private void Start()
    {
        if (sharedCards != null)
        {
            sharedCards.OnListChanged += OnSharedCardsUpdated;
        }

        Card[] allCardInstances = Resources.FindObjectsOfTypeAll<Card>();
        allCards = new Card[allCardInstances.Length];
        int index = 0;
        foreach (Card card in allCardInstances)
        {
            //Debug.LogWarning($"Card Object ID: {card.id}. Card {card.cardName}.");
            if (card.gameObject.scene.IsValid())
            {
                allCards[index++] = card;
            }
        }
        System.Array.Resize(ref allCards, index);

        foreach (var card in allCards)
        {
            if (!cardLookup.ContainsKey(card.id))
            {
                cardLookup.Add(card.id, card);
            }
            else
            {
                Debug.LogWarning($"Duplicate card ID detected: {card.id}. Card {card.cardName} was not added to lookup.");
            }
        }
    }

    //public void Update(){
    //    CheckAndResetGame();
    //}

    private void OnDestroy()
    {
        if (sharedCards != null)
        {
            sharedCards.OnListChanged -= OnSharedCardsUpdated;
            sharedCards.Dispose();
            sharedCards = null;
        }
    }

    private void OnConfirmButtonPressed()
    {
        
        if (IsOwner)
        {
            SubmitCardsToServer(selectedCards);
            selectedCards.Clear();
        }
        else
        {

            // Clients send their selected cards to the server
            SubmitCardsToHostServerRpc(selectedCards.ToArray());
            selectedCards.Clear();
        }
    }

    public void SelectCard(Card card)
    {
        var cardData = new CardData(card.id, card.cost, card.power, card.sourceClientId);
        if (!selectedCards.Contains(cardData))
        {
            selectedCards.Add(cardData);
        }
    }

    public void DeselectCard(Card card)
    {
        selectedCards.RemoveAll(c => c.id == card.id);
    }

    private void SubmitCardsToServer(List<CardData> cardDataList)
    {
        int[] ids = new int[cardDataList.Count];
        int[] costs = new int[cardDataList.Count];
        int[] powers = new int[cardDataList.Count];

        for (int i = 0; i < cardDataList.Count; i++)
        {
            ids[i] = cardDataList[i].id;
            costs[i] = cardDataList[i].cost;
            powers[i] = cardDataList[i].power;
        }

        Debug.Log("Selected Card IDs: " + string.Join(", ", ids));
        Debug.Log("Selected Card Costs: " + string.Join(", ", costs));
        Debug.Log("Selected Card Powers: " + string.Join(", ", powers));

        SubmitCardsToServerRpc(ids, costs, powers);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SubmitCardsToHostServerRpc(CardData[] cardDataList, ServerRpcParams rpcParams = default)
    {
        foreach (var cardData in cardDataList)
        {
            var updatedCardData = new CardData(cardData.id, cardData.cost, cardData.power, rpcParams.Receive.SenderClientId);

            if (!sharedCards.Contains(updatedCardData))
            {
                sharedCards.Add(updatedCardData);
            }
        }

        // Disable playerOne for the corresponding client
        var clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { rpcParams.Receive.SenderClientId }
            }
        };

        SetPlayerOneInactiveClientRpc(clientRpcParams);

        // playerOne.SetActive(false); // Reference to the PlayerOne GameObject
        Debug.Log("Cards from client added to shared list on host.");
    }

    [ServerRpc(RequireOwnership = false)]
    private void SubmitCardsToServerRpc(int[] ids, int[] costs, int[] powers, ServerRpcParams rpcParams = default)
    {
        // Get the clientId of the client that called this RPC
        ulong clientId = rpcParams.Receive.SenderClientId;

        sharedCards.Clear(); // Clear the list first

        // Add each card using the provided arrays, including clientId as the source
        for (int i = 0; i < ids.Length; i++)
        {
            CardData cardData = new CardData(ids[i], costs[i], powers[i], clientId);
            sharedCards.Add(cardData); // Add selected cards to the shared network list
        }

        gm.Update();
        playerOne.SetActive(false);
        Debug.Log($"Selected cards shared with all clients by client {clientId}.");
    }

    [ClientRpc]
    private void SetPlayerOneInactiveClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (playerOne != null)
        {
            playerOne.SetActive(false);
            Debug.Log("playerOne object set to inactive for the corresponding client.");
        }
        else
        {
            Debug.LogWarning("playerOne object is null on this client.");
        }
    }

    private void OnSharedCardsUpdated(NetworkListEvent<CardData> changeEvent)
    {
        updateCount++;
        //Debug.Log("Shared cards updated across clients:");
        
        Card lastCard = null;
        CardData lastCardData  = new CardData(0, 0, 0, 0);
        
        foreach (var cardData in sharedCards)
        {
            // Check if the card has already been displayed
            //if (!displayedCards.Contains(cardData.id))
            //{
                if (cardLookup.TryGetValue(cardData.id, out Card card))
                {
                    lastCard = card;
                    lastCardData = cardData;
                    //DisplaySharedCard(card, cardData.clientId);
                    //displayedCards.Add(cardData.id); // Mark this card as displayed
                    //Debug.Log($"Card with ID {cardData.id} and client {cardData.clientId}.");
                }
                else
                {
                    Debug.LogWarning($"Card with ID {cardData.id} not found in lookup table.");
                }
            //}
        }

        if(lastCard != null && updateCount % 2 == 0){
            //DisplaySharedCard(lastCard, lastCardData.clientId);
            //Debug.Log($"Update: {updateCount}");
            //Debug.Log($"Card with ID {lastCardData.id} and client {lastCardData.clientId}.");
            var key = (lastCardData.id, lastCardData.clientId);
            displayedCards[key] = lastCard;
        }

        if(updateCount % 6 == 0){
            foreach (var entry in displayedCards)
            {
                var key = entry.Key; // Composite key (cardId, clientId)
                Card card = entry.Value; // Card object

                int cardId = key.cardId;
                ulong clientId = key.clientId;
                //Debug.Log($"Card with ID {cardId} and client {clientId}.");

                // Display the card using the existing DisplaySharedCard method
                DisplaySharedCard(card, clientId);
            }
            hostWin.gameObject.SetActive(true);
            clientWin.gameObject.SetActive(true);

            CalculateTurn();
            
        }

    }

    public void CalculateTurn(){
        StartCoroutine(DisplayMatchupsWithPause());
    }

    private IEnumerator DisplayMatchupsWithPause()
    {
        for (int i = 0; i < 1; i++)
        {
            if(hW == 7 || cW == 7){
                break;
            }
            if(i == 0){
                yield return new WaitForSeconds(2.5f);     
            }

            Card c1 = sharedCardCopies[i].GetComponent<Card>();
            Card c2 = sharedCardCopies[i + 1].GetComponent<Card>();

            Debug.Log($"Displaying matchup {i + 1}: {c1.cardName} {c1.sourceClientId}  - {c2.cardName} {c2.sourceClientId}");
            
            if(compareTypes(c1.type, c2.type)){
                c1.power = c1.power * 4;

                powerSlots[i].SetActive(true);
                powerSlots[i].transform.position = sharedCardCopies[i].transform.position;
                powerSlots[i].transform.position += new Vector3(-0.8f, 1.1f, -8f);

                yield return new WaitForSeconds(2.5f);  
            }
            else if(compareTypes(c2.type, c1.type)){
                c2.power = c2.power * 4;

                powerSlots[i].SetActive(true);
                powerSlots[i].transform.position = sharedCardCopies[i + 1].transform.position;
                powerSlots[i].transform.position += new Vector3(-0.8f, 1.1f, -8f);

                yield return new WaitForSeconds(2.5f);  
            }


            if (c1.power > c2.power)
            {
                if(c1.sourceClientId == 0){
                    hW++;
                    hostWin.text = hW.ToString();
                }
                else{
                    cW++;
                    clientWin.text = cW.ToString();
                }

                c2.gameObject.SetActive(false);

                if(compareTypes(c2.type, c1.type))
                    powerSlots[i].SetActive(false);
            }
            else if (c2.power > c1.power)
            {
                if(c2.sourceClientId == 0){
                    hW++;
                    hostWin.text = hW.ToString();
                }
                else{
                    cW++;
                    clientWin.text = cW.ToString();
                }
                
                c1.gameObject.SetActive(false);
                
                if(compareTypes(c1.type, c2.type))
                    powerSlots[i].SetActive(false);
            }

            // Pause before the next matchup
            yield return new WaitForSeconds(2.5f); // Adjust the duration as needed
        }
        
        hostWin.gameObject.SetActive(false);
        clientWin.gameObject.SetActive(false);
        if(hW == 5 || cW == 5){
            deckOverlay.SetActive(false);
            discardOverlay.SetActive(false);
            disconnectButton.gameObject.SetActive(true);
            if(hW == 5){
                if(IsHost){
                    winScreen.gameObject.SetActive(true);
                }
                else{
                    loseScreen.gameObject.SetActive(true);
                }
            }
            else if(cW == 5){
                if(IsHost){
                    loseScreen.gameObject.SetActive(true);
                }
                else{
                    winScreen.gameObject.SetActive(true);
                }
            }
        }
        else{
            playerOne.SetActive(true);
        }
        DestroyAllSharedCards();
        


    }

    private bool compareTypes(string t1, string t2){
        if(t1 == "Heat" && t2 == "Pressure"){
            return true;
        }
        if(t1 == "Pressure" && t2 == "Electrical"){
            return true;
        }
        if(t1 == "Electrical" && t2 == "Heat"){
            return true;
        }

        return false;
    }

    private void DisplaySharedCard(Card card, ulong sourceClientId)
    {
        Debug.Log($"Displaying shared card: {card.cardName}");

        // Create a copy of the card game object
        GameObject cardCopy = Instantiate(card.gameObject);
        cardCopy.SetActive(true); // Ensure the copied card is active

        sharedCardCopies.Add(cardCopy);
        // Get the Card component from the copied game object
        Card copiedCard = cardCopy.GetComponent<Card>();

        // Update the display position based on client or host
        float xOffset = 5 - (cardDisplayOrder * 2.5f) - 2.5f;

        if (IsHost && sourceClientId == NetworkManager.Singleton.LocalClientId)
        {
            // Host is displaying its own shared card
            cardCopy.transform.position = new Vector3(2.5f - xOffset, -3, 0); // Host's position for its own card
        }
        else if (IsHost && sourceClientId != NetworkManager.Singleton.LocalClientId)
        {
            // Host is displaying a card shared by a client
            cardCopy.transform.position = new Vector3(2.5f - xOffset, 3, 0); // Host's position for a client-shared card
        }
        else if (IsClient && sourceClientId == NetworkManager.Singleton.LocalClientId)
        {
            // Client is displaying its own shared card
            cardCopy.transform.position = new Vector3(2.5f - xOffset, -3, 0); // Client's position for its own card
        }
        else if (IsClient && sourceClientId != NetworkManager.Singleton.LocalClientId)
        {
            // Client is displaying a card shared by the host
            cardCopy.transform.position = new Vector3(2.5f - xOffset, 3, 0); // Client's position for a host-shared card
        }

        // If needed, update other properties of the copied card
        copiedCard.hasBeenPlayed = true;
        copiedCard.id = card.id;
        copiedCard.cardName = card.cardName;
        copiedCard.cost = card.cost;
        copiedCard.power = card.power;
        copiedCard.sourceClientId = sourceClientId;

        // Optionally, parent the cardCopy to a specific container or UI element
        // cardCopy.transform.SetParent(transform, worldPositionStays: true);

        cardDisplayOrder++; // Increment the order for the next card

        // Reset the display order if you want to start over at some point
        if (cardDisplayOrder >= 1)
        {
            cardDisplayOrder = 0;
        }
    }

    public void DestroyAllSharedCards()
    {
        foreach (var slot in powerSlots)
        {
            if (slot != null)
            {
                slot.SetActive(false);
            }
        }
        
        foreach (var card in sharedCardCopies)
        {
            if (card != null)
            {
                Destroy(card);
            }
        }
        sharedCardCopies.Clear();
        displayedCards.Clear();
        Debug.Log("All shared cards have been destroyed.");
    }


}