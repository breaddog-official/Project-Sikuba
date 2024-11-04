using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsEventsTransmitter : MonoBehaviour
{
    [SerializeField] private bool useComponents;
    [ShowIf(nameof(useComponents))]
    [SerializeField] private Component[] components;
    [Space]
    [SerializeField] private bool useGameObjects;
    [ShowIf(nameof(useGameObjects))]
    [SerializeField] private GameObject[] gameObjects;


    public IReadOnlyCollection<Component> Components => useComponents ? components : componentsPlug;
    public IReadOnlyCollection<GameObject> GameObjects => useGameObjects ? gameObjects : gameObjectsPlug;

    private readonly Component[] componentsPlug = new Component[0];
    private readonly GameObject[] gameObjectsPlug = new GameObject[0];



    private void OnTriggerEnter(Collider other)
    {
        foreach (Component col in Components)
        {
            col.SendMessage(nameof(OnTriggerEnter), other, SendMessageOptions.DontRequireReceiver);
        }
        foreach (GameObject obj in GameObjects)
        {
            obj.SendMessage(nameof(OnTriggerEnter), other, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        foreach (Component col in Components)
        {
            col.SendMessage(nameof(OnTriggerStay), other, SendMessageOptions.DontRequireReceiver);
        }
        foreach (GameObject obj in GameObjects)
        {
            obj.SendMessage(nameof(OnTriggerStay), other, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (Component col in Components)
        {
            col.SendMessage(nameof(OnTriggerExit), other, SendMessageOptions.DontRequireReceiver);
        }
        foreach (GameObject obj in GameObjects)
        {
            obj.SendMessage(nameof(OnTriggerExit), other, SendMessageOptions.DontRequireReceiver);
        }
    }




    private void OnCollisionEnter(Collision collision)
    {
        foreach (Component col in Components)
        {
            col.SendMessage(nameof(OnCollisionEnter), collision, SendMessageOptions.DontRequireReceiver);
        }
        foreach (GameObject obj in GameObjects)
        {
            obj.SendMessage(nameof(OnCollisionEnter), collision, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (Component col in Components)
        {
            col.SendMessage(nameof(OnCollisionStay), collision, SendMessageOptions.DontRequireReceiver);
        }
        foreach (GameObject obj in GameObjects)
        {
            obj.SendMessage(nameof(OnCollisionStay), collision, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        foreach (Component col in Components)
        {
            col.SendMessage(nameof(OnCollisionExit), collision, SendMessageOptions.DontRequireReceiver);
        }
        foreach (GameObject obj in GameObjects)
        {
            obj.SendMessage(nameof(OnCollisionExit), collision, SendMessageOptions.DontRequireReceiver);
        }
    }
}
