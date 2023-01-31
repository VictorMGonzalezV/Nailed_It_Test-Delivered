# Nailed_It_Test-Delivered

I was asked to do the following:

Remove the following SDKs: IronSrc, GameAnalytics, Tenjin, Facebook and Firebase. 

Remove URP and make sure the game is playable with the built in render pipeline

Make sure the game does not show any advertisement (banners, interstitial, cross promo)

The game must always boot up into the 5th level in the game. The game should not be playable any further than the 5th level.

The game must be immediately playable and not show the home screen with upgrade buttons, shop button and settings button etc.

The player must not play with the default gun, but immediately play with the “crossbow” weapon 

Remove as many assets as needed to strip down the game’s size

The game must not show the default win screen when clearing a level. Instead show the app icon in the middle of the screen with a button below it with the text “Install Now”. When tapping on this button, the game should open the link to http://shouldnotrevealcompanyname.com/ (the app icon is included in the root path of the zip file called “playable_assets.zip”). 

Bonus assignment: add a new feature to the game - when the player makes a headshot, the enemy’s head must detach from its body

I managed to do everything, including the bonus assignment, which required a creative solution: opening the enemy mesh in Blender to separate the head and store it as a separate mesh, which I then added to the nail prefab and made it invisible.
Then when the player scores a headshot, the enemy's head bone is scaled down to 0,0,0 to make it seem to disappear, while the head already attached to the nail becomes active.
This way I solved the issue with breaking the head joint when scoring a headshot, which made the model's skin stretch along the path of the nail.

The comments not marked with my initials were already there, maybe from the company devs or previous applicants.

The game cannot be properly built for Android (it wasn't meant to), and in order to play it it must be launched from Assets->Scenes->Init so the level initialization occurs.
