# TODO: add build and Unity project
# Eye Tracking Based Gaming Combat System

Goal of this project was to design and implement a fighting game with an aiming mechanic using player's eye gaze. Eye gaze is tracked using specialized hardware Pupil Core.
If afore mentioned hardware is not available, it's possible to use mouse instead, but combat input in such a case is limited to punches only.

## Content
* app - game demo files
    * build - contains executable ``bp.exe``
    * project - Unity project ``Requires Unity 2020.3.25f1``
    * source - source code ``Written in C#``

* docs - HTML documentation ``Doxygen generated``

* licenses - licenses of used assets
    * AprilTags - BSD 2-Clause "Simplified" License

* surface - Surface definition file for Pupil Capture. ``Copy contents to C:\Users\user_name\pupil_capture_settings``

## Controls
#### Movement
* A - left
* D - right
* S - crouch
* W or Spacebar - jump
* A, A - dash left 
* D, D - dash right 

#### Combat (keyboard)
* U - light punch (LP)
* I - heavy punch (HP)
* J - light kick (LK)
* K - heavy kick (HK)
* O - block

#### Combat (mouse)
* LMB - light punch (LP)
* RMB - heavy punch (HP)
* MMB - block

#### Combos
* HP -> LP -> HP - 3 hit combo
* LP -> HP -> LP -> HP - 4 hit combo

#### Special attack
* Backward -> Forward -> LP - Dash Punch
