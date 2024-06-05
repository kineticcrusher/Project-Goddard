using Assets.Scripts.Interfaces;
using System;
using UnityEngine;

namespace Assets.Scripts.Objects
{
    public class Parachute : MonoBehaviour, IStageable
    {
        public Action Stage { get; set; }

        private void Awake()
        {
            Stage = Deploy;
        }

        private void Deploy()
        {
            Debug.Log("Parachute Deployed");
        }
    }
}
