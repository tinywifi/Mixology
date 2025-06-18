# crapette-party
A card game made with Photon on Unity. For Educational purpose.

# Pre-requisites
This game use two packages not included in this repository due to licence sharing.
- Photon PUN https://assetstore.unity.com/packages/tools/network/pun-2-free-119922
- Demigiant DOTWeen https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676

# How does it work ?
The goal is to show to students how Photon works and how to manage Multiplayer games.
This game is based on Crapette rules (see below) and can be played locally (same screen) or online.

If you play locally, one player use the mouse, the second use the keyboard : 
- AZERT for selecting a card
- Left/Right arrow to send the card on the stack
- Space to draw a new card

# Crapette rules
The game is prepared like this : 

|   Player 2 board  |  

| Stack L | Stack R |

|   Player 1 board  |

Each player plays with half of the deck.

## Step 0 : Preparation 
They prepare their board like this : 

1 card | 2 cards | 3 cards | 4 cards | 5 cards

in front of them. The other cards are placed next to the stack.

## Step 1 : Start the game 
When ready, both players draw a card and place it on the stack of their side.

## Step 2 : Play
Players have to empty their board.
They have to place their cards on top of any stack, if they are the next, or the previous value of the actual card.
For example : 
- If you have a 8 on Stack R, you can place a 7 or a 9
- The color of the card doesn't count
If no player can continue, come back to Step 1

## Step 3 : End of the game
The game doesn't follow the regular rules. Here, the first player who empty his board wins.

On a regular game, when your board is empty you have to tap one stack, and add it to your deck. Then you restart from Step 0.
If your deck doesn't have enough cards for the preparation : you win.

# Known issues
- This game works at least for one run. But it can fail if you try to restart.
- The game doesn't handle conflicts (when two players try to put a card at the same time, on the same stack)

Enjoy !
