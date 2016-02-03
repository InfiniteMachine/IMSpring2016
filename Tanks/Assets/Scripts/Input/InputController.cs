using UnityEngine;

public class InputController : MonoBehaviour
{
	public enum Axis { MOVE = 0, AIM_X, AIM_Y, AIM_ANGLE }
    private float[] axis;

    public enum Buttons { JUMP = 0, FIRE, CHARGE, SPECIAL_FIRE, SPECIAL_DEFENSE, DASH_DOWN }
    public enum State { RELEASED, PRESSED, CLEARED }
    private State[] buttons;
    
    private Controller controller;
    private bool useKeyboard = false;
    void Start()
    {
        Cursor.visible = false;
        axis = new float[System.Enum.GetNames(typeof(Axis)).Length];
        for (int i = 0; i < axis.Length; i++)
            axis[i] = 0;
        buttons = new State[System.Enum.GetNames(typeof(Buttons)).Length];
        for (int i = 0; i < buttons.Length; i++)
            buttons[i] = State.RELEASED;
        if(GetComponent<PlayerController>().controllerNumber == -1 || ControllerPool.GetInstance() == null)
            useKeyboard = true;
        else
            controller = ControllerPool.GetInstance().GetController(GetComponent<PlayerController>().controllerNumber);
    }

    // Update is called once per frame
    void Update()
    {
        if (!useKeyboard)
            GamePadControl();
        else
            KeyboardControl();
    }

    public float GetAxis(Axis a)
    {
        return axis[(int)a];
    }

    public float GetAxisRaw(Axis a)
    {
        return axis[(int)a];
    }

    public bool GetButton(Buttons b)
    {
        return buttons[(int)b] == State.PRESSED;
    }

    public void ClearButton(Buttons b)
    {
        if (buttons[(int)b] == State.PRESSED)
            buttons[(int)b] = State.CLEARED;
    }

    public void ResetAxes()
    {
        for(int i = 0; i < axis.Length; i++){
            axis[i] = 0;
        }
    }

    public void ResetButtons()
    {
        for (int i = 0; i < buttons.Length; i++){
            buttons[i] = State.RELEASED;
        }
    }

    private void GamePadControl()
    {
        axis[(int)Axis.MOVE] = controller.GetAxis(0);
        axis[(int)Axis.AIM_X] = controller.GetAxis(3);
        axis[(int)Axis.AIM_Y] = Mathf.Max(-controller.GetAxis(4), 0);
        axis[(int)Axis.AIM_ANGLE] = Mathf.Atan2(axis[(int)Axis.AIM_Y], axis[(int)Axis.AIM_X]) * Mathf.Rad2Deg;

        if (buttons[(int)Buttons.JUMP] == State.RELEASED && controller.GetButton(0))
        {
            buttons[(int)Buttons.JUMP] = State.PRESSED;
        }
        else if (buttons[(int)Buttons.JUMP] != State.RELEASED && !controller.GetButton(0))
        {
            buttons[(int)Buttons.JUMP] = State.RELEASED;
        }

        if (buttons[(int)Buttons.SPECIAL_DEFENSE] == State.RELEASED && controller.GetButton(4))
        {
            buttons[(int)Buttons.SPECIAL_DEFENSE] = State.PRESSED;
        }
        else if (buttons[(int)Buttons.SPECIAL_DEFENSE] != State.RELEASED && !controller.GetButton(4))
        {
            buttons[(int)Buttons.SPECIAL_DEFENSE] = State.RELEASED;
        }

        if (buttons[(int)Buttons.SPECIAL_FIRE] == State.RELEASED && controller.GetButton(5))
        {
            buttons[(int)Buttons.SPECIAL_FIRE] = State.PRESSED;
        }
        else if (buttons[(int)Buttons.SPECIAL_FIRE] != State.RELEASED && !controller.GetButton(5))
        {
            buttons[(int)Buttons.SPECIAL_FIRE] = State.RELEASED;
        }

        if (buttons[(int)Buttons.DASH_DOWN] == State.RELEASED && controller.GetButton(1))
        {
            buttons[(int)Buttons.DASH_DOWN] = State.PRESSED;
        }
        else if (buttons[(int)Buttons.DASH_DOWN] != State.RELEASED && !controller.GetButton(1))
        {
            buttons[(int)Buttons.DASH_DOWN] = State.RELEASED;
        }

        if (buttons[(int)Buttons.FIRE] == State.RELEASED && controller.GetAxis(2) < 0)
        {
            buttons[(int)Buttons.FIRE] = State.PRESSED;
        }
        else if (buttons[(int)Buttons.FIRE] != State.RELEASED && !(controller.GetAxis(2) < 0))
        {
            buttons[(int)Buttons.FIRE] = State.RELEASED;
        }
    }

    private void KeyboardControl()
    {
        axis[(int)Axis.MOVE] = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
        axis[(int)Axis.AIM_Y] = (Input.GetKey(KeyCode.S) ? 1 : 0) - (Input.GetKey(KeyCode.W) ? 1 : 0);
        

        if (buttons[(int)Buttons.JUMP] == State.RELEASED && Input.GetKey(KeyCode.LeftShift))
        {
            buttons[(int)Buttons.JUMP] = State.PRESSED;
        }
        else if (buttons[(int)Buttons.JUMP] != State.RELEASED && !Input.GetKey(KeyCode.LeftShift))
        {
            buttons[(int)Buttons.JUMP] = State.RELEASED;
        }

        if (buttons[(int)Buttons.SPECIAL_DEFENSE] == State.RELEASED && Input.GetKey(KeyCode.Q))
        {
            buttons[(int)Buttons.SPECIAL_DEFENSE] = State.PRESSED;
        }
        else if (buttons[(int)Buttons.SPECIAL_DEFENSE] != State.RELEASED && !Input.GetKey(KeyCode.Q))
        {
            buttons[(int)Buttons.SPECIAL_DEFENSE] = State.RELEASED;
        }

        if (buttons[(int)Buttons.SPECIAL_FIRE] == State.RELEASED && Input.GetKey(KeyCode.E))
        {
            buttons[(int)Buttons.SPECIAL_FIRE] = State.PRESSED;
        }
        else if (buttons[(int)Buttons.SPECIAL_FIRE] != State.RELEASED && !Input.GetKey(KeyCode.E))
        {
            buttons[(int)Buttons.SPECIAL_FIRE] = State.RELEASED;
        }
    }
}