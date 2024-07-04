---
layout: page
title: Packages
permalink: /developer/packages
nav_order: 2
parent: Developer Documentation
has_toc: false
has_children: true
---

## Packages

For the development of the VENTUS project various packages were used. These include both external packages and features, which we converted into separate packages.

External packages used:

- [FishNet](https://github.com/FirstGearGames/FishNet) as network framework
- [FishyUnityTransport](https://github.com/ooonush/FishyUnityTransport/tree/master) API implementation of the Unity Transport for FishNet
- [Unity glTFast](https://docs.unity3d.com/Packages/com.unity.cloud.gltfast@5.2/manual/index.html) API for importing GLTF-models into Unity
- [XR Interaction Toolkit](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.0/manual/index.html) Unity package for developing cross-platform XR applications


Our provided packages are:
- [UnityFileExplorer](/VENTUS/developer/packages/UnityFileExplorer) package for browsing the local file explorer during the Unity runtime
- [UnityHierarchyView](/VENTUS/developer/packages/UnityHierarchyView) package for visualising the Unity scene hierarchy with a customized UI during the Unity runtime
- [UnitySTEPImporter](/VENTUS/developer/packages/UnitySTEPImporter) package for importing STEP-models into Unity
- [VRSketchingGeometry](/VENTUS/developer/packages/VRSketchingGeometry) package for sketching with geometry in virtual reality
- [XRPlatformManagement](/VENTUS/developer/packages/XRPlatformManagement) package for managing XR applications and supporting different controller models
