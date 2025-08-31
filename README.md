# UI System

* This package is an extension for Unity UI Toolkit System.
* Unity minimum version: **2022.3**
* Current version: **0.2.0**
* License: **MIT**
* Dependencies:
	- Unity.Input System : [1.2.0](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.2/changelog/CHANGELOG.html)
	- ActionCode.Awaitable System : [1.0.0](https://github.com/HyagoOliveira/AwaitableSystem/tree/1.0.0)
	- ActionCode.Input System : [1.1.0](https://github.com/HyagoOliveira/InputSystem/tree/1.1.0)
	- ActionCode.Serialized Dictionary : [1.2.0](https://github.com/HyagoOliveira/SerializedDictionary/tree/1.2.0)
	- com.unity.ugui: [2.0.0](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/changelog/CHANGELOG.html)
	- com.unity.modules.uielements: [1.0.0](https://docs.unity3d.com/6000.2/Documentation/ScriptReference/UnityEngine.UIElementsModule.html)

## Summary

Create Menus and Popups using Unity UI Toolkit

### Custom Runtime Theme

This package has a custom [runtime theme](/Settings/Themes/ActionCodeUISystem.tss) overriding some classes styles.
You can see the overriding style sheet [here](/Settings/Themes/Default.uss).

When using UI Builder, you can select this theme:

![ActionCode UI System Theme](/Docs~/ActionCodeUISystem.png)

To see it in runtime, make sure to use this [PanelSettings](/Settings/PanelSettings.asset) on your UI Document component.

### UI Background Click Disabler

Normally, when interacting with an UI Document, if you click outside a Visual Element, the last selected element is disabled. This is not good for UI in games.

To disable this behavior, use the [BackgroundClickDisabler](/Runtime/Inputs/BackgroundClickDisabler.cs) component. 

Just put the prefab [InputEventSystem](/Prefabs/Inputs/InputEventSystem.prefab) into your current/dependency scene. This prefab contains all the components necessary to run your UI correctly.

### Popups

All available popups are ready for simple or localized texts, using show and/or close animations.

You can use any available popup from this package by using the [Popups](/Prefabs/Popups/Popups.prefab) prefab. Put this prefab inside your current/dependency scene and use [Popups](/Runtime/Popups/Popups.cs) component.

Alternatively, you can create your own popups prefabs, place them inside Popups global prefab and use them for your project.

The next section shows how to use any available popup.

### Dialogue Popup

This Popup has a message, an optional title and a Confirm and Cancel buttons, with optional callbacks to each button click action.

You can use the default [DialoguePopup](/Prefabs/Popups/Dialogue/DialoguePopup.prefab) implementation found in this package or create your own using the [DialoguePopup](/Runtime/Popups/DialoguePopup.cs) component.

You can show show the dialogue as follow:

```csharp
private void ShowQuitGameDialogue()
{
    Popups.Dialogue.Show(
        message: "Are you sure?",
        title: "Quitting the game",
        onConfirm: QuitGame, // Action to execute when confirming the quit
        onCancel: GoToMainMenu // Action to execute when canceling the quit
    );
}

private void ShowLocalizedQuitGameDialogue()
{
    Popups.Dialogue.Show(
        tableId: "LoadMenu", // A localization Table with this name must exist in the project.
        messageId: "confirm_message", // The message id inside the localization table.
        titleId: "delete_title", // The title id inside the localization table.
        onConfirm: QuitGame,
        onCancel: GoToMainMenu
    );
}
```

>>Note: For the above example, a [Popups](/Runtime/Popups/Popups.cs) instance should be instantiated into your current/dependency scene.

Finally, you can close the popup by using the Cancel button, the Navigation Cancel from the keyboard (Esc button) or gamepad (East button).

## Installation

### Using the Package Registry Server

Follow the instructions inside [here](https://cutt.ly/ukvj1c8) and the package **ActionCode-UI System** 
will be available for you to install using the **Package Manager** windows.

### Using the Git URL

You will need a **Git client** installed on your computer with the Path variable already set. 

- Use the **Package Manager** "Add package from git URL..." feature and paste this URL: `https://github.com/HyagoOliveira/UISystem.git`

- You can also manually modify you `Packages/manifest.json` file and add this line inside `dependencies` attribute: 

```json
"com.actioncode.input-system":"https://github.com/HyagoOliveira/UISystem.git"
```

---

**Hyago Oliveira**

[GitHub](https://github.com/HyagoOliveira) -
[BitBucket](https://bitbucket.org/HyagoGow/) -
[LinkedIn](https://www.linkedin.com/in/hyago-oliveira/) -
<hyagogow@gmail.com>