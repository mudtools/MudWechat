// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Callback;

/// <summary>
/// APIv3 通知的四个签名相关请求头（<c>Wechatpay-Timestamp</c> / <c>-Nonce</c> / <c>-Signature</c> / <c>-Serial</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何独立成值对象</b>：接收器的验签输入与 ASP.NET 无关 —— 有了本类型，验签 / 时效 / 重放三道闸
/// 可以在<b>不构造 HttpContext</b> 的前提下被单元测试（否则只能起集成宿主，闸的边界用例会大面积漏测）。
/// </para>
/// <para><b>红线（PAY-B7）</b>：<see cref="Signature"/> <b>不得</b>入日志；本类型不提供任何回显 / ToString 重写。</para>
/// </remarks>
public readonly struct WechatPayCallbackHeaders
{
    /// <summary>初始化请求头值对象。</summary>
    /// <param name="timestamp"><c>Wechatpay-Timestamp</c> 原文。</param>
    /// <param name="nonce"><c>Wechatpay-Nonce</c> 原文。</param>
    /// <param name="signature"><c>Wechatpay-Signature</c>（Base64，<b>不得入日志</b>）。</param>
    /// <param name="serialNumber"><c>Wechatpay-Serial</c>（平台证书序列号，或公钥模式的 <c>PUB_KEY_ID_…</c>）。</param>
    public WechatPayCallbackHeaders(string? timestamp, string? nonce, string? signature, string? serialNumber)
    {
        Timestamp = timestamp;
        Nonce = nonce;
        Signature = signature;
        SerialNumber = serialNumber;
    }

    /// <summary><c>Wechatpay-Timestamp</c> 原文（保持字符串，不得先转数值再格式化）。</summary>
    public string? Timestamp { get; }

    /// <summary><c>Wechatpay-Nonce</c> 原文。</summary>
    public string? Nonce { get; }

    /// <summary><c>Wechatpay-Signature</c>（Base64）。<b>不得入日志</b>。</summary>
    public string? Signature { get; }

    /// <summary><c>Wechatpay-Serial</c>（平台证书序列号 / 公钥模式 ID）。</summary>
    public string? SerialNumber { get; }

    /// <summary>从 ASP.NET 请求头集合读取四个值（缺失即 <c>null</c>，由接收器按缺失类别拒绝）。</summary>
    /// <param name="headers">ASP.NET 请求头集合。</param>
    /// <returns>请求头值对象。</returns>
    public static WechatPayCallbackHeaders From(IHeaderDictionary headers)
    {
        if (headers is null)
        {
            throw new ArgumentNullException(nameof(headers));
        }

        return new WechatPayCallbackHeaders(
            First(headers, "Wechatpay-Timestamp"),
            First(headers, "Wechatpay-Nonce"),
            First(headers, "Wechatpay-Signature"),
            First(headers, "Wechatpay-Serial"));
    }

    private static string? First(IHeaderDictionary headers, string name)
    {
        if (!headers.TryGetValue(name, out var values) || values.Count == 0)
        {
            return null;
        }

        var value = values[0];
        return string.IsNullOrEmpty(value) ? null : value;
    }
}
