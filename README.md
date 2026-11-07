# Slot Machine

A simple Unity WebGL slot machine game with weighted symbols, animated reels,
bet selection, balance tracking, and triple-symbol payouts.

## Run the WebGL build

1. Open the hosted repository and download the contents of `Build/WebGL/Build2`.
2. Serve that folder with a local web server. For example:

   ```bash
   python3 -m http.server 8000 --directory Build/WebGL/Build2
   ```

3. Open `http://localhost:8000` in a browser.

Opening `index.html` directly may be blocked by browser security rules, so use
a local web server.

## Project structure

- `Assets/Scripts` - gameplay, reel, lever, and UI scripts
- `Assets/Data` - symbol configuration assets
- `Assets/UI` - game artwork and UI sprites
- `Assets/Scenes` - the playable Unity scene
- `Build/WebGL` - the compressed WebGL submission build

The project uses scene-based UI and animation. Separate prefabs are not
required for this single-scene game.

## Bonus features

- Weighted random symbol selection
- Configurable symbol payout multipliers
- Reusable reel slot images instead of creating objects during spins
- Animated betting popup and lever

## Approach

Game rules are separated from presentation: `SpinResolver` decides the result
before animation starts, while the controllers handle visuals and input.
Symbol values are stored as ScriptableObject assets so game balancing can be
changed in the Unity Editor without changing code.
