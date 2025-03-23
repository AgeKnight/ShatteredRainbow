using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Events;


public class ButtonPress : Button
{
    GameObject selectImage;
    GameObject temp;
    protected override void Awake() 
    {
        base.Awake();
        if(this.gameObject.transform.childCount>0)
            selectImage = this.gameObject.transform.GetChild(0).gameObject;
        onClick.AddListener(() => {DoStateTransition(SelectionState.Highlighted, false); });
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (interactable)
        {
            if(FindObjectOfType<GameManager>())
                temp = GameManager.Instance.AudioPlay(GameManager.Instance.MenuSound[2],true);
            else
            {
                temp = Instantiate(TitleManager.Instance.SelectSound.gameObject);
                TitleManager.Instance.SelectSound.Play();
            }            
			selectImage.SetActive(true);
		}
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (interactable)
        {
            Destroy(temp);
			selectImage.SetActive(false);
		}
    }
}
