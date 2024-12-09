using System;
using System.Collections;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class Card : MonoBehaviour
{
    private Vector3 originalPosition;
    public bool hasBeenPlayed;
    public bool isInSlot;
    public bool ignoreExit = false;
    public int handIndex;
    public int playIndex;

    public int id;
    public string cardName;
    public string type;
    public int cost;
    public int power;
    public ulong sourceClientId;

    private GameManager2 gm;
    private CardSharingManager cardSharingManager;
    private int clickCount;
    private static Card selectedCard = null;
    public GameObject nameLabel;
    public TextMeshPro cName;

    private void Start()
    {
        gm = FindObjectOfType<GameManager2>();
        //cardSharingManager = FindObjectOfType<CardSharingManager>();
        cName = nameLabel.GetComponent<TextMeshPro>();
        //nameLabel.text = cardName;
        //nameLabel.enabled = false;
        nameLabel.SetActive(false);
        StartCoroutine(FindCardSharingManager());
    }

    private void OnMouseDown()
    {
        // Reset the previously selected card if it's not the current card
        if (selectedCard != null && selectedCard != this)
        {
            selectedCard.ResetCardState();
        }

        // Mark the current card as the selected card
        selectedCard = this;

        // Continue with your existing logic
        if (!hasBeenPlayed && clickCount == 0)
        {
            originalPosition = transform.position;
            transform.position += new Vector3(0, 0.05f, -4);

            Vector3 labelOffset = new Vector3(95f + (handIndex * 65f), 0, 0);
            if (cName != null)
            {
                cName.text = cardName;
            }

            if (isInSlot)
                nameLabel.transform.position = transform.position + new Vector3(0, 1.4f, -1f);
            else
                nameLabel.transform.position = transform.position + new Vector3(0, 1.4f, -3f);

            nameLabel.SetActive(true);
            clickCount++;
        }
        else if (hasBeenPlayed == false && clickCount == 1)
        {
            if (isInSlot)
            {
                selectedCard.ResetCardState();
                gm.UndoPlaySlot(this);
                originalPosition = transform.position;
                cardSharingManager.DeselectCard(this); // Deselect from sharing
            }
            else
            {
                selectedCard.ResetCardState();
                gm.MoveToPlaySlot(this);
                originalPosition = transform.position;
                cardSharingManager.SelectCard(this); // Select for sharing
            }
            ignoreExit = true;
            StartCoroutine(ResetIgnoreMouseExit());
            nameLabel.SetActive(false);
        }
    }

    public void ResetCardState()
    {
        // Reset the clickCount
        clickCount = 0;

        // Move the card back to its original position
        transform.position = originalPosition;

        // Hide the name label
        nameLabel.SetActive(false);
    }

    public CardData GetCardData()
    {
        return new CardData(id, cost, power, sourceClientId);
    }

    private IEnumerator ResetIgnoreMouseExit()
    {
        yield return new WaitForSeconds(0.1f);
        ignoreExit = false;
    }

    private IEnumerator FindCardSharingManager()
    {
        // Wait until CardSharingManager is spawned and connected to the network
        while (cardSharingManager == null)
        {
            //Debug.Log("cardsharing activated.");
            cardSharingManager = FindObjectOfType<CardSharingManager>();
            yield return null;
        }
    }
}

[Serializable]
public struct CardData : INetworkSerializable, IEquatable<CardData>
{
    public int id;
    public int cost;
    public int power;
    public ulong clientId; // Add clientId field to track the source of the card data

    public CardData(int id, int cost, int power, ulong clientId)
    {
        this.id = id;
        this.cost = cost;
        this.power = power;
        this.clientId = clientId;
    }

    // Implement IEquatable<CardData>
    public bool Equals(CardData other)
    {
        return id == other.id && cost == other.cost && power == other.power && clientId == other.clientId;
    }

    public override bool Equals(object obj)
    {
        return obj is CardData other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + id.GetHashCode();
            hash = hash * 23 + cost.GetHashCode();
            hash = hash * 23 + power.GetHashCode();
            hash = hash * 23 + clientId.GetHashCode();
            return hash;
        }
    }

    public static bool operator ==(CardData left, CardData right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CardData left, CardData right)
    {
        return !(left == right);
    }

    // Implement INetworkSerializable
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref id);
        serializer.SerializeValue(ref cost);
        serializer.SerializeValue(ref power);
        serializer.SerializeValue(ref clientId); // Serialize clientId
    }
}