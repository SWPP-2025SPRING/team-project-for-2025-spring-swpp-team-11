# Guidance Line Documentation

"Guidance Line" - Version 1.0.0 by CesarG

1. Introduction  
2. Installation  
3. Asset Includes  
4. Setup Guide
5. License  
6. Feedback and Support  


1. Introduction
Guidance Line is a tool to help developers create dynamic guide lines for players to navigate levels. This documentation will guide you through installation, setup, and customization.

2. Installation

From the Unity Asset store asset page, click "Add to my Assets", or if you have the editor open: Window > Package Manager > Sort by Packages: My Assets > Select "Guidance Line" > Download > Import

3. Asset Includes

- The 2 scripts required to make the Guidance Line Work + a custom editor script for setting up the Guidance Line
- 2 Custom Shaders + 2 materials for the Line Renderer's material
- An example scene with an already setup Guidance Line to test it out (a blocked out example level with basic shapes and materials)
- A Guidance Line Prefab + a player Prefab for the example scene


4. Setup Guide

For a detailed setup and tutorial video, go wacth the video - > https://www.youtube.com/watch?v=-939Ee6BJXA

	1- In your scene, either add the GuidanceLine prefab or create an empty GameObject to which you must add the GuidanceLine script AND a Line Renderer.

	2- Toggle on/off the line's gizmos at your will. It is recommended to keep them on for better understanding and visualization

	3- Setup the line's parameters: choose the width of the line, drag the start point of the line (usually the player) and the end point (the target or desired location the player has to go to)

	4- Check Points: Add and remove check points with the 2 buttons. Place them in your level in critical points of the path the player has to go through (doorways, corridors, stairs, etc...).The more the better detailed the line will be.

	5- Adjust CheckPoint Distance Threshold: dictates the distance at which a check point or the end point are considered as "reached"

	6- Adjust Points Per Segment: dictates the number of points the line will go through between each check points. A value too low will result in a line that is not smooth looking

	7- "Visualize Guidance Line" & "Stop Visualizing": these buttons will add or remove the visual of the line in the editor

	8- IF YOU ADDED CHECKPOINTS: Follow the following setup:

		8a - Toggle the Gizmos at your will
		8b - Ensure the Player field is not empty
		8c - MANUALLY add the next checkpoint (or end point in the case of the last check point) this allows the checkpoint to move dynamically and gives life to the Guidance Line
		8d - Adjust move speed: dictates the reactivity of the line to the player's movements
		8e - Adjust the radius: dictates how far the checkpoint can move away from the position it was placed at initially

	9- You can change the line's material, duplicate them to have different looks and colors when selecting the material itself

5. License

You may use this asset freely in your projects as long as you credit me if you deploy / publish your project

6. Feedback and bug report

Contact me for questions or bug report. Hope to receive feedback from everyone. Thank you! 

[My Asset Store Profile](https://assetstore.unity.com/publishers/109534?preview=1)
