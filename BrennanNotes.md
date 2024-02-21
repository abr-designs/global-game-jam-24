TODO
----------------
 - clamp maximum character velocity
 - character can squeeze behind door colliders on side
	- make sure doors are overlapping wall
 - Objects To Make
        scrolls
        rug
        pie
        books
		vase / statues    
 - Change input everywhere to use Input delegator events (especially in hands controller)

FIXES
------------
 - Level is now centered on 0 in the x-axis
	- just needed to move the === LEVEL === container as well as the king character
 - Marked some architecture prefabs as static (in case of baked lighting)
 - Turned off SM_Buildings colliders in scene (wall prefabs already have box colliders on them)
	- scaled wall prefabs colliders appropriately (added some extra height to prevent things going over the top)
 - Added shatterVFX and shatterSFX to the PropObject for when shattered event is triggered
	- this was used for crate/barrel dust + pieces effects
 - Added coins explosion from chest open prop event
 - Added slight spotlight for windows (starts toggled off but you can turn in on in a scene by checking it)
	- also added a decal-based option that can be enabled on the Window prefab
 - Made scoring text easier to see
	- Changed negative point popups to red
	- Adding a grid system to ensure popups spread out when stacked in the same area
 - Changed fonts on labels and buttons
 - Throwing reworked
	- Now the held object is set to kinematic with collisions disabled and follows the left hand transform in the update
	- Upon throw action the collider bounds calculate how far to move the object above the ground before applying force
		- object is set to "throw" layer which collides with everything except the character
		- once object hits anything else the layer is returned to "interactable"
	- Objects that are picked up now remove kinematic from all parents and children (as if they were bumped by the player)
		- we also need to unparent any child objects so that the bounds detection for the held item is correct
- Added VFX for stunned player
- Changed prop triggering to happening when a thrown object hits a prop as well
- Temporarily removed Push action (is a little confusing when throw using similar interaction)
- Changed guards from prop to character object, guards will stun and push player away when they get near
- Added scoring slider to track how close player is to winning
- Added early exit if player achieves target score early
- Added range circle indicator with timer to explosive barrels

BUGS
------------

PROPS
 - Chest had lid pivot target inverted (needed to be a positive number)
	- Depends on the x-axis direction
 - Sword needed proper colliders (replaced mesh with boxes)
 - Added DisallowMultipleComponent to PropObject and InteractableObject to prevent multiple components on same object
 - SwapShatteredMesh could trigger multiple times in a single fixed update if the object collided with several things at once (usually hit the floor and player)
 
MODELS
 - Coin had wrong pivot position
 - Added some coins to treasure chest base

VFX
 - Particle systems were being spawned with zero rotation applied and were facing parallel to the ground
	- redesigned particle system to work from 0,0,0 rotation
	- fixed red barrel explosion to scale with hierarchy so all child particles were correct size

Mac_Level_2
 - Guards still had 2 PropObject components attached in the prefab
	- this caused a crash whenever they were hit