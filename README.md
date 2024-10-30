# Children Grow Faster Mod

## Overview

The Children Grow Faster mod for Mount & Blade II: Bannerlord accelerates the growth rate of children in the game. This mod allows players to customize the growth rate of their children and other children in the game, providing a more dynamic and engaging gameplay experience.

## Features

- **Customizable Growth Rate**: Adjust the growth rate of children through the mod settings.
- **Player's Children Focus**: Option to apply the growth rate only to the player's children.
- **Instant Growth**: Instantly age up children based on the configured growth rate.
- **Random Traits**: Randomly assign traits to the player's children.
- **Spouse Events**: Adds a chance for spouse-related events if the spouse is not pregnant.

## Installation

1. Download the mod files.
2. Extract the contents to the `Modules` folder in your Mount & Blade II: Bannerlord installation directory.
3. Enable the mod in the Bannerlord launcher.


### Configuration

The mod settings can be configured through the Mod Configuration Menu (MCM). You can adjust the following settings:

- **Affect Only Player Children**: Toggle whether the growth rate applies only to the player's children.
- **New Growth Rate**: Set the new growth rate for children.
- **Instant Growth**: Toggle instant growth for children.

### Events

- **Daily Tick Event**: The mod applies the growth rate to children during the daily tick event.
- **Spouse Event**: There is a 5% chance of the player's spouse being kidnapped by bandits if she is not pregnant and the main hero is not in the same settlement.
**More Coming Soon**: More events will be implemented later. 



### Project Structure

- **SubModule.cs**: Contains the main logic for the mod, including event handlers and growth rate application.
- **SubModulePatches.cs**: Contains patches for the game components.
- **SubModuleDebugging.cs**: Contains debugging utilities for the mod.
- **SubModuleSettings.cs**: Contains the settings for the mod.

### Dependencies

The mod relies on several libraries and packages:

- **Bannerlord.MCM**: For mod configuration.
- **Bannerlord.Lib.Harmony**: For patching game methods.
- **Bannerlord.ReferenceAssemblies**: Reference assemblies for various game modules.

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request with your changes.

## License

This mod is licensed under the MIT License. See the [LICENSE](LICENSE) file for more information.

## Contact

For support or inquiries, please contact me on [NexusMods](https://next.nexusmods.com/profile/BuntaFFR) or [Discord](https://discord.gg/D7gwcMzT2K).
