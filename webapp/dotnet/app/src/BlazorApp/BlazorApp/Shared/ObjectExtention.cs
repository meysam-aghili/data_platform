using System.Text.Json;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Text.Json.Nodes;
using System.Reflection;
using System.Linq.Expressions;
using Novell.Directory.Ldap;


namespace BlazorApp.Shared;

public static class ObjectExtension
{
    public static T RandomElement<T>(this List<T> listOfElements)
    {
        return listOfElements[Random.Shared.Next(listOfElements.Count)];
    }

    public static string ToJson<T>(this T data) =>
        JsonSerializer.Serialize(data);

    public static void Dump<T>(this T data) =>
        Debug.WriteLine(data.ToJson());

    public static string ToSHA256(this string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        var sb = new StringBuilder();
        foreach (var b in bytes)
        {
            sb.Append(b.ToString("X2"));
        }
        return sb.ToString();
    }

    public static string ToSlug(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }
        var slug = input.Normalize(NormalizationForm.FormD);
        char[] chars = slug
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) !=
                UnicodeCategory.NonSpacingMark).ToArray();
        slug = new string(chars);
        slug = slug.ToLower();
        slug = Regex.Replace(slug, @"[^A-Za-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", " ").Trim();
        slug = Regex.Replace(slug, @"\s", "-");
        return slug;
    }

    public static string ToSlugUnderlined(this string input) => input.ToSlug().Replace("-", "_");

    public static string ToHex(this string input) =>
        Convert.ToHexString(Encoding.UTF8.GetBytes(input));

    public static string XORCipher(this string data, string key)
    {
        int dataLength = data.Length;
        int keyLength = key.Length;
        char[] output = new char[dataLength];

        for (var i = 0; i < dataLength; i++)
        {
            output[i] = (char)(data[i] ^ key[i % keyLength]);
        }

        return new string(output);
    }

    public static string FromHex(this string? input)
    {
        if (input == null)
        {
            return string.Empty;
        }
        var bytes = new byte[input.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(input.Substring(i * 2, 2), 16);
        }
        return Encoding.Unicode.GetString(bytes);
    }

    public static string ToBsonId(this string input) =>
        input.ToSlug().ToHex();

    public static string ToCommaSeparatedString(this int input)
    {
        var asString = input.ToString();
        var commaSeparatedString = string.Empty;
        for (int index = asString.Length - 1; index >= 0; index--)
        {
            commaSeparatedString += asString[asString.Length - 1 - index]; ;
            if (index % 3 == 0 && index != 0)
            {
                commaSeparatedString += ",";
            }
        }
        return commaSeparatedString;
    }

    public static string GetDescription(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());
        var attributes = fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes?.Length > 0 ? ((DescriptionAttribute)attributes[0]).Description : value.ToString();
    }

    public static string? ToQueryString(this Dictionary<string, object>? payload)
    {
        if (payload is null)
        {
            return null;
        }
        string queryString = "?";
        foreach (var field in payload.Keys)
        {
            queryString += $"{field}={payload[field]}&";
        }
        queryString = queryString[..(queryString.Length - 1)];
        return queryString;
    }

    public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(items);

        if (list is List<T> asList)
        {
            asList.AddRange(items);
        }
        else
        {
            foreach (T item in items)
            {
                list.Add(item);
            }
        }
    }

    public static async Task<JsonNode> ParseAsJsonAsync(this HttpContent payload)
    {
        var payloadStream = await payload.ReadAsStreamAsync() ?? throw new Exception("Payload is empty.");
        return JsonNode.Parse(payloadStream) ?? throw new Exception("Couldn't parse Payload.");
    }

    public static T? GetAttribute<T>(this Enum enumValue) where T : Attribute =>
        enumValue
            .GetType()
            .GetMember(enumValue.ToString())
            .First()
            .GetCustomAttribute<T>();

    public static TAttribute? GetAttribute<TAttribute, TType>(this TType cls, string propertyName)
        where TAttribute : Attribute
        where TType : class
    {
        TAttribute? attribute = cls
            .GetType()
            .GetProperty(propertyName)?
            .GetCustomAttributes(typeof(TAttribute), true)
            .FirstOrDefault()
            as TAttribute;
        return attribute;
    }

    public static TValue? GetAttribute<T, TOut, TAttribute, TValue>(
        Expression<Func<T, TOut>> propertyExpression,
        Func<TAttribute, TValue> valueSelector
        ) where TAttribute : Attribute
    {
        var expression = (MemberExpression)propertyExpression.Body;
        var propertyInfo = (PropertyInfo)expression.Member;
        return
            propertyInfo.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() is TAttribute attr ?
            valueSelector(attr) :
            default;
    }

    //public static TAttributeValue BiGetAttribute<T, TAttribute, TAttributeValue>(this T insp, Func<T, object> propertySelector) where T : class
    //{
    //    var props = typeof(T).GetProperties();
    //    var prop = from p in props where nameof(p) == select p
    //}

    //public static TAttributeValue BiGetAttribute<TAttribute, TAttributeValue>(
    //    Type classType, Expression<Func<Type, object>> propertySelector) where TAttribute : Attribute where T : class
    //{
    //    if (classType == null || propertySelector == null)
    //    {
    //        throw new ArgumentNullException();
    //    }

    //    PropertyInfo propertyInfo = GetPropertyInfo(propertySelector)
    //        ?? throw new ArgumentException("Invalid property selector");
    //    TAttribute attribute = propertyInfo.GetCustomAttributes<TAttribute>(false).First()
    //        ?? throw new ArgumentException("Attribute not found on the selected property");
    //    if (attribute is not TAttributeValue value)
    //        throw new InvalidCastException("Attribute value cannot be cast to TAttributeValue");

    //    return value;
    //}

    //private static PropertyInfo? GetPropertyInfo<T>(Expression<Func<T, object>> propertySelector)
    //{
    //    var member = propertySelector.GetType()
    //        .GetMember(((MemberExpression)propertySelector.Body).Member.Name)
    //        .FirstOrDefault();

    //    return member as PropertyInfo;
    //}

    //public static TAttribute? Fuck<T, TAttribute>(Expression<Func<T, object>> propSelector) where TAttribute : Attribute =>
    //typeof(T).GetProperty(nameof(propSelector)).GetCustomAttribute<TAttribute>() ?? null;

    public static TEnum AsEnumValue<TEnum>(this string enumStringValue) where TEnum : Enum =>
        (TEnum)Enum.Parse(typeof(TEnum), enumStringValue, true);

    public static TAttribute? GetPropertyAttribute<T, TAttribute>(string propertyName) where TAttribute : Attribute
    {
        var prop = typeof(T).GetProperty(propertyName) ?? throw new Exception($"{propertyName} not found.");
        var attr = prop.GetCustomAttribute<TAttribute>();
        return attr;
    }

    public static LdapAttribute? GetAttributeOrNull(this LdapEntry entry, string attr) =>
        entry.GetAttributeSet().TryGetValue(attr, out var value) ? value : null;

    public static Stream ToStream(this string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    public static async Task<string> ToBase64StringAsync(this Stream stream)
    {
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        return Convert.ToBase64String(ms.ToArray());
    }

    public static string ToBase64String(this string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str);
        return Convert.ToBase64String(bytes);
    }

    public static string FromBase64(this string s)
    {
        byte[] data = Convert.FromBase64String(s);
        return Encoding.UTF8.GetString(data);
    }

    public static Dictionary<TKey, TValue> AddRange<TKey, TValue>(this Dictionary<TKey, TValue> leftDict, Dictionary<TKey, TValue> rightDict)
    {
        Dictionary<TKey, TValue>[] dicts = [leftDict, rightDict];
        return dicts.SelectMany(dict => dict).ToDictionary();
    }

    public static TAttribute GetEnumAttribute<TAttribute>(this Enum enumValue) where TAttribute : Attribute
    {
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
        var attributes = (TAttribute[])(fieldInfo?.GetCustomAttributes(typeof(TAttribute)) ?? []);
        return attributes.First();
    }
}
