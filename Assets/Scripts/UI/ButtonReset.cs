using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class ButtonReset : Button
{
    GameObject selectImage;
    GameObject temp;
    GameObject Hint;
    protected override void Awake()
    {
        base.Awake();
        if (this.gameObject.transform.childCount > 0)
        {
            selectImage = this.gameObject.transform.GetChild(0).gameObject;
            Hint = this.gameObject.transform.GetChild(1).gameObject;
        }
        onClick.AddListener(() => { DoStateTransition(SelectionState.Highlighted, false); });
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (interactable)
        {
            if (FindObjectOfType<GameManager>())
                temp = GameManager.Instance.AudioPlay(GameManager.Instance.MenuSound[2], true);
            else
            {
                temp = Instantiate(TitleManager.Instance.SelectSound.gameObject);
                TitleManager.Instance.SelectSound.Play();
            }
            selectImage.SetActive(true);
            Hint.SetActive(true);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (interactable)
        {
            Destroy(temp);
            selectImage.SetActive(false);
            Hint.SetActive(false);
        }
    }
}
