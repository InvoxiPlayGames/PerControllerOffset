# PerControllerOffset

A simple Beat Saber plugin to save a different controller offset
depending on which SteamVR controller is being used.

The name means "per controller" as in per controller system, not
per controller as in per-hand offset adjustment. Useful for those
who have several VR setups, but use the same PC.

For more advanced offset configuration, profile management, etc,
use [EasyOffset](https://github.com/Reezonate/EasyOffset).
(This plugin is not tested to work with EasyOffset.)

Oculus VR mode should work, but isn't tested.

Compatible with 1.29.1, *maybe* earlier versions too. 1.29.4+ uses
OpenXR which doesn't expose proper controller information. For users
of the SteamVR OpenXR runtime, I can think of a few ways to fix this,
but for now it doesn't work.

## Notice

This plugin is a work-in-progress.

## How to Use

Launch the game, and save your controller offset configuration.
A new config file will be made for the controller currently active
in the right hand. This config file will be loaded whenever the game
is launched using that controller.

## Configuration

There are no in-game UI options to configure. The `UserData/PerControllerOffset`
folder will contain JSON files with the offsets set for each
controller type used. These can be edited to dial in percise values,
or swapped between installations to share configs.
