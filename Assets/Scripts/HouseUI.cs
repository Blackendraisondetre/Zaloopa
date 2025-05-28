using UnityEngine;
using UnityEngine.UI;

public enum HouseState { Neutral, Brainwashed, Dead }

public class HouseUI : MonoBehaviour
{
    public HouseState state = HouseState.Neutral;
    public Image houseImage;

    public Color neutralColor;
    public Color brainwashedColor;
    public Color deadColor;

  
    void Awake()
    {
        ColorUtility.TryParseHtmlString("#7FFFD4", out neutralColor);       // Aquamarine
        ColorUtility.TryParseHtmlString("#808080", out deadColor);          // Gray
        ColorUtility.TryParseHtmlString("#B03060", out brainwashedColor);   // Maroon/Pinkish red
    }
    
    void Start()
    {
        UpdateVisual();
    }
    void Update(){

    }
    public void SetState(HouseState newState)
    {
        state = newState;
        UpdateVisual();
    }

   public void UpdateVisual()
    {
        switch (state)
        {
            case HouseState.Neutral:
                houseImage.color = neutralColor;
                Debug.Log("neurtal");
                break;
            case HouseState.Brainwashed:
                houseImage.color = brainwashedColor;
                                Debug.Log("Brainwashed");

                break;
            case HouseState.Dead:
                houseImage.color = deadColor;
                                Debug.Log("dead");

                break;
        }
      
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        UpdateVisual();
    }
#endif
    // public void OnClick()
    // {
    //     GameManager.Instance.OnHouseClicked(this);
    // }
}
