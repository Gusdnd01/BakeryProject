using UnityEngine;

public class ItemObjectTrigger : MonoBehaviour
{
    [Header("Feedbacks")]
    [SerializeField] private FeedbackPlayer _addFeedback;

    private ItemObject _itemObject;

    private void Awake()
    {
        _itemObject = transform.parent.GetComponent<ItemObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Player>(out Player player))
        {
            _addFeedback.PlayFeedback();
            //player.onPickUpItem?.Invoke();
            _itemObject.PickUpItem();
        }
    }
}
