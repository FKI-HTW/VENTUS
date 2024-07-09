---
layout: page
title: Object Manipulation
permalink: /user/objects
nav_order: 5
parent: User Documentation
---

## Object Manipulation

### Transform Objects

<video width="50%" height="auto" autoplay muted loop>
<source src="https://github.com/FKI-HTW/VENTUS/raw/develop/Assets/StreamingAssets/GlossaryVideos/transform.mp4" type="video/mp4">
</video>

This panel allows you to transform sketched objects and individual parts of models. Here the currently selected object can either be moved, rotated or scaled. Depending on what button you clicked when opening the transformation panel, a different transformation type will be activated at the start. The current transformation type can be changed by either clicking on the corresponding row in the panel, or by going back to the radial ui and choosing a different transformation.

Once a transformation type is chosen, the transformation can be applied to the selected object in four different ways. Either click on the -/+ button corresponding to one axis for a single small change or keep the button pressed for a gradual change. Another method for transforming the object is by pressing on the black panel in between the -/+ buttons and dragging left or right to adjust the value in that direction. The last method for transforming the object is by directly pressing on the selected object and moving according to the selected transformation type.
If the position transformation is selected, moving your hand will move the object along with it. If rotation is selected, rotating your will rotate the object accordingly. If scaling is selected, pointing your hand further away from the pivot of the object , visualised by the coordinate system, will increase the scale and vice-versa.

---

### Annotate Objects

This tool allows you to add, edit or delete notes on any object in the scene. Annotations will be seen by yourself and your coworkers. You can reach the annotation option through the select tool and clicking the button ‘Open Annotation’ in the radial UI, which opens when you select an object or drawing.
Create: To create an annotation please click on ‘Create Annotation’. When creating a new annotation you can either use preset text suggestions and subjects provided in a second window above the annotation window, or you can type in any custom text through the provided keyboard and confirm through the green ‘confirm’ button. You will then be able to attach your annotation to any position on the selected object. To place it there, you only need to press the trigger button. 
Read: To read an annotation you can click on the annotation icon you placed on the object.
Edit: To edit an annotation please select the object, click on ‘Open Annotation’ and then ‘Edit Annotations’ and you will be provided with a list of annotations added to this model part. To modify any specific annotation you can select it from this list, which will open an extra window to modify the information.

---

### Hierarchy

The hierarchy of a model is equivalent to its component structure. You will find the name of the object at the top and the different object meshes it has listed underneath. All currently selected model parts will be marked in their players’ colours. To delete an entire model you can click the button ‘Delete Model’ at the top of the window.
