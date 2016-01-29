using UnityEngine;
using System.Collections.Generic;

public class ControllerPool : MonoBehaviour{
    private List<Controller> pool;
    private string[] controllers;
    private string joined = "";
    [Range(0, 1)]
    public float deadBand = 0.15f;
    [Range(0, 10)]
    public int numAxes = 10;
    [Range(0, 20)]
    public int numButtons = 10;
    public float clearDuration = 0.15f;

    private static ControllerPool instance;
    
    private string[] newControllers;
    private string newJoined;

    public static ControllerPool GetInstance()
    {
        return instance;
    }

    // Use this for initialization
    void Awake()
    {
        if (instance != null)
            Debug.LogWarning("ControllerPool: There is more than one instance of the Controller Pool in the scene.");
        else
            instance = this;
        pool = new List<Controller>();
        controllers = Input.GetJoystickNames();
        joined = string.Join(" ", controllers);
        for (int i = 0; i < controllers.Length; i++)
        {
            AddController(controllers[i]);
        }

        if (pool.Count == 0)
        {
            AddController("null");
            pool[0].Disconnected();
        }
    }

    // Update is called once per frame
    void Update()
    {
        string[] newControllers = Input.GetJoystickNames();
        string newJoined = string.Join(" ", newControllers);
        if (newJoined != joined)
        {
            //Controllers Added
            for (int i = controllers.Length; i < newControllers.Length; i++)
            {
                if (controllers.Length == 0 && i == 0)
                {
                    pool[0].Reinitialize(newControllers[0]);
                    pool[0].Connect();
                }
                else
                {
                    AddController(newControllers[i]);
                }
            }
            //Controller Disconnected
            for (int i = 0; i < controllers.Length; i++)
            {
                if (controllers[i] != newControllers[i])
                {
                    if (controllers[i] != "")
                    {
                        Debug.Log("Controller Disconnected: #" + i + " (" + controllers[i] + ")");
                        pool[i].Disconnected();
                    }
                    else
                    {
                        Debug.Log("Controller Reconnected: #" + i + " (" + newControllers[i] + ")");
                        pool[i].Connect();
                    }
                }
            }
            controllers = newControllers;
            joined = newJoined;
        }
        for (int i = 0; i < pool.Count; i++)
            pool[i].Update(Time.deltaTime);
    }

    void Destroy()
    {
        if (instance == this)
            instance = null;
    }

    private void AddController(string controllerName)
    {
        pool.Add(new Controller(controllerName, (pool.Count + 1)));
        pool[pool.Count - 1].Connect();
        Debug.Log("controller connected");
    }

    public Controller GetController(int controller)
    {
        if ((controller - 1) < pool.Count)
            return pool[(controller - 1)];
        return null;
    }

    public Controller[] GetControllers()
    {
        return pool.ToArray();
    }
}