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

FIXES
------------
 - Level is now centered on 0 in the x-axis
	- just needed to move the === LEVEL === container as well as the king character
 - Marked some architecture prefabs as static (in case of baked lighting)
 - Turned on SM_Buildings colliders in scene (wall prefabs already have box colliders on them)
	- scaled wall prefabs colliders appropriately (added some extra height to prevent things going over the top)
 - Added shatterVFX and shatterSFX to the PropObject for when shattered event is triggered
	- this was used for crate/barrel dust + pieces effects
 - Added coins explosion from chest open prop event
 - Added slight spotlight for windows (starts toggled off but you can turn in on in a scene by checking it)
	- also added a decal-based option that can be enabled on the Window prefab
 - Made scoring text easier to see
 - Changed fonts on labels and buttons

BUGS
------------

 - Throwing has issues
	- object collides with character if thrown through the body
 	- when giving object mass it will trigger impulse collisions that break the object while held
		- getting good results setting the object to a lighter value but adding some drag to reduce the jumpiness
		- TODO -- use "proxy" object with a rigidbody of mass 1 and 1000 drag + angular drag to make it look like the character is dragging the object around, take the main object out of collision and only use it's visual mesh ( can move it to a separate layer )

PROPS
 - Chest had lid pivot target inverted (needed to be a positive number)
	- Depends on the x-axis direction
 - Sword needed proper colliders (replaced mesh with boxes)

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