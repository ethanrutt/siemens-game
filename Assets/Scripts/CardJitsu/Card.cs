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
    public int cost;
    public int power;
    public ulong sourceClientId;

    private GameManager2 gm;
    private CardSharingManager cardSharingManager;
    public TextMeshProUGUI nameLabel;

    private void Start()
    {
        gm = FindObjectOfType<GameManager2>();
        //cardSharingManager = FindObjectOfType<CardSharingManager>();
        nameLabel.text = cardName;
        nameLabel.enabled = false;
        StartCoroutine(FindCardSharingManager());
    }

    private void OnMouseDown()
    {
        if (hasBeenPlayed == false)
        {
            if (isInSlot)
            {
                gm.UndoPlaySlot(this);
                cardSharingManager.DeselectCard(this); // Deselect from sharing
            }
            else
            {
                gm.MoveToPlaySlot(this);
                cardSharingManager.SelectCard(this); // Select for sharing
            }
            ignoreExit = true;
            StartCoroutine(ResetIgnoreMouseExit());
            nameLabel.enabled = false;
        }
    }

    private void OnMouseEnter()
    {
        if (!hasBeenPlayed)
        {
            originalPosition = transform.position;
            transform.position += new Vector3(0, 0.05f, -4);
            Vector3 labelOffset = new Vector3(95f + (handIndex * 65f), 0, 0);
            nameLabel.transform.position = labelOffset;
            nameLabel.enabled = true;
        }
    }

    private void OnMouseExit()
    {
        if (!hasBeenPlayed && !ignoreExit)
        {
            transform.position = originalPosition;
            nameLabel.enabled = false;
        }
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