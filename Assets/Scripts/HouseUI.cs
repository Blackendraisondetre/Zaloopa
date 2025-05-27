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

    public void SetState(HouseState newState)
    {
        state = newState;
        UpdateVisual();
    }

    void UpdateVisual()
    {
        switch (state)
        {
            case HouseState.Neutral:
                houseImage.color = neutralColor;
                break;
            case HouseState.Brainwashed:
                houseImage.color = brainwashedColor;
                break;
            case HouseState.Dead:
                houseImage.color = deadColor;
                break;
        }
    }

    // public void OnClick()
    // {
    //     GameManager.Instance.OnHouseClicked(this);
    // }
}
