# UI System

* This package is an extension for Unity UI Toolkit System.
* Unity minimum version: **6000.1**
* Current version: **0.3.0**
* License: **MIT**
* Dependencies:
	- Unity.Input System : [1.14.2](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.14/changelog/CHANGELOG.html)
	- ActionCode.Awaitable System : [1.1.0](https://github.com/HyagoOliveira/AwaitableSystem/tree/1.1.0)
	- ActionCode.Input System : [1.3.0](https://github.com/HyagoOliveira/InputSystem/tree/1.3.0)
    - ActionCode.Screen Fade System : [1.0.0](https://github.com/HyagoOliveira/ScreenFadeSystem/tree/1.0.0)
	- ActionCode.Serialized Dictionary : [1.2.0](https://github.com/HyagoOliveira/SerializedDictionary/tree/1.2.0)
	- com.unity.ugui: [2.0.0](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/changelog/CHANGELOG.html)
	- com.unity.modules.uielements: [1.0.0](https://docs.unity3d.com/6000.2/Documentation/ScriptReference/UnityEngine.UIElementsModule.html)

## Summary

Create Menus and Popups faster using Unity UI Toolkit.

## Custom Runtime Theme

This package has a custom Theme Style Sheet called [ActionCode-UISystem](/Settings/Themes/ActionCode-UISystem.tss), overriding some classes styles by using the [ActionCode-Global](/Settings/StyleSheets/ActionCode-Global.uss) style sheet.

When using UI Builder, you can select this theme on the Viewport:

![ActionCode UI System Theme](/Docs~/ActionCodeUISystem.png)

To see it in runtime on your UI Document component, make sure to use a Panel Settings with this Theme Style Sheet set.

## UI Background Click Disabler

Normally, when interacting with an UI Document, if you click outside a Visual Element, the last selected element is disabled. This is not good for UI in games.

To disable this behavior, put the prefab [InputEventSystem](/Prefabs/Inputs/InputEventSystem.prefab) into your current/dependency scene. This prefab contains all the Input Components necessary to run your UI input correctly, with `InputSystemUIInputModule.DeselectOnBackgroundClick` disabled.

>Note: in order to really disable deselection on background clicks, always set the Picking Mode to Ignore (instead of the default Position) inside every background Visual Element your mouse can click in outside selectable elements.

## Extensions

There are some usefull extensions you can use on the [Extensions Folder](/Runtime/Extensions/).

Here are some examples:

```csharp
using ActionCode.UISystem; // Add UISystem namespace

var root = // find a VisualElement from a UIDocument.
var label = root.Find<Label>(name: "MyText"); // Finds a Label with 'MyText' name. Shows an error if element is not found.

label.SetDisplayEnabled(true); // Sets whether the element should be displayed in the layout.
label.SetSelectableEnabled(false); // Sets whether the element can be selected.
label.UpdateLocalization(tableId: "MyLocTable", entryId: "MyLocId"); // Updates the localization binding using the given table and entry IDs (need Unity Localization package).

if (label.IsFocused()) { } // Check whether the element is currently focused.
```

## Popups

All available Popups are ready for normal or localized texts, also using show/close animations.

You can use any available Popup from this package by using the [Popups](/Prefabs/Popups/Popups.prefab) prefab.
This prefab contains all Popups ready to use. You just need to put this prefab inside your current/dependency Scene and use the [Popups](/Runtime/Popups/Popups.cs) component on your code (examples in the next sections).

Alternatively, you can create your own Popup prefabs, place them inside a your Popups global prefab and use them in your projects.

The next section shows how to use any available popup.

### Dialogue Popup

This Popup has a message, an optional title and a Confirm and Cancel buttons, with optional callbacks to each button action.

![Dialogue Popup](/Docs~/DialoguePopup.png)

You can use the default [DialoguePopup](/Prefabs/Popups/Dialogue/DialoguePopup.prefab) implementation found in this package or create your own using the [DialoguePopup](/Runtime/Popups/DialoguePopup.cs) component.

You can show the dialogue as follow:

```csharp
private void ShowQuitGameDialogue()
{
    Popups.Dialogue.Show(
        message: "Are you sure?",
        title: "Quitting the game",
        onConfirm: QuitGame,    // Action to execute when confirming the quit
        onCancel: GoToMainMenu  // Action to execute when canceling the quit
    );
}

private void ShowLocalizedQuitGameDialogue()
{
    Popups.Dialogue.Show(
        tableId: "LoadMenu",            // A localization Table with this name must exist in the project.
        messageId: "confirm_message",   // The message id inside the localization table.
        titleId: "delete_title",        // The title id inside the localization table.
        onConfirm: QuitGame,
        onCancel: GoToMainMenu
    );
}
```

>Note: For the above example, a [Popups](/Runtime/Popups/Popups.cs) instance should be instantiated into your current/dependency Scene.

### Confirmation Popup

Similar to Dialogue Popup but with only a Confirmation button.

![Confirmation Popup](/Docs~/ConfirmationPopup.png)

You can show the confirmation dialog as follow:

```csharp
private void ShowSaveGameConfirmationDialogue()
{
    Popups.Confirmation.Show(
        message: "Your game was saved!",
        title: "Save Game"
    );
}
```

Use it to show important confirmation messages to the player.

>Note: You can close the any Popup by using the its own Cancel button or the Navigation Cancel from the Keyboard (usually the Esc button) or Gamepad (usually the East button).

### Using other Popups

To show your own Popups, use the function `Popups.GetPopup<YourPopupType>()`. Don't forget to add the Popup prefab as a child of your Popups prefab.

## Localization

You can show localized texts by using the [LocalizedString](/Runtime/Localization/LocalizedString.cs) struct.

```csharp
var label = root.Find<Label>(name: "PressAnyButtonLabel");
var localization = new LocalizedString(
    tableId: "MainMenu",            // The Table Id where the entry is located.
    entryId: "press_any_button",    // The Entry Id to be localized.
    fallback: "Press any button"    // The fallback text to be used when an available localization is not found.
);

label.UpdateLocalization(localization); // Updates the localization here.
```

You can also show in any Dialogue:

```csharp
public static void ShowWebGLQuitConfirmation()
{
    Popups.Confirmation.Show(
        message: new LocalizedString("Popups", "webgl_quit_message", "You must close your browser manually!"),
        title: new LocalizedString("Popups", "webgl_quit_title", "Quitting the Browser")
    );
}
```

## Installation

### Using the Package Registry Server

Follow the instructions inside [here](https://cutt.ly/ukvj1c8) and the package **ActionCode-UI System** 
will be available for you to install using the **Package Manager** windows.

### Using the Git URL

You will need a **Git client** installed on your computer with the Path variable already set. 

- Use the **Package Manager** "Add package from git URL..." feature and paste this URL: `https://github.com/HyagoOliveira/UISystem.git`

- You can also manually modify you `Packages/manifest.json` file and add this line inside `dependencies` attribute: 

```json
"com.actioncode.ui-system":"https://github.com/HyagoOliveira/UISystem.git"
```

---

**Hyago Oliveira**

[GitHub](https://github.com/HyagoOliveira) -
[BitBucket](https://bitbucket.org/HyagoGow/) -
[LinkedIn](https://www.linkedin.com/in/hyago-oliveira/) -
<hyagogow@gmail.com>