using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Controller : MonoBehaviour
{
    
    public static Controller Instance { get; private set; }

    [SerializeField]
    private List<ButtonPressSequence> winButtonPressSequences;

    [SerializeField]
    private SequenceCondition[] sequences;

    [SerializeField]
    private int firstIncorrectIndexToLose;

    private List<Buttons> clickedButtonSequence = new List<Buttons>();

    private ControllerState controllerState;


    private void Awake()
    {
        Instance = this;
    }

    public void ButtonClicked(Buttons button)
    {
        if (controllerState != ControllerState.AwaitingInput)
            return;

        controllerState = ControllerState.InSequence;

        clickedButtonSequence.Add(button);

        foreach (var sequenceCondition in sequences)
        {
            if (sequenceCondition.buttonIdsSequencesToExecute.Any(seq => clickedButtonSequence.ToArray().SequenceEqual(seq.sequence)))
            {
                sequenceCondition.execute.StartSequence();
                break;
            }
        }

        int i = clickedButtonSequence.Count - 1;

        if (winButtonPressSequences.Any(seq => clickedButtonSequence.ToArray().SequenceEqual(seq.sequence)))
        {
            GameManager.instance.Win();
        }    
    }

    public void CompletedSequence()
    {
        if (controllerState != ControllerState.InSequence)
            return;

        controllerState = ControllerState.AwaitingInput;
    }
    
    public void ResetButtonSequence()
    {
        clickedButtonSequence.Clear();
    }

    public void UndoButtonPress()
    {
        clickedButtonSequence.RemoveAt(clickedButtonSequence.Count - 1);
    }

}

public enum ControllerState
{
    AwaitingInput,
    InSequence
}

[System.Serializable]
public class SequenceCondition
{
    public List<ButtonPressSequence> buttonIdsSequencesToExecute = new List<ButtonPressSequence>();

    public Sequencer execute;
}

[System.Serializable]
public class ButtonPressSequence
{
    public Buttons[] sequence;
}

public enum Buttons
{
    Green,
    Yellow,
    Red,
    Blue,
    Orange,
    Pink
}