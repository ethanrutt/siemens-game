using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class CardSharingManager : NetworkBehaviour
{
    [SerializeField] private string buttonPath = "PlayerOne/PrefabCanvas/PlayTurn";
    [SerializeField] private Button confirmButton;

    private Dictionary<int, Card> cardLookup = new Dictionary<int, Card>();
    private List<CardData> selectedCards = new List<CardData>();
    private NetworkList<CardData> sharedCards;
    private HashSet<int> displayedCards = new HashSet<int>();
    private int cardDisplayOrder = 0;
    private int updateCount = 0;

    public Card[] allCards;

    

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

        Debug.Log($"Selected cards shared with all clients by client {clientId}.");
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
                }
                else
                {
                    Debug.LogWarning($"Card with ID {cardData.id} not found in lookup table.");
                }
            //}
        }

        if(lastCard != null && updateCount % 2 == 0){
            DisplaySharedCard(lastCard, lastCardData.clientId);
            displayedCards.Add(lastCardData.id);
        }
    }

    private void DisplaySharedCard(Card card, ulong sourceClientId)
    {
        Debug.Log($"Displaying shared card: {card.cardName}");

        // Create a copy of the card game object
        GameObject cardCopy = Instantiate(card.gameObject);
        cardCopy.SetActive(true); // Ensure the copied card is active

        // Get the Card component from the copied game object
        Card copiedCard = cardCopy.GetComponent<Card>();

        // Update the display position based on client or host
        float xOffset = 5 - (cardDisplayOrder * 2.5f);

        if (IsHost && sourceClientId == NetworkManager.Singleton.LocalClientId)
        {
            // Host is displaying its own shared card
            cardCopy.transform.position = new Vector3(-1 - xOffset, 3, 0); // Host's position for its own card
        }
        else if (IsHost && sourceClientId != NetworkManager.Singleton.LocalClientId)
        {
            // Host is displaying a card shared by a client
            cardCopy.transform.position = new Vector3(6 - xOffset, 3, 0); // Host's position for a client-shared card
        }
        else if (IsClient && sourceClientId == NetworkManager.Singleton.LocalClientId)
        {
            // Client is displaying its own shared card
            cardCopy.transform.position = new Vector3(-1 - xOffset, 3, 0); // Client's position for its own card
        }
        else if (IsClient && sourceClientId != NetworkManager.Singleton.LocalClientId)
        {
            // Client is displaying a card shared by the host
            cardCopy.transform.position = new Vector3(6 - xOffset, 3, 0); // Client's position for a host-shared card
        }

        // If needed, update other properties of the copied card
        copiedCard.hasBeenPlayed = true;
        copiedCard.id = card.id;
        copiedCard.cardName = card.cardName;
        copiedCard.cost = card.cost;
        copiedCard.power = card.power;

        // Optionally, parent the cardCopy to a specific container or UI element
        // cardCopy.transform.SetParent(transform, worldPositionStays: true);

        cardDisplayOrder++; // Increment the order for the next card

        // Reset the display order if you want to start over at some point
        if (cardDisplayOrder >= 3)
        {
            cardDisplayOrder = 0;
        }
    }
}