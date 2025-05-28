using UnityEngine;
using UnityEngine.UI;

public enum HouseState { Neutral, Brainwashed, Dead }

public class HouseUI : MonoBehaviour
{
    public HouseState state = HouseState.Neutral;
    public Image houseImage;

    public Color neutralColor = Color.blue;
    public Color brainwashedColor = Color.red;
    public Color deadColor = Color.black;
    
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
