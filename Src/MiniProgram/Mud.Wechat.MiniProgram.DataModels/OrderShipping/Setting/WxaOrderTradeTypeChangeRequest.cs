// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「小程序交易类型变更申请」请求体（<c>POST /wxa/sec/order/setwxatradetypecgi</c>）。
/// </summary>
/// <remarks>
/// <para><b>一个账号只能申请一次</b>（官方注意事项原文）：重复提交返回 <c>268486048</c>，
/// 需多次申请须联系小程序客服 ⇒ 本端点<b>不是</b>可重复调用的设置接口。</para>
/// <para><b>材料规模限制</b>（与频率无关）：<c>material_list</c> 最多 10 个，其中视频最多 3 个。</para>
/// <para><b>路由名是 <c>setwxatradetypecgi</c> 全小写连写</b>，且带历史后缀 <c>cgi</c> ⇒ 照官方原文，
/// 不得写成 <c>set_wxa_trade_type</c>。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_setwxatradetypecgi.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaOrderTradeTypeChangeRequest
{
    /// <summary>trade_type（官方类型 <c>number</c>，必填）：申请变更后的目标交易类型。取值见 <see cref="WxaOrderTradeTypeValues"/></summary>
    [JsonPropertyName("trade_type")]
    public long? TradeType { get; set; }

    /// <summary>material_list（官方类型 <c>objarray</c>，必填）：申请材料列表，最多10个，其中视频最多3个</summary>
    [JsonPropertyName("material_list")]
    public List<WxaOrderTradeTypeMaterial>? MaterialList { get; set; }

    /// <summary>reason（官方类型 <c>string</c>，必填）：申请理由</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}
