// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using Mud.AI.Agent.Tools;

namespace Mud.Wechat.Work.AI.Tools;

/// <summary>
/// 生成物 Args 解包器使用的入参读取助手：把模型给来的 <c>IReadOnlyDictionary&lt;string, object?&gt;</c>
/// 按声明类型取成 CLR 值（缺失 / 类型不符 / 闭集非法值给出可读错误）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么要产品侧提供</b>：引擎只发射「调用哪个 helper」的代码（<c>ToolArgs.RequireInt(args, "…")</c>），
/// 而 helper 的实现在 owner 程序集——这是「引擎与产品解耦」的接缝：引擎不认识任何产品类型，
/// 产品也不必继承引擎的内部基类。
/// </para>
/// <para>
/// <b>helper 集与引擎映射表一一对应</b>：<c>string</c> / <c>string[]</c> / <c>int</c> / <c>bool?</c> /
/// <c>TEnum</c> / <c>int</c>（常量类闭集按名映射）六个形态各一对 Require/Optional；引擎若新增形态，
/// 编译期即以 <c>CS0117</c> 暴露（而不是运行期静默取到默认值）。
/// </para>
/// <para>
/// <b>错误类型</b>：一律 <see cref="ArgumentException"/>（参数类错误，执行链分类为 <c>Validation/invalid_args</c>）；
/// 闭集非法值额外用 <see cref="ToolArgumentException"/> 携带合法取值清单，让模型可直接纠错。
/// </para>
/// </remarks>
internal static class ToolArgs
{
    /// <summary>读取必填字符串。</summary>
    /// <param name="args">入参表。</param>
    /// <param name="name">参数名（snake_case，模型可见）。</param>
    /// <returns>非空字符串。</returns>
    /// <exception cref="ArgumentException">缺失、空白或类型不符。</exception>
    public static string RequireString(IReadOnlyDictionary<string, object?> args, string name)
    {
        var value = OptionalString(args, name);
        return value ?? throw new ArgumentException($"缺少必填参数 {name}（应为非空字符串）。", nameof(args));
    }

    /// <summary>读取可选字符串。</summary>
    /// <param name="args">入参表。</param>
    /// <param name="name">参数名。</param>
    /// <returns>字符串；缺失或空白返回 <see langword="null"/>。</returns>
    /// <exception cref="ArgumentException">存在但类型不符。</exception>
    public static string? OptionalString(IReadOnlyDictionary<string, object?> args, string name)
    {
        var raw = Read(args, name);
        if (raw is null)
        {
            return null;
        }

        var text = raw as string ?? (raw is JsonElement element && element.ValueKind == JsonValueKind.String
            ? element.GetString()
            : throw new ArgumentException($"参数 {name} 应为字符串。", nameof(args)));

        return string.IsNullOrWhiteSpace(text) ? null : text;
    }

    /// <summary>读取必填字符串数组。</summary>
    /// <param name="args">入参表。</param>
    /// <param name="name">参数名。</param>
    /// <returns>非空数组。</returns>
    /// <exception cref="ArgumentException">缺失、为空或类型不符。</exception>
    public static string[] RequireStringArray(IReadOnlyDictionary<string, object?> args, string name)
    {
        var value = OptionalStringArray(args, name);
        return value is { Length: > 0 }
            ? value
            : throw new ArgumentException($"缺少必填参数 {name}（应为非空字符串数组）。", nameof(args));
    }

