using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TerminalInterpreter : MonoBehaviour
{
    List<string> response = new List<string>();

    public GameObject currentControllableObject;

    private IControllable controllable;

    private void Awake()
    {
        if (currentControllableObject) controllable = currentControllableObject.GetComponent<IControllable>();
    }

    public List<string> Interpret(string input)
    {
        response.Clear();

        var args = input.Split();

        switch (args[0].ToLower())
        {
            case "help":
                return Help(args);
            case "throttle":
                return Throttle(args);
            default:
                return NotRecognized();
        }
    }

    private List<string> NotRecognized()
    {
        response.Add("Command not recognized.");
        response.Add("Type 'help' for list of commands.");

        return response;
    }

    private List<string> Throttle(string[] args)
    {
        if(args.Length == 1)
        {
            response.Add("Expected an additional argument.");
            response.Add("Type 'help throttle' to learn more about the throttle command.");            
        }
        if(controllable is null)
        {
            response.Add("A controllable object has not been set.");
        }
        else
        {
            switch (args[1].ToLower())
            {
                case "up":
                    controllable.SetThrottle(100.0f);
                    response.Add("Throttle set to max.");
                    break;
                case "down":
                    controllable.SetThrottle(0f);
                    response.Add("Throttle set to min.");
                    break;
                default:
                    if(float.TryParse(args[1], out float value))
                    {
                        controllable.SetThrottle(value);
                        response.Add($"Throttle set to {value}.");
                    }
                    else
                    {
                        response.Add("Invalid arguement.");
                        response.Add("Please type 'help throttle' to learn how to use this command.");
                    }
                    break;
            }
        }

        return response;
    }

    private List<string> Help(string[] args)
    {
        if (args.Length == 1)
        {
            response.Add("The help command allows you to gain more information from each available command.");
            response.Add("To learn about a command type 'help <command name>'");
            response.Add("The list of commands are as follows:");
            response.Add("\tThrottle");
        }
        else
        {
            switch (args[1].ToLower())
            {
                case "throttle":
                    response.Add("thottle allows you to set the throttle value of the controlled vehicle.");
                    response.Add("'throttle up' for max throttle.");
                    response.Add("'throttle down' for min throttle.");
                    response.Add("'thottle <num>' to input a value 0-100 to set the throttle to.");
                    break;
                default:
                    response.Add("Unknown command. Type 'help' for a list of commands.");
                    break;
            }
        }

        return response;
    }
}
