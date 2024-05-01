using System;
using System.Collections.Generic;

namespace VENTUS.UI.Glossary
{
    public struct GlossaryEntry
    {
        public EntryType Type;
        public string Title;
        public string Text;
        public string VideoUrl;

        public GlossaryEntry(EntryType type, string title, string text, string videoUrl)
        {
            Type = type;
            Title = title;
            Text = text;
            VideoUrl = videoUrl;
        }
    }

    public enum EntryType
    {
        Welcome,
        Controls,
        Keyboard,
        DraggablePanels,
        AvatarCreation,
        CreateServer,
        JoinServer,
        Taskbar,
        ServerStatus,
        ModeSwitch,
        ReturnToOrigin,
        ModelLibrary,
        FileExplorer,
        SelectObject,
        TransformObject,
        AnnotateObjects,
        Hierarchy,
        GrabObject,
        Sketching,
        Pointing,
        UndoRedo,
        MinimizeMaximize,
    }
    
    public static class GlossaryManager
    {
        public static Dictionary<EntryType, GlossaryEntry> Entries { get; } = new();

        public static event Action<GlossaryEntry> OnCurrentEntryUpdated;

        public static GlossaryEntry CurrentEntry { get; private set; }

        public static void SetCurrentEntry(EntryType value)
        {
            if (!Entries.TryGetValue(value, out var entry))
                return;

            CurrentEntry = entry;
            OnCurrentEntryUpdated?.Invoke(entry);
        }
        
        #region initialisation

