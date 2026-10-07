// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;
using Mud.HttpUtils.Payloads;

namespace Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads;

/// <summary>
/// 公众号回调载荷字段转换器（<c>[PayloadContract(Converter = typeof(MpPayloadConverter))]</c> 的转换方法集）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何各产品线各持一份</b>：转换器是「上游 <c>PayloadNode</c> → 载荷属性」的<b>产品线语义适配</b> —
/// 企微侧携带 <c>WechatUserGender</c>/<c>WechatUserStatus</c> 等领域枚举解析，公众号侧字段集与值域不同；
/// 转换器本身不涉及加解密/验签等安全面（安全面已单源下沉叶层），故不做跨产品线共享。
/// </para>
/// <para>
/// 方法签名约束（上游生成器契约）：<c>static</c>、非泛型（泛型的 <c>Number&lt;T&gt;</c> 通过 <c>Method</c> 通道声明）、
/// 恰 1 个 <see cref="PayloadNode"/> 参数。
/// </para>
/// </remarks>
public static class MpPayloadConverter
{
    /// <summary>标量文本；节点缺失或空 ⇒ <c>null</c>。</summary>
    /// <param name="node">字段节点。</param>
    /// <returns>文本值或 <c>null</c>。</returns>
    public static string? Text(PayloadNode? node)
    {
        var text = node?.Value;
        return string.IsNullOrEmpty(text) ? null : text;
    }

    /// <summary>数值（官方以字符串承载）；非法 ⇒ <c>null</c>。</summary>
    /// <typeparam name="T">目标数值类型。</typeparam>
    /// <param name="node">字段节点。</param>
    /// <returns>数值或 <c>null</c>。</returns>
    public static T? Number<T>(PayloadNode? node)
        where T : struct
    {
        var text = node?.Value;
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        if (typeof(T) == typeof(int)
            && int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue))
        {
            return (T)(object)intValue;
        }

        if (typeof(T) == typeof(long)
            && long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longValue))
        {
            return (T)(object)longValue;
        }

        return null;
    }

    /// <summary>布尔标量（官方以 0/1 承载）；非法 ⇒ <c>null</c>。</summary>
    /// <typeparam name="T">布尔类型（<c>bool</c>）。</typeparam>
    /// <param name="node">字段节点。</param>
    /// <returns>布尔值或 <c>null</c>。</returns>
    public static T? Flag<T>(PayloadNode? node)
        where T : struct
    {
        var text = node?.Value;
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        if (typeof(T) == typeof(bool))
        {
            if (text == "1" || string.Equals(text, "true", StringComparison.OrdinalIgnoreCase))
            {
                return (T)(object)true;
            }

            if (text == "0" || string.Equals(text, "false", StringComparison.OrdinalIgnoreCase))
            {
                return (T)(object)false;
            }
        }

        return null;
    }

    /// <summary>浮点标量（官方存在如经纬度的小数形态）；非法 ⇒ <c>null</c>。</summary>
    /// <param name="node">字段节点。</param>
    /// <returns>浮点值或 <c>null</c>。</returns>
    public static double? ParseReal(PayloadNode? node)
    {
        var text = node?.Value;
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
            ? value
            : (double?)null;
    }
}
