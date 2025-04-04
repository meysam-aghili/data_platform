using Bit.BlazorUI;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Shared;

public static class RazorExtension
{
    public static List<BitDropdownItem<T>> GetEnumValuesAsBitDropdownItems<T>() where T : Enum =>
        [..
            from T val in Enum.GetValues(typeof(T))
            select new BitDropdownItem<T>
            {
                Text = val.GetAttribute<DisplayAttribute>()?.Name ?? val.ToString(),
                Value = val
            }
        ];

    public static string GetDisplayName<T>(this T cls, string propertyName)
        where T : class =>
        cls.GetAttribute<DisplayAttribute, T>(propertyName)?.Name ?? propertyName;

    public static string GetDisplayDescription<T>(this T cls, string propertyName)
        where T : class =>
        cls.GetAttribute<DisplayAttribute, T>(propertyName)?.Description ?? string.Empty;

    public static string GetShortName<T>(this T cls, string propertyName)
        where T : class =>
        cls.GetAttribute<DisplayAttribute, T>(propertyName)?.ShortName ?? string.Empty;

    public static List<BitDropdownItem<T>> ToDropdownItems<T>(this IEnumerable<T> items) =>
        [..
            from item in items
            select new BitDropdownItem<T>
            {
                Text = item.ToString(),
                Value = item
            }
        ];
}
