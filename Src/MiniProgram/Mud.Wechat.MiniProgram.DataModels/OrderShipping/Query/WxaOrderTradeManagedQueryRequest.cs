// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「按 appid 查询交易保障开通状态」请求体（<c>is_trade_managed</c> 与
/// <c>is_trade_management_confirmation_completed</c> 两端点<b>共用</b>，字段面逐字相同）。
/// </summary>
/// <remarks>
/// <para><b><c>appid</c> 是必填而非「默认本账号」</b>（官方原文「待查询小程序的 appid，非服务商调用时仅能查询本账号」）：
/// 自建/普通小程序调用时须显式传自己的 appid，漏传即 <c>40013</c> / <c>40097</c> 族错误。</para>
/// <para><b>服务商侧前置</b>：官方注意事项「服务商被授权了 18 或 142 权限集时才能进行查询」。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_istrademanaged.html"/> ; <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_istrademanagementconfirmationcompleted.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaOrderTradeManagedQueryRequest
{
    /// <summary>appid（官方类型 <c>string</c>，必填）：待查询小程序的 appid，非服务商调用时仅能查询本账号</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }
}
