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

## Other Features

### UI Background Click Disabler

Normally, when interacting with an UI, if you click outside a Visual Element, the last selected element is disabled. This is not good for UIs in games on PC.

To disable this behavior, use the [BackgroundClickDisabler](/Runtime/BackgroundClickDisabler.cs) component inside a Prefab using a EventSystem and InputSystemUIInputModule components:

![Background Click Disabler](/Documentation~/BackgroundClickDisabler.png)

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