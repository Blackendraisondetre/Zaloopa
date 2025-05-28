using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State")]
    public GameState currentState = GameState.Idle;

    [Header("Turn Tracking")]
    public int totalDays = 3;
    public int actionsPerDay = 2;
    public int currentDay = 1;
    public int actionsLeft = 2;

    [Header("Kill Limit")]
    public int killsUsed = 0;
    public int maxKills = 2;

    [Header("References")]
    public HouseUI selectedHouse;
    // Enums
    public enum GameState
    {
        Idle,           // Waiting for player input
        Dialog,         // Pigeon is speaking
        ChoosingAction, // Action cards are shown
        Executing,      // An action is being resolved
        EndDay,         // Transition between days
        GameOver        // Game has ended
    }

    public enum ActionType
    {
        Brainwash,
        Kill
    }

    // Initialization
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start() { }
    private void Update() { }

    // Called when a house is clicked
    public void OnHouseClicked(HouseUI house)
    {
        if (currentState != GameState.Idle) return;

        selectedHouse = house;
        currentState = GameState.Dialog;

        // DialogueManager.Instance.StartDialogueForHouse(house);
    }

    // Called when an action card is used
    public void ApplyAction(ActionType actionType)
    {
        if (currentState != GameState.ChoosingAction) return;
        if (selectedHouse == null) return;

        if (actionType == ActionType.Kill)
        {
            if (killsUsed >= maxKills)
            {
                Debug.Log("Kill limit reached");
                return;
            }

            selectedHouse.SetState(HouseState.Dead);
            killsUsed++;
        }
        // else if (actionType == ActionType.Brainwash)
        // {
        //   //  float chance = selectedHouse.brainwashChance;
        //    // bool success = Random.value < chance;

        //     if (success)
        //     {
        //         selectedHouse.SetState(HouseState.Brainwashed);
        //     }
        //     else
        //     {
        //         Debug.Log("Brainwash failed");
        //     }
        // }

        actionsLeft--;
        selectedHouse = null;
        currentState = GameState.Executing;

        EndAction();
    }
        
    // Called after action is resolved
    public void EndAction()
    {
        if (actionsLeft <= 0)
        {
            EndDay();
        }
        else
        {
            currentState = GameState.Idle;
        }
    }

    // Called when a day ends
    public void EndDay()
    {
        currentDay++;
        actionsLeft = actionsPerDay;
        currentState = GameState.Idle;

        Debug.Log("End of Day " + (currentDay - 1));

        if (currentDay > totalDays)
        {
            CheckWinLose();
        }

        // TODO: Notify UI to update calendar and reset state
    }

    // Used by card UI buttons
    public void OnActionCardClicked(int actionIndex)
    {
        ApplyAction((ActionType)actionIndex);
    }

    // Checks win/lose condition after last day
    public void CheckWinLose()
    {
        int supporters = 0;

        foreach (var house in FindObjectsOfType<HouseUI>())
        {
            if (house.state == HouseState.Brainwashed)
                supporters++;
        }

        if (supporters >= 3)
        {
            Debug.Log("YOU WIN!");
            currentState = GameState.GameOver;
        }
        else
        {
            Debug.Log("YOU LOSE!");
            currentState = GameState.GameOver;
        }

        // TODO: Trigger endgame UI
    }
}
