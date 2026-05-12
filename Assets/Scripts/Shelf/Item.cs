using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class Item : MonoBehaviour
{
    [SerializeField] GameObject destroyEffect;
    [SerializeField] GameObject disableImage;

    public Slot currentSlot;
    bool moveToTarget = true;
    bool isDraggable = false;
    float speed = 2000f;
    bool destroy = false;
    float timerDestroy = 0f;
    float timeDestroy = 0.3f;
    bool animationPlay = false;
    public string name = "";
    public bool isBehind = false;
    float behindDifferentSize = 15f;
    float offsetBehind = 30f;
    float currentWidth;
    bool negativeIndent = false;

    RectTransform rectTransform;
    Image image;
    DraggableItem draggableItem;
    Animator anim;
        
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        draggableItem = GetComponent<DraggableItem>();
        image = GetComponent<Image>();
        anim = GetComponent<Animator>();

        currentWidth = GetComponent<RectTransform>().rect.width;
    }

    void Update()
    {
        if (currentSlot != null && moveToTarget)
        {
            Vector3 targetPos = currentSlot.transform.position;

            if (isBehind)
            {
                targetPos.x = negativeIndent ? targetPos.x - offsetBehind : targetPos.x + offsetBehind;
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.001f)
            {
                moveToTarget = false;                
                AnimationPlay("Placed", true);
                animationPlay = true;
                UIManager.main.PlayThrowEffect();
            }
        }
        
        if (animationPlay)
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        
            if (stateInfo.IsName("PlacedItem") && stateInfo.normalizedTime >= 1.0f)
            {
                AnimationPlay("Placed", false);
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
    
    public void AnimationPlay(string _name, bool _play)
    {
        anim.SetBool(_name, _play);
    }
    
    public void Initialize(Slot slot, Sprite _image, bool _isBehind)
    {
        moveToTarget = true;
        name = _image.name;
        image.sprite = _image;

        if (!_isBehind)
        {
            EnableDraggable(true);
        }
        else
        {
            int random = Random.Range(0, 2);
            negativeIndent = random == 1;

            isBehind = true;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x - behindDifferentSize, rectTransform.sizeDelta.y - behindDifferentSize);
            draggableItem.enabled = false;
        }

        SetCurrentSlot(slot);
        
        if (_isBehind)
        {
            slot.SetBehindItem(this);
        }
        else
        {
            slot.SetCurrentItem(this);
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

        var emptySlots = GridManager.main.GetAllSlots().Where(slot => slot.IsEmpty() && slot.GetBehindItemsCount() == 0).ToArray();

        isDraggable = false;
        moveToTarget = true;
        
        if (emptySlots.Length == 0) return;

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

        if (currentSlot != null)
        {
            moveToTarget = true;
        }
    }

    public Slot GetCurrentSlot()
    {
        return currentSlot;
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

    public void EnableDraggable(bool _enable)
    {
        isBehind = false;
        draggableItem.enabled = _enable;
        disableImage.SetActive(false);
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x + behindDifferentSize, rectTransform.sizeDelta.y + behindDifferentSize);
        moveToTarget = true;
    }
}
