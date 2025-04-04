using Bit.BlazorUI;
using MudBlazor;


namespace BlazorApp.Components.Layout;

public partial class MainLayout
{
    public static MudTheme AppMudTheme { get; set; } = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = AppsPalette.LightPrimary,
            Background = AppsPalette.LightBackground,
            DrawerBackground = AppsPalette.LightBackgroundShade1,
            AppbarBackground = AppsPalette.LightBackgroundShade1,
            Info = AppsPalette.LightPrimary
        },
        PaletteDark = new PaletteDark
        {
            Primary = AppsPalette.DarkPrimary,
            Background = AppsPalette.DarkBackground,
            DrawerBackground = AppsPalette.DarkBackgroundShade1,
            AppbarBackground = AppsPalette.DarkBackgroundShade1,
            Info = AppsPalette.DarkPrimary
        }
    };

    public static BitTheme AppBitLightTheme { get; set; } = new()
    {
        Color =
        {
            Primary =
            {
                Main = AppsPalette.LightPrimary,
                MainHover = AppsPalette.LightPrimaryShade1,
                MainActive = AppsPalette.LightPrimaryShade2
            },
            Background =
            {
                Primary = AppsPalette.LightBackground,
                Secondary = AppsPalette.LightBackgroundShade1
            }
        }
    };

    public static BitTheme AppBitDarkTheme { get; set; } = new()
    {
        Color =
        {
            Primary =
            {
                Main = AppsPalette.DarkPrimary,
                MainHover = AppsPalette.DarkPrimaryShade1,
                MainActive = AppsPalette.DarkPrimaryShade2,
            },
            Background =
            {
                Primary = AppsPalette.DarkBackground,
                Secondary = AppsPalette.DarkBackgroundShade1,
                Disabled = AppsPalette.LightBackgroundShade2
            }
        }
    };
}

public static class AppsPalette
{
    public static string LightBackground = "#ffffff";
    public static string LightBackgroundShade1 = "#faf9f8";
    public static string LightBackgroundShade2 = "#e7e2dd";
    public static string LightPrimary = "#df110d";
    public static string LightPrimaryShade1 = "#d20002";
    public static string LightPrimaryShade2 = "#c50000";

    public static string DarkBackground = "#373740";
    public static string DarkBackgroundShade1 = "#27272f";
    public static string DarkBackgroundShade2 = "#17171b";
    public static string DarkPrimary = "#e0930a";
    public static string DarkPrimaryShade1 = "#dd8805";
    public static string DarkPrimaryShade2 = "#d77a00";
}
