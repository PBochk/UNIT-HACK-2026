using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeNodeExample : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    public Upgrade Upgrade;
    private Upgrade Required;

    public void Start()
    {
        if (Upgrade.RequiredUpgrade != null)
        {
            gameObject.SetActive(false);
            Required = Upgrade.RequiredUpgrade;
            Required.OnUpgradeGot += () => gameObject.SetActive(true);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Click!");
        if (Upgrade.UpgradeValues.Count <= Upgrade.Level) return;
        Upgrade.AddLevel();       
        Debug.Log(Upgrade.Level);
    }
}