    /// <summary>读取可选字符串数组。</summary>
    /// <param name="args">入参表。</param>
    /// <param name="name">参数名。</param>
    /// <returns>数组（可空）。</returns>
    /// <exception cref="ArgumentException">存在但类型不符。</exception>
    public static string[]? OptionalStringArray(IReadOnlyDictionary<string, object?> args, string name)
    {
        var raw = Read(args, name);
        if (raw is null)
        {
            return null;
        }

        var items = new List<string>();
        switch (raw)
        {
            case string single when !string.IsNullOrWhiteSpace(single):
                return [single];
            case string:
                return null;
            case IEnumerable<string> strings:
                items.AddRange(strings);
                break;
            case JsonElement element when element.ValueKind == JsonValueKind.Array:
                foreach (var item in element.EnumerateArray())
                {
                    var text = item.ValueKind == JsonValueKind.String ? item.GetString() : null;
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        items.Add(text!);
                    }
                }

                break;
            case IEnumerable<object?> objects:
                items.AddRange(objects.OfType<string>().Where(static s => !string.IsNullOrWhiteSpace(s)));
                break;
            default:
                throw new ArgumentException($"参数 {name} 应为字符串数组。", nameof(args));
        }

        return items.Count == 0 ? null : [.. items];
    }

    /// <summary>读取必填整数。</summary>
    /// <param name="args">入参表。</param>
    /// <param name="name">参数名。</param>
    /// <returns>整数。</returns>
    /// <exception cref="ArgumentException">缺失或不是整数。</exception>
    public static int RequireInt(IReadOnlyDictionary<string, object?> args, string name)
        => OptionalInt(args, name) ?? throw new ArgumentException($"缺少必填参数 {name}（应为整数）。", nameof(args));

    /// <summary>读取可选整数。</summary>
    /// <param name="args">入参表。</param>
    /// <param name="name">参数名。</param>
    /// <returns>整数；缺失返回 <see langword="null"/>。</returns>
    /// <exception cref="ArgumentException">存在但不是整数。</exception>
    public static int? OptionalInt(IReadOnlyDictionary<string, object?> args, string name)
    {
        var raw = Read(args, name);
        if (raw is null || (raw is string text && string.IsNullOrWhiteSpace(text)))
        {
            return null;
        }

        return TryToInt64(raw, out var value) && value is >= int.MinValue and <= int.MaxValue
            ? (int)value
            : throw new ArgumentException($"参数 {name} 应为整数。", nameof(args));
    }

    /// <summary>读取可选布尔。</summary>
    /// <param name="args">入参表。</param>
    /// <param name="name">参数名。</param>
    /// <returns>布尔；缺失返回 <see langword="null"/>。</returns>
    /// <exception cref="ArgumentException">存在但不是布尔。</exception>
    public static bool? OptionalBool(IReadOnlyDictionary<string, object?> args, string name)
    {
        var raw = Read(args, name);
        switch (raw)
        {
            case null:
                return null;
            case bool value:
                return value;
            case JsonElement element when element.ValueKind is JsonValueKind.True or JsonValueKind.False:
                return element.GetBoolean();
            case string text when bool.TryParse(text, out var parsed):
                return parsed;
            case string text when text is "1" or "0":
                return text == "1";
            default:
                throw new ArgumentException($"参数 {name} 应为布尔值（true/false）。", nameof(args));
        }
    }

    /// <summary>读取必填枚举（按成员名解析，忽略大小写）。</summary>
    /// <typeparam name="TEnum">枚举类型。</typeparam>
    /// <param name="args">入参表。</param>
    /// <param name="name">参数名。</param>
    /// <returns>枚举值。</returns>
    /// <exception cref="ToolArgumentException">缺失或取值不在闭集内（附合法取值清单）。</exception>
    public static TEnum RequireEnum<TEnum>(IReadOnlyDictionary<string, object?> args, string name)
        where TEnum : struct, Enum
        => OptionalEnum<TEnum>(args, name)
           ?? throw new ToolArgumentException(
               $"缺少必填参数 {name}（合法取值：{string.Join("/", Enum.GetNames(typeof(TEnum)))}）。",
               Enum.GetNames(typeof(TEnum)));

    /// <summary>读取可选枚举（按成员名解析，忽略大小写；空白视为缺失）。</summary>
    /// <typeparam name="TEnum">枚举类型。</typeparam>
    /// <param name="args">入参表。</param>
    /// <param name="name">参数名。</param>
    /// <returns>枚举值；缺失返回 <see langword="null"/>。</returns>
    /// <exception cref="ToolArgumentException">取值不在闭集内（附合法取值清单）。</exception>
    public static TEnum? OptionalEnum<TEnum>(IReadOnlyDictionary<string, object?> args, string name)
        where TEnum : struct, Enum
    {
        var raw = OptionalString(args, name);
        if (raw is null)
        {
            return null;
        }

        // Enum.TryParse<TEnum>(string, bool, out) / Enum.IsDefined<TEnum> 自 net5.0 起才提供：
        // 四档统一走非泛型重载（语义一致：忽略大小写按成员名解析，且必须在闭集内）。
        TEnum parsed;
        try
        {
            parsed = (TEnum)Enum.Parse(typeof(TEnum), raw, ignoreCase: true);
        }
        catch (ArgumentException)
        {
            throw new ToolArgumentException(
                $"参数 {name} 的取值 '{raw}' 非法（合法取值：{string.Join("/", Enum.GetNames(typeof(TEnum)))}）。",
                Enum.GetNames(typeof(TEnum)));
        }

        if (Enum.IsDefined(typeof(TEnum), parsed))
        {
            return parsed;
        }

        throw new ToolArgumentException(
            $"参数 {name} 的取值 '{raw}' 非法（合法取值：{string.Join("/", Enum.GetNames(typeof(TEnum)))}）。",
            Enum.GetNames(typeof(TEnum)));
    }

    /// <summary>读取必填「常量名 → 整型平台值」闭集参数。</summary>
    /// <param name="args">入参表。</param>
    /// <param name="name">参数名。</param>
    /// <param name="closedSet">闭集映射（成员名 → 平台值）。</param>
    /// <returns>平台值。</returns>
    /// <exception cref="ToolArgumentException">缺失或取值不在闭集内（附合法取值清单）。</exception>
    public static int RequireNamedInt(
        IReadOnlyDictionary<string, object?> args,
        string name,
        IReadOnlyDictionary<string, int> closedSet)
        => OptionalNamedInt(args, name, closedSet)
           ?? throw new ToolArgumentException(
               $"缺少必填参数 {name}（合法取值：{string.Join("/", closedSet.Keys)}）。",
               [.. closedSet.Keys]);

    /// <summary>读取可选「常量名 → 整型平台值」闭集参数（也接受闭集内的裸数值）。</summary>
    /// <param name="args">入参表。</param>
    /// <param name="name">参数名。</param>
    /// <param name="closedSet">闭集映射（成员名 → 平台值）。</param>
    /// <returns>平台值；缺失返回 <see langword="null"/>。</returns>
    /// <exception cref="ToolArgumentException">取值不在闭集内（附合法取值清单）。</exception>
    public static int? OptionalNamedInt(
        IReadOnlyDictionary<string, object?> args,
        string name,
        IReadOnlyDictionary<string, int> closedSet)
    {
        var raw = Read(args, name);
        if (raw is null)
        {
            return null;
        }

        if (raw is string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            if (closedSet.TryGetValue(text, out var named))
            {
                return named;
            }
        }

        if (TryToInt64(raw, out var numeric) && closedSet.Values.Contains((int)numeric))
        {
            return (int)numeric;
        }

        throw new ToolArgumentException(
            $"参数 {name} 的取值非法（合法取值：{string.Join("/", closedSet.Keys)}）。",
            [.. closedSet.Keys]);
    }

    private static object? Read(IReadOnlyDictionary<string, object?> args, string name)
        => args.TryGetValue(name, out var value) ? value : null;

    private static bool TryToInt64(object raw, out long value)
    {
        switch (raw)
        {
            case int intValue:
                value = intValue;
                return true;
            case long longValue:
                value = longValue;
                return true;
            case JsonElement element when element.ValueKind == JsonValueKind.Number:
                return element.TryGetInt64(out value);
            case JsonElement element when element.ValueKind == JsonValueKind.String:
                return long.TryParse(element.GetString(), out value);
            case string text:
                return long.TryParse(text, out value);
            default:
                value = 0;
                return false;
        }
    }
}
