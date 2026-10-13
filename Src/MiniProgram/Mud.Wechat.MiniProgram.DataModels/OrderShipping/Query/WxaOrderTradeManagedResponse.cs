// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「查询小程序是否已开通发货信息管理服务」响应（<c>POST /wxa/sec/order/is_trade_managed</c>）。
/// </summary>
/// <remarks>
/// <para><b>本字段是本域的接入前置开关</b>：为 <c>false</c> 时录入类端点（<c>upload_shipping_info</c> 等）
/// 无法使用，须先在小程序后台开通发货信息管理服务。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_istrademanaged.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaOrderTradeManagedResponse : WxaResponse
{
    /// <summary>is_trade_managed（官方类型 <c>boolean</c>）：是否已开通小程序发货信息管理服务</summary>
    [JsonPropertyName("is_trade_managed")]
    public bool? IsTradeManaged { get; set; }
}
