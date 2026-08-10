using System;
using System.Collections.Generic;
using UnityEngine;

public class HintsTrigger : MonoBehaviour
{
    [SerializeField] private bool showHintOnlyBeforeFirstActivate;
    
    public HashSet<GameObject> charactersInTrigger = new ();
    
    private event Action TriggerEnter;
    private event Action TriggerExit;
    
    private bool _btnFirstActivated;

    public bool btnFirstActivated
    {
        get => _btnFirstActivated;
        set
        {
            _btnFirstActivated = value;
            if (_btnFirstActivated)
                TriggerExit?.Invoke();
        }
    }

    public void Initialize(Action TriggerEnter, Action TriggerExit)
    {
        this.TriggerEnter = TriggerEnter;
        this.TriggerExit = TriggerExit;
    }

    private void Start()
    {
        G.characters.CharacterChange += OnCharacterChanged;
    }
    private void OnDestroy()
    {
        G.characters.CharacterChange -= OnCharacterChanged;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!(showHintOnlyBeforeFirstActivate && btnFirstActivated))
        {
            charactersInTrigger.Add(collision.gameObject);
            if (collision.gameObject == G.characters.currentCharacter)
                TriggerEnter?.Invoke();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        charactersInTrigger.Remove(collision.gameObject);
        if (collision.gameObject == G.characters.currentCharacter)
            TriggerExit?.Invoke();
    }

    private void OnCharacterChanged(GameObject character)
    {
        if (!(showHintOnlyBeforeFirstActivate && btnFirstActivated))
        {
            if (charactersInTrigger.Contains(character))
                TriggerEnter?.Invoke();
            else
                TriggerExit?.Invoke();
        }
    }
}
