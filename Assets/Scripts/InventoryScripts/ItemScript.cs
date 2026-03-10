using UnityEngine;

[CreateAssetMenu(menuName = "inventory/item")]
public class Item : ScriptableObject
{
    public int id;
    public string item_name;
    public int max_stack;

    public Sprite icon;
    public GameObject prefab;
    //public string action;
}

[System.Serializable]
public class ItemInstance
{
    [SerializeField] public Item itemData;
    [SerializeField] public int price;
}

//public class ItemContainer : MonoBehaviour
//{
//    [SerializeField] public ItemInstance item;
//    [SerializeField] public int amount = 1;

//    public void pickUp(int remaining)
//    {
//        if (remaining > 0)
//        {
//            amount = remaining;
//        }
//        else Destroy(gameObject);
//    }
//}
//public class ItemScript : MonoBehaviour
//{

//}
