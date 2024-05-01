---
layout: page
title: Taskbar
permalink: /user/taskbar
nav_order: 4
parent: User Documentation
---

## Taskbar

The horizontal taskbar is shown after creating or joining a session. When moving your cursor or interaction ray over it tooltips will open and provide you with the name of each button. From left to right you will find the Help Menu button, Info button to open the Server Status information panel, return to scene’s origin button, Start VR-Mode button, Model Library Button, File Explorer button, Select Object toggle button, Grab Tool toggle button, 3D Sketching toggle button, Pointing toggle button, Undo button, Redo button, Time & Date, Minimise Taskbar button. For each button you will find a separate description in this help menu. 
Each of the four toggle buttons will become blue when clicked once, which means you selected this interaction. As long as one interaction is selected it defines the behaviour of your controller or mouse when pressing the trigger button. To choose a different selection, just click on another toggle button or unselect the current interaction by clicking on the blue button again. 
In case the taskbar is too big for you at the moment, you can drag and drop it to a different position, as described in ‘Draggable Panels’, or minimise it through the Minimise button.

### Server Status

The Server Status panel provides you with all information regarding your current session. Here you will find the Join Code, which is a 6-8 number and digit code, needed to join your session. Via the two buttons next to it, you can either copy or send it via email to users that want to join the session.
At the bottom of the panel you can also find a list of currently connected users, through which you can kick unwelcome users, if you are the session host. Beneath the client list you will also find a disconnect and exit button. When clicked, you will be disconnected from the current session - or as a host, close the session for all users. You will then be redirected to the VENTUS Welcome panel, from which you can exit the application or start a new session.

---

### VR- and Desktop-Mode

VENTUS comes with two different modes: the VR-Mode and the Desktop-Mode. When first starting the application you will find yourself in the desktop mode to complete the first steps to create or join a session. When the taskbar opens you will then find a button with VR-glasses on it. When clicked you will switch to VR-Mode in case a VR-device is connected. To change back to Desktop-Mode you can click the button with a computer on it at the exact same position where the VR-button was.

---

### Return to Origin

This button in the taskbar will teleport you directly back to the centre of the scene marked with a blue cross on the floor. When first joining the scene every client starts here.

---

### Model Library

<video width="50%" height="auto" autoplay muted loop>
<source src="https://github.com/CENTIS-HTW/VENTUS/raw/develop/Assets/StreamingAssets/GlossaryVideos/library.mp4" type="video/mp4">
</video>

The model library provides example models that can be used to demonstrate or test the functions of VENTUS. By clicking one of the example models, they will be imported into the scene in front of you.

---

### File Explorer

The file explorer provides the functionality of importing models from the local filesystem into the scene. The explorer can be navigated, like most filesystems, by either clicking on the listed folders or selecting the folders in the path located at the top of the file explorer. By using the arrow buttons at the top left, navigating between the previously visited folders is also possible. To import a model into the scene, simply select a file of a supported type in your file system.

---

### Select Objects

<video width="50%" height="auto" autoplay muted loop>
<source src="https://github.com/CENTIS-HTW/VENTUS/raw/develop/Assets/StreamingAssets/GlossaryVideos/select.mp4" type="video/mp4">
</video>

The select interaction can be used to interact with either model parts or sketched objects. To do so simply point at an object and press the interaction button. Once an object is selected a radial UI opens at its centre. From his UI you can choose between multiple options from the top clockwise: Scale, Rotate, Move, Close, Hierarchy, Open Annotation.
To deselect the object please click on the ‘x’ button to close the radial UI.
When clicking on Scale, Rotate or Move a Transform Object window will open through which you can make changes to this specific model part. Either click on the provided ‘+’ and ‘-’ buttons to adjust settings or click on the object itself to transform it free hand with the selected option. To go back to all options simply click on the ‘back’-button at the upper left corner or the ‘close’-button in the upper right corner of the window.
To understand the hierarchy of the selected object you can open the Hierarchy window through clicking on the Hierarchy button in this radial UI. 
To open all options regarding Annotations please click on the ‘Open Annotation’ button to be provided with a radial UI created to manage all annotations. 
For more information please also read the separate sections about the specific options of the radial UI, like the Transform Object window, the Hierarchy and the Annotations.

---

### Grab Objects

<video width="50%" height="auto" autoplay muted loop>
<source src="https://github.com/CENTIS-HTW/VENTUS/raw/develop/Assets/StreamingAssets/GlossaryVideos/grab.mp4" type="video/mp4">
</video>

Grab is a free-hand tool, which can be applied on model parts and sketched objects. This interaction combines the transformation types of move and rotate into one. By pointing at a grabbable object and pressing the interaction button you can then move and rotate the object using your hand’s movement and rotation.

---

### Sketching

<video width="50%" height="auto" autoplay muted loop>
<source src="https://github.com/CENTIS-HTW/VENTUS/raw/develop/Assets/StreamingAssets/GlossaryVideos/sketching.mp4" type="video/mp4">
</video>

When the sketching interaction is activated, it is possible to draw lines in the scene using the trigger button on your controller. These lines can be customised in the panel, which opens when selecting the interaction button. Here you can choose a colour for your sketches and the line diameter. Sketching is only possible if the controller is not currently pointing at or interacting with an interface. Sketched lines can also be selected and grabbed like the imported models.

---

### Pointing

<video width="50%" height="auto" autoplay muted loop>
<source src="https://github.com/CENTIS-HTW/VENTUS/raw/develop/Assets/StreamingAssets/GlossaryVideos/magicPointer.mp4" type="video/mp4">
</video>

When the pointing interaction is activated, you can use your controller as a virtual laser pointer. You can point on everything around you, pressing the trigger button on your controller. The ray you see while pressing the trigger is networked and can be seen by everyone who joined the session.

---

### Undo Redo

<video width="50%" height="auto" autoplay muted loop>
<source src="https://github.com/CENTIS-HTW/VENTUS/raw/develop/Assets/StreamingAssets/GlossaryVideos/undoRedo.mp4" type="video/mp4">
</video>

Using this functionality, you can undo or redo interactions that you did while using VENTUS. These interactions include the selection, grabbing and transformation of model parts and sketches, creation of annotations and importing of models. You can only undo interactions that you performed, not any interactions done by other connected clients. If any other connected client overwrites one of your last interactions with his own, like selecting a model part which you previously transformed, your interaction will be removed from your undo list.

---

### Maximize Minimize

At the very right side of the taskbar you will find the minimise button, which allows you to shrink the taskbar to its smallest possible size. To transform the taskbar to its original size please click on the maximise button next to the clock.