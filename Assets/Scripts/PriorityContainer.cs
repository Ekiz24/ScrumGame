using UnityEngine;
using UnityEngine.EventSystems;

public class PriorityContainer : MonoBehaviour, IDropHandler
{
    public string priorityLevel; // 容器的优先级："高优先级"、"中优先级"、"低优先级"、"无优先级"

    public void OnDrop(PointerEventData eventData)
    {
        // 我们不在这里处理拖拽逻辑，交给DraggableItem.OnEndDrag处理即可
        // 这样可以避免两个方法竞争导致的问题
    }
}