using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class Item : MonoBehaviour
{
    [SerializeField] string name = "";
    [SerializeField] GameObject destroyEffect;

    Slot currentSlot;
    RectTransform rectTransform;
    Canvas canvas;
    bool isDraggable = false;
    bool moveToTarget = false;
    float speed = 2000f;
    bool destroy = false;
    float timerDestroy = 0f;
    float timeDestroy = 0.3f;
    Animator anim;
    bool animationPlay = false;
    
    
    void Start()
    {
        canvas = LevelManager.main.GetCanvas();
        rectTransform = GetComponent<RectTransform>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (currentSlot != null && moveToTarget)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentSlot.transform.position, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, currentSlot.transform.position) < 0.001f)
            {
                moveToTarget = false;
                anim.SetBool("Placed", true);
                animationPlay = true;
                UIManager.main.PlayThrowEffect();
            }
        }
        
        if (animationPlay)
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        
            if (stateInfo.IsName("PlacedItem") && stateInfo.normalizedTime >= 1.0f)
            {
                anim.SetBool("Placed", false);
                animationPlay = false;
            }
        }

        if (destroy)
        {
            timerDestroy += Time.deltaTime;

            if (timerDestroy >= timeDestroy)
            {
                if (destroyEffect != null)
                {
                    Instantiate(destroyEffect, transform.position, Quaternion.identity);
                }
                
                destroy = false;
                Destroy(gameObject);
            }
        }
    }

    public bool IsAnimationPlaying(string animationName) 
    {        
        var animatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.IsName(animationName))
        {
            return true;
        }
        
        return false;
    }

    public void FindEmptySlot()
    {
        RemoveCurrentSlot();

        var emptySlots = GridManager.main.GetAllSlots().Where(slot => slot.IsEmpty()).ToArray();

        isDraggable = false;
        moveToTarget = true;
        
        if (emptySlots.Length == 0 && canvas == null) return;

        Slot nearestSlot = null;
        float minDistance = float.MaxValue;
        Vector2 mousePosition = Input.mousePosition;

        foreach (Slot slot in emptySlots)
        {
            Vector2 slotScreenPosition = Camera.main.WorldToScreenPoint(
                slot.GetRectTransform().position
            );
            
            float distance = Vector2.Distance(mousePosition, slotScreenPosition);
            
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestSlot = slot;
            }
        }

        if (nearestSlot != null)
        {
            SetCurrentSlot(nearestSlot);
            nearestSlot.SetCurrentItem(this);
        }
    }

    public void SetCurrentSlot(Slot _slot)
    {
        currentSlot = _slot;
    }

    public Slot GetCurrentSlot()
    {
        return currentSlot;
    }

    public string GetName()
    {
        return name;
    }

    public void SetName(string _name)
    {
        name = _name;
    }

    public string GetNameItem()
    {
        return name;
    }

    public void RemoveCurrentSlot()
    {
        if (currentSlot != null)
        {
            currentSlot.RemoveCurrentItem();
            currentSlot = null;
        }
    }

    public void SetDraggableStatus()
    {
        isDraggable = true;
    }

    public void DestroyItem()
    {
        destroy = true;
    }
}
