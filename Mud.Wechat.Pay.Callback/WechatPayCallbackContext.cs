// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;

namespace Mud.Wechat.Pay.Callback;

/// <summary>
/// 支付通知上下文：<b>已通过三道闸（验签 / 时效 / 一次性指纹）且 <c>resource</c> 已解密</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>本上下文是「可信边界」的产物</b>：拿到它即表示报文来源可信、未被篡改、不是重放。
/// 处理器只需关注业务（<c>out_trade_no</c> 幂等落库）。
/// </para>
/// <para>
/// <b>红线（PAY-B7）</b>：<see cref="ResourceJson"/> 是<b>解密后的明文</b>（含 <c>openid</c> 等），
/// <b>不得</b>写入日志 / 遥测 / 异常消息。
/// </para>
/// </remarks>
public sealed class WechatPayCallbackContext
{
    private readonly string _resourceJson;

    internal WechatPayCallbackContext(
        string merchantKey,
        WechatPayMerchantConfig merchant,
        WechatPayNotification notification,
        string resourceJson,
        DateTimeOffset receivedAt)
    {
        MerchantKey = merchantKey;
        Merchant = merchant;
        Notification = notification;
        _resourceJson = resourceJson;
        ReceivedAt = receivedAt;
    }

    /// <summary>命中的商户键（= 路由段 = <c>IWechatPayMerchantManager</c> 的键）。</summary>
    public string MerchantKey { get; }

    /// <summary>命中的商户配置（提供 <c>MchId</c> / <c>AuthorizationMchId</c> 等）。</summary>
    public WechatPayMerchantConfig Merchant { get; }

    /// <summary>通知信封（<c>event_type</c> / <c>resource_type</c> / <c>summary</c> / <c>id</c>）。</summary>
    public WechatPayNotification Notification { get; }

    /// <summary>通知事件类型（<c>event_type</c> 快捷方式）。</summary>
    public string? EventType => Notification.EventType;

    /// <summary>通知 ID（<c>id</c> 快捷方式；官方唯一标识，可作为幂等键的补充）。</summary>
    public string? NotificationId => Notification.Id;

    /// <summary>接收时间（本地时钟，用于诊断；<b>不是</b>官方时间）。</summary>
    public DateTimeOffset ReceivedAt { get; }

    /// <summary>
    /// 解密后的 <c>resource</c> JSON 明文。
    /// </summary>
    /// <remarks>
    /// <b>不得入日志</b>（PAY-B7）。首选 <see cref="GetTransaction"/> / <see cref="GetRefund"/> 取类型化载荷；
    /// 仅当通知载荷为尚未建模的类型时才直接读本属性。
    /// </remarks>
    public string ResourceJson => _resourceJson;

    /// <summary>把解密后的载荷按<b>交易</b>（<c>TRANSACTION.*</c>）类型化解析。</summary>
    /// <returns>解析失败或载荷非交易形态时返回 <c>null</c>。</returns>
    /// <remarks>走源生成 <c>JsonTypeInfo</c> 快车道（零反射、AOT 安全），不使用反射版 <c>Deserialize&lt;T&gt;</c>。</remarks>
    public WechatPayTransactionResource? GetTransaction()
        => TryDeserialize(WechatPayCallbackJsonContext.Default.WechatPayTransactionResource);

    /// <summary>把解密后的载荷按<b>退款</b>（<c>REFUND.*</c>）类型化解析。</summary>
    /// <returns>解析失败或载荷非退款形态时返回 <c>null</c>。</returns>
    public WechatPayRefundResource? GetRefund()
        => TryDeserialize(WechatPayCallbackJsonContext.Default.WechatPayRefundResource);

    /// <summary>
    /// 把解密后的载荷按<b>分账</b>（<c>original_type = profitsharing</c>）类型化解析。
    /// </summary>
    /// <returns>解析失败或载荷非分账形态时返回 <c>null</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>⚠️ 调用前必须自行判 <c>resource.original_type</c></b>：分账动态通知的 <c>event_type</c> 与
    /// 支付成功通知<b>同为</b> <c>TRANSACTION.SUCCESS</c>（官方原文），只看 <c>event_type</c> 会把交易载荷
    /// 按分账解析 —— 字段大面积为空但<b>不报错</b>，是最危险的静默错位。判别式见
    /// <see cref="WechatPayNotificationOriginalTypes.ProfitSharing"/>。
    /// </para>
    /// <para>
    /// 本上下文<b>刻意不做</b>「按 <c>event_type</c> 自动选载荷」的便利方法：官方事件类型存在同名复用，
    /// 任何自动选择都会在复用场景下静默选错；把判别显式留给调用方是唯一诚实的形态。
    /// </para>
    /// </remarks>
    public WechatPayProfitSharingResource? GetProfitSharing()
        => TryDeserialize(WechatPayCallbackJsonContext.Default.WechatPayProfitSharingResource);

    private T? TryDeserialize<T>(System.Text.Json.Serialization.Metadata.JsonTypeInfo<T> typeInfo)
        where T : class
    {
        try
        {
            return JsonSerializer.Deserialize(_resourceJson, typeInfo);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
