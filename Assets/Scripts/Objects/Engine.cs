using Assets.Scripts.Interfaces;
using UnityEngine;
using System;

namespace Assets.Scripts.Objects
{
    public class Engine : MonoBehaviour, IStageable
    {
        public Action Stage { get; set; }

        private void Awake()
        {
            Stage = Ignite;
        }

        private void Ignite()
        {
            Debug.Log("Engine Ignited");
        }
    }
}
