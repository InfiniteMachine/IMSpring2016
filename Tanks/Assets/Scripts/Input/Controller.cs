using UnityEngine;
using System.Collections;

[System.Serializable]
public class Controller{
    private float dead = 0.15f;
    private float[] axes;
    private int[] axisNeutral;
    private float clear = 0.25f;
    private int controllerNumber = 1;
    private bool isConnected = false;
    private bool hasSync = false;
    private string controllerName;
    private int numButtons;

    public Controller(string name, int controller)
    {
        axes = new float[ControllerPool.GetInstance().numAxes];
        for (int i = 0; i < axes.Length; i++)
        {
            axes[i] = 0;
        }
        controllerName = name;
        clear = ControllerPool.GetInstance().clearDuration;
        dead = ControllerPool.GetInstance().deadBand;
        controllerNumber = controller;

        axisNeutral = new int[axes.Length];
        numButtons = ControllerPool.GetInstance().numButtons;
    }

    public void Reinitialize(string name)
    {
        axes = new float[ControllerPool.GetInstance().numAxes];
        for (int i = 0; i < axes.Length; i++)
        {
            axes[i] = 0;
        }
        controllerName = name;
        clear = ControllerPool.GetInstance().clearDuration;
        dead = ControllerPool.GetInstance().deadBand;
        hasSync = false;
        axisNeutral = new int[axes.Length];
        numButtons = ControllerPool.GetInstance().numButtons;
    }

    public float GetAxis(int number)
    {
        if (!isConnected || !hasSync)
            return 0;
        float axisVal = 0;
        if (axisNeutral[number] == 0)
        {
            axisVal = Input.GetAxis("joy_" + (controllerNumber - 1) + "_axis_" + number);
        }
        else if (axisNeutral[number] == -1)
        {
            axisVal = (Input.GetAxis("joy_" + (controllerNumber - 1) + "_axis_" + number) + 1) / 2;
        }
        else if (axisNeutral[number] == 1)
        {
            axisVal = (Input.GetAxis("joy_" + (controllerNumber - 1) + "_axis_" + number) - 1) / 2;
        }
        if (Mathf.Abs(axisVal) < dead)
        {
            return 0;
        }
        else if (axisVal < 0)
        {
            return ((axisVal + dead) / (1 - dead));
        }
        else
        {
            return ((axisVal - dead) / (1 - dead));
        }
    }

    public bool GetAxisAsButton(int number, bool positive)
    {
        if (!isConnected || !hasSync)
            return false;
        if (axes[number] > 0)
        {
            return false;
        }
        float value = GetAxis(number);
        if ((positive && value > dead) || (!positive && value < -dead))
        {
            axes[number] = clear;
            return true;
        }
        return false;
    }

    public bool GetButtonDown(int button)
    {
        if (!isConnected || !hasSync)
            return false;
        return Input.GetKeyDown("joystick " + controllerNumber + " button " + button);
    }

    public bool GetButtonUp(int button)
    {
        if (!isConnected || !hasSync)
            return false;
        return Input.GetKeyUp("joystick " + controllerNumber + " button " + button);
    }

    public bool GetButton(int button)
    {
        if (!isConnected || !hasSync)
            return false;
        return Input.GetKey("joystick " + controllerNumber + " button " + button);
    }

    public string GetName()
    {
        return controllerName;
    }

    public int GetControllerNumber()
    {
        return controllerNumber;
    }

    public bool IsConnected()
    {
        return isConnected;
    }

    public void Reset()
    {
        for (int i = 0; i < axes.Length; i++)
        {
            axes[i] = 0;
        }
        hasSync = false;
    }

    public void Disconnected()
    {
        isConnected = false;
        hasSync = false;
        for (int i = 0; i < axes.Length; i++)
        {
            axes[i] = 0;
        }
    }

    public void Connect()
    {
        isConnected = true;
    }

    public void Update(float deltaTime)
    {
        if (!isConnected)
            return;
        if (!hasSync)
        {
            for (int i = 0; i < numButtons; i++)
            {
                if (Input.GetKey("joystick " + controllerNumber + " button " + i))
                {
                    RegenerateNeutrals();
                    return;
                }
            }
            for (int i = 0; i < axes.Length; i++)
            {
                if (Mathf.Abs(Input.GetAxis("joy_" + (controllerNumber - 1) + "_axis_" + i)) > dead)
                {
                    RegenerateNeutrals();
                    return;
                }
            }
        }
        else
        {
            for (int i = 0; i < axes.Length; i++)
            {
                if (axes[i] > 0)
                {
                    axes[i] -= deltaTime;
                    if (Mathf.Abs(Input.GetAxis("joy_" + (controllerNumber - 1) + "_axis_" + i)) < dead * 0.5f)
                        axes[i] = 0;
                }
            }
        }
    }

    private void RegenerateNeutrals()
    {
        hasSync = true;
        for (int i = 0; i < ControllerPool.GetInstance().numAxes; i++)
        {
            float axisVal = Input.GetAxis("joy_" + (controllerNumber - 1) + "_axis_" + i);
            if (Mathf.Abs(axisVal) < 0.8f)
            {
                axisNeutral[i] = 0;
            }
            else if (axisVal < 0)
            {
                axisNeutral[i] = -1;
            }
            else
            {
                axisNeutral[i] = 1;
            }
        }
    }
}