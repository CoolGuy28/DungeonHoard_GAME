using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MainUIPanel : MonoBehaviour
{

    [SerializeField] private GameObject charUIObject;
    private void OnEnable()
    {
        foreach (Transform t in transform.GetChild(0))
            Destroy(t.gameObject);
        foreach (CharacterData c in GameManager.instance.party)
        {
            GameObject newCharUIObject = Instantiate(charUIObject, transform.GetChild(0));
            newCharUIObject.transform.GetChild(0).GetComponent<Image>().sprite = c.unit.charPortrait;
            newCharUIObject.transform.GetChild(1).GetComponent<Slider>().maxValue = c.currentStats.maxHealth;
            newCharUIObject.transform.GetChild(1).GetComponent<Slider>().value = c.currentHealth;
            newCharUIObject.transform.GetChild(1).GetChild(3).GetComponent<TMP_Text>().text = c.currentHealth.ToString();
            newCharUIObject.transform.GetChild(2).GetComponent<TMP_Text>().text = c.unit.name.ToString();
        }
    }

}
