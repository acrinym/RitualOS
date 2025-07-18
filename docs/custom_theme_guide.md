# Creating Custom Themes

Want to give RitualOS your own vibe? Follow these steps to craft a new theme:

1. Duplicate any `Theme.*.xaml` file in `src/Styles/Themes/` and rename it, e.g., `Theme.MyStyle.xaml`.
2. Edit the color brushes inside to match your palette.
3. Add the new file name to `AvailableThemes` in `ThemeViewModel.cs`.
4. Rebuild RitualOS and pick your theme from the Theme tab or the Theme Picker window.

That’s it—your colors, your magic! 🌟
