using UnityEngine;

public class ItemContainerScript : MonoBehaviour
{
        [SerializeField] public ItemInstance item;
        [SerializeField] public int amount = 1;

        public void pickUp(int remaining)
        {
            if (remaining > 0)
            {
                amount = remaining;
            }
            else Destroy(gameObject);
        }

}
