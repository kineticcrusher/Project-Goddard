using Assets.Scripts.Interfaces;
using Assets.Scripts.Objects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControlPod : MonoBehaviour, IControllable
{
    List<List<GameObject>> Stages = new();

    public float throttle;

    public void SetThrottle(float throttle)
    {
        this.throttle = throttle;
    }

    private void Start()
    {
        var parachute = new GameObject();
        parachute.AddComponent<Parachute>();

        var engine = new GameObject();
        engine.AddComponent<Engine>();  

        List<GameObject> stage1 = new List<GameObject>() { engine};
        List<GameObject> stage2 = new List<GameObject>() { parachute };

        Stages.Add(stage1);
        Stages.Add(stage2);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Stage();
        }
    }


    public void Stage()
    {

        var stage = Stages[0];
        foreach (IStageable stageable in stage.Select(x => x.GetComponent<IStageable>()))
        {
            if (stageable != null)
            {
                stageable.Stage();
            }
        }

        Stages.Remove(stage);
    }
}
