using System;
using System.Collections.Generic;
using CENTIS.XRPlatformManagement.Controller.Elements;
using CENTIS.XRPlatformManagement.Controller.Manager;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VENTUS.PlaformPackageExtension
{
    public class ExtendedControllerModelSpawner : ControllerModelSpawner
    {
        [SerializeField] private XRInteractorLineVisual _xrInteractorLineVisual;
        
        protected override void OnTrackingInitialized()
        {
            AddComponents();
            
            _xrInteractorLineVisual.enabled = HasTrackedDevice;
        }

        protected override void OnTrackingAcquired()
        {
            AddComponents();
            _xrInteractorLineVisual.enabled = true;
        }

        protected override void OnTrackingLost()
        {
            base.OnTrackingLost();
            _xrInteractorLineVisual.enabled = false;
        }

        private void AddComponents()
        {
            Dictionary<Enum, ControllerElementServiceLocator> modelsLookup = GetCurrentModelsLookup();
            foreach (var controllerElementServiceLocator in modelsLookup)
            {
                controllerElementServiceLocator.Value.AddComponentOfType<ParentLayerInheritor>();
            }
        }
    }
}