        static GlossaryManager() 
        {
            Entries.Add(EntryType.Welcome, new(
                EntryType.Welcome,
                "Welcome",
				@"VENTUS is a framework for collaborative prototyping of CAD-Models. Using VENTUS it is possible to edit and discuss these models in an immersive environment on a global scale in real time. Easy-to-use features, such as model transformations, annotations for providing feedback and sketching in 3D-space, allow both professionals and new users alike to utilise VENTUS for developing prototypes with colleagues or clients.

If you require any further explanation on any of the functionalities VENTUS provides, you will find ‘?’ buttons in the upper right corner of every UI panel, which will open the help menu at the relevant section.",
                "welcome.mp4"));
            CurrentEntry = Entries[EntryType.Welcome];
            
            Entries.Add(EntryType.Controls, new(
                EntryType.Controls,
                "Controls",
                @"To move around your scene in Desktop-Mode, please keep the right mouse button pressed while using the WASD-keys to navigate. To move to the right press ‘D’, to move to the left press ‘A’, to move forward press ‘W’ and to move back press ‘S’. It is also possible to press more keys then one to combine movements. To interact with any object or interface, please press the left mouse button.
To interact with an item or interface in VR-Mode please press the trigger button underneath your controller as indicated through a tooltip when waiting for a view seconds. To move around in VR you can push the analog-stick forward, while aiming at your desired destination. Releasing the analog then starts the teleport.",
                ""));
            
            Entries.Add(EntryType.Keyboard, new(
                EntryType.Keyboard,
                "VR-Keyboard",
                @"Whenever you click on an input field in VR a keyboard will open. There are different keyboards for example a small one for numbers and a bigger one for text input. Just click on the different keys to write and confirm your input through clicking on the green arrow confirm button.",
                ""));
            
            Entries.Add(EntryType.DraggablePanels, new(
                EntryType.DraggablePanels,
                "Draggable Panels",
                @"All panels like the taskbar and top windows, the transform window or the input windows for annotations can be dragged to a position you prefer. When hovering over any window, you will see a light outline around it. Just press the trigger button when the outline is there, move the window to its new position while pressing the trigger button and release it wherever you would like the window to be placed for now.",
                ""));
            
            Entries.Add(EntryType.AvatarCreation, new(
                EntryType.AvatarCreation,
                "Avatar Creation",
				@"In this window you can create and customise your avatar for this session. This avatar will be visible to every connected client. Customisation is possible by choosing your own username, a hairstyle, hair colour, and main colour. Your chosen main colour will be visible as the colour of your avatar’s shirt and outline when selecting models in the scene. 
After customising your avatar you can choose between the options ‘Create Session’ or ‘Join Session’ to either be the host and create a new session, or join an existing session.",
                "welcome.mp4"));

            Entries.Add(EntryType.CreateServer, new(
                EntryType.CreateServer,
                "Create Session",
                @"In this window you can create your own session. The maximum number of users who are allowed to join the session can be changed in the first dropdown. While this session is hosted on your computer, it requires a separate server that establishes the connection between you and other users who would like to join your session. This servers’ location can be changed in the second dropdown, in case the selected default location is not suitable for your use case. To confirm your choices please click the ‘Create’ button. Once you have joined a session the scene will change and you will see a horizontal taskbar, through which you can interact with the application.",
				"createServer.mp4"));

            Entries.Add(EntryType.JoinServer, new(
                EntryType.JoinServer,
                "Join Session",
                @"In this window you can join an already created session. To do this simply type in the join code, which you should have received from the host of the created session, or paste it in the input field using the button right next to it. The code may consist of up to 8 capital letters and numbers. To confirm your input please click the ‘Join’ button. Once you have joined a session the scene will change and you will see a horizontal taskbar, through which you can interact with the application.",
				"joinServer.mp4"));
            
            Entries.Add(EntryType.Taskbar, new(
                EntryType.Taskbar,
                "Taskbar",
                @"The horizontal taskbar is shown after creating or joining a session. When moving your cursor or interaction ray over it tooltips will open and provide you with the name of each button. From left to right you will find the Help Menu button, Info button to open the Server Status information panel, return to scene’s origin button, Start VR-Mode button, Model Library Button, File Explorer button, Select Object toggle button, Grab Tool toggle button, 3D Sketching toggle button, Pointing toggle button, Undo button, Redo button, Time & Date, Minimise Taskbar button. For each button you will find a separate description in this help menu. 
Each of the four toggle buttons will become blue when clicked once, which means you selected this interaction. As long as one interaction is selected it defines the behaviour of your controller or mouse when pressing the trigger button. To choose a different selection, just click on another toggle button or unselect the current interaction by clicking on the blue button again. 
In case the taskbar is too big for you at the moment, you can drag and drop it to a different position, as described in ‘Draggable Panels’, or minimise it through the Minimise button.",
                ""));
            
            Entries.Add(EntryType.ServerStatus, new(
                EntryType.ServerStatus,
                "Server Status",
				@"The Server Status panel provides you with all information regarding your current session. Here you will find the Join Code, which is a 6-8 number and digit code, needed to join your session. Via the two buttons next to it, you can either copy or send it via email to users that want to join the session.
At the bottom of the panel you can also find a list of currently connected users, through which you can kick unwelcome users, if you are the session host. Beneath the client list you will also find a disconnect and exit button. When clicked, you will be disconnected from the current session - or as a host, close the session for all users. You will then be redirected to the VENTUS Welcome panel, from which you can exit the application or start a new session.",
                ""));
            
            Entries.Add(EntryType.ModeSwitch, new(
                EntryType.ModeSwitch,
                "VR- and Desktop-Mode",
                @"VENTUS comes with two different modes: the VR-Mode and the Desktop-Mode. When first starting the application you will find yourself in the desktop mode to complete the first steps to create or join a session. When the taskbar opens you will then find a button with VR-glasses on it. When clicked you will switch to VR-Mode in case a VR-device is connected. To change back to Desktop-Mode you can click the button with a computer on it at the exact same position where the VR-button was.",
                ""));
            
            Entries.Add(EntryType.ReturnToOrigin, new(
                EntryType.ReturnToOrigin,
                "Return to Origin",
                @"This button in the taskbar will teleport you directly back to the centre of the scene marked with a blue cross on the floor. When first joining the scene every client starts here.",
                ""));
            
            Entries.Add(EntryType.ModelLibrary, new(
                EntryType.ModelLibrary,
                "Model Library",
				@"The model library provides example models that can be used to demonstrate or test the functions of VENTUS. By clicking one of the example models, they will be imported into the scene in front of you.",
                "library.mp4"));
            
            Entries.Add(EntryType.FileExplorer, new(
                EntryType.FileExplorer,
                "File Explorer",
				@"The file explorer provides the functionality of importing models from the local filesystem into the scene. The explorer can be navigated, like most filesystems, by either clicking on the listed folders or selecting the folders in the path located at the top of the file explorer. By using the arrow buttons at the top left, navigating between the previously visited folders is also possible. To import a model into the scene, simply select a file of a supported type in your file system.",
                ""));
            
            Entries.Add(EntryType.SelectObject, new(
                EntryType.SelectObject,
                "Select Objects",
                @"The select interaction can be used to interact with either model parts or sketched objects. To do so simply point at an object and press the interaction button. Once an object is selected a radial UI opens at its centre. From his UI you can choose between multiple options from the top clockwise: Scale, Rotate, Move, Close, Hierarchy, Open Annotation.
To deselect the object please click on the ‘x’ button to close the radial UI.
When clicking on Scale, Rotate or Move a Transform Object window will open through which you can make changes to this specific model part. Either click on the provided ‘+’ and ‘-’ buttons to adjust settings or click on the object itself to transform it free hand with the selected option. To go back to all options simply click on the ‘back’-button at the upper left corner or the ‘close’-button in the upper right corner of the window.
To understand the hierarchy of the selected object you can open the Hierarchy window through clicking on the Hierarchy button in this radial UI. 
To open all options regarding Annotations please click on the ‘Open Annotation’ button to be provided with a radial UI created to manage all annotations. 
For more information please also read the separate sections about the specific options of the radial UI, like the Transform Object window, the Hierarchy and the Annotations.",
                ""));

            Entries.Add(EntryType.TransformObject, new(
                EntryType.TransformObject,
                "Object Transformation",
                @"This panel allows you to transform sketched objects and individual parts of models. Here the currently selected object can either be moved, rotated or scaled. Depending on what button you clicked when opening the transformation panel, a different transformation type will be activated at the start. The current transformation type can be changed by either clicking on the corresponding row in the panel, or by going back to the radial ui and choosing a different transformation.

Once a transformation type is chosen, the transformation can be applied to the selected object in four different ways. Either click on the -/+ button corresponding to one axis for a single small change or keep the button pressed for a gradual change. Another method for transforming the object is by pressing on the black panel in between the -/+ buttons and dragging left or right to adjust the value in that direction. The last method for transforming the object is by directly pressing on the selected object and moving according to the selected transformation type.
If the position transformation is selected, moving your hand will move the object along with it. If rotation is selected, rotating your will rotate the object accordingly. If scaling is selected, pointing your hand further away from the pivot of the object , visualised by the coordinate system, will increase the scale and vice-versa.",
				"transform.mp4"));
            
            Entries.Add(EntryType.AnnotateObjects, new(
                EntryType.AnnotateObjects,
                "Object Annotations",
                @"This tool allows you to add, edit or delete notes on any object in the scene. Annotations will be seen by yourself and your coworkers. You can reach the annotation option through the select tool and clicking the button ‘Open Annotation’ in the radial UI, which opens when you select an object or drawing.
Create: To create an annotation please click on ‘Create Annotation’. When creating a new annotation you can either use preset text suggestions and subjects provided in a second window above the annotation window, or you can type in any custom text through the provided keyboard and confirm through the green ‘confirm’ button. You will then be able to attach your annotation to any position on the selected object. To place it there, you only need to press the trigger button. 
Read: To read an annotation you can click on the annotation icon you placed on the object.
Edit: To edit an annotation please select the object, click on ‘Open Annotation’ and then ‘Edit Annotations’ and you will be provided with a list of annotations added to this model part. To modify any specific annotation you can select it from this list, which will open an extra window to modify the information.",
                ""));
            
            Entries.Add(EntryType.Hierarchy, new(
                EntryType.Hierarchy,
                "Model Hierarchy",
                @"The hierarchy of a model is equivalent to its component structure. You will find the name of the object at the top and the different object meshes it has listed underneath. All currently selected model parts will be marked in their players’ colours. To delete an entire model you can click the button ‘Delete Model’ at the top of the window.",
                ""));
            
            Entries.Add(EntryType.GrabObject, new(
                EntryType.GrabObject,
                "Grab Objects",
                @"Grab is a free-hand tool, which can be applied on model parts and sketched objects. This interaction combines the transformation types of move and rotate into one. By pointing at a grabbable object and pressing the interaction button you can then move and rotate the object using your hand’s movement and rotation.",
                ""));
            
            Entries.Add(EntryType.Sketching, new(
                EntryType.Sketching,
                "Sketching",
				@"When the sketching interaction is activated, it is possible to draw lines in the scene using the trigger button on your controller. These lines can be customised in the panel, which opens when selecting the interaction button. Here you can choose a colour for your sketches and the line diameter. Sketching is only possible if the controller is not currently pointing at or interacting with an interface. Sketched lines can also be selected and grabbed like the imported models.",
                "sketching.mp4"));
            
            Entries.Add(EntryType.Pointing, new(
                EntryType.Pointing,
                "Pointing",
				@"When the pointing interaction is activated, you can use your controller as a virtual laser pointer. You can point on everything around you, pressing the trigger button on your controller. The ray you see while pressing the trigger is networked and can be seen by everyone who joined the session.",
				"magicPointer.mp4"));
            
            Entries.Add(EntryType.UndoRedo, new(
                EntryType.UndoRedo,
                "Undo and Redo",
                @"Using this functionality, you can undo or redo interactions that you did while using VENTUS. These interactions include the selection, grabbing and transformation of model parts and sketches, creation of annotations and importing of models. You can only undo interactions that you performed, not any interactions done by other connected clients. If any other connected client overwrites one of your last interactions with his own, like selecting a model part which you previously transformed, your interaction will be removed from your undo list.",
                ""));
            
            Entries.Add(EntryType.MinimizeMaximize, new(
                EntryType.MinimizeMaximize,
                "Minimize and Maximize",
                @"At the very right side of the taskbar you will find the minimise button, which allows you to shrink the taskbar to its smallest possible size. To transform the taskbar to its original size please click on the maximise button next to the clock.",
                ""));
        }
        
        #endregion
    }
}
