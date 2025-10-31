using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MainUIPanel : MonoBehaviour
{
    [SerializeField] private GameObject charUIObject;
    [SerializeField] private GameObject conditionUIPrefab;
    [SerializeField] private List<GameObject> charUI;
    private int selectedIndex;
    private void OnEnable()
    {
        selectedIndex = 0;
        foreach (Transform t in transform.GetChild(0))
            Destroy(t.gameObject);
        charUI.Clear();
        foreach (CharacterData c in GameManager.instance.party)
        {
            GameObject newCharUIObject = Instantiate(charUIObject, transform.GetChild(0));
            SetCharUIObj(newCharUIObject, c);
            charUI.Add(newCharUIObject);
        }
        SelectCharUIObj();
    }

    public void AdjustUIValues()
    {
        SetCharUIObj(charUI[selectedIndex], GameManager.instance.party[selectedIndex]);
    }

    private void SetCharUIObj(GameObject newCharUIObject, CharacterData c)
    {
        newCharUIObject.transform.GetChild(0).GetComponent<Image>().sprite = c.unit.charPortrait;
        newCharUIObject.transform.GetChild(1).GetComponent<Slider>().maxValue = c.currentStats.maxHealth;
        newCharUIObject.transform.GetChild(1).GetComponent<Slider>().value = c.currentHealth;
        newCharUIObject.transform.GetChild(1).GetChild(3).GetComponent<TMP_Text>().text = c.currentHealth.ToString();
        newCharUIObject.transform.GetChild(2).GetComponent<TMP_Text>().text = c.unit.name.ToString();
        foreach (Transform child in newCharUIObject.transform.GetChild(3))
        {
            Destroy(child.gameObject);
        }
        foreach (ConditionStats condition in c.conditions)
        {
            GameObject newConditionUI = Instantiate(conditionUIPrefab, Vector3.zero, Quaternion.identity, newCharUIObject.transform.GetChild(3));
            if (condition.condition.sprite != null)
            {
                newConditionUI.GetComponent<Image>().sprite = condition.condition.GetSprite(condition.level);
            }
            newConditionUI.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
        }
        //charUI[selectedIndex].transform.GetChild(4).gameObject.SetActive(true);
    }

    public void SelectNewChar(int dir)
    {
        DeselectCharUIObj();
        selectedIndex += dir;
        if (selectedIndex >= charUI.Count)
            selectedIndex = 0;
        if (selectedIndex < 0)
            selectedIndex = charUI.Count - 1;
        SelectCharUIObj();
    }

    private void SelectCharUIObj()
    {
        charUI[selectedIndex].transform.GetChild(4).gameObject.SetActive(false);
    }

    private void DeselectCharUIObj()
    {
        charUI[selectedIndex].transform.GetChild(4).gameObject.SetActive(true);
    }

    public CharacterData GetSelectCharUIObj()
    {
        return GameManager.instance.party[selectedIndex];
    }
}
