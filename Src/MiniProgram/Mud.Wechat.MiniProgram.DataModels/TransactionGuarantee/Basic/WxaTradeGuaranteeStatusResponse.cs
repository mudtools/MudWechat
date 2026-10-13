// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>获取交易保障标状态应答（<c>GET /wxaapi/wxamptrade/get_guarantee_status</c>）。</summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/basic/api_getguaranteestatus.html"/>。
/// </para>
/// <para>
/// <b>成功响应无 <c>errmsg</c></b>：官方两处示例均只返回 <c>errcode</c> / <c>isActived</c> / <c>msg</c> /
/// <c>reasons</c> ⇒ <see cref="WxaResponse.ErrorMessage"/> 缺省即为 <c>null</c>，不得据其判成败
/// （判据是 <c>errcode == 0</c> ＋ <see cref="IsActived"/>）。
/// </para>
/// <para><b>未开通时 <c>reasons</c> 为原因文案数组</b>（官方示例 <c>["经营时长小于3年", "保证金余额不足"]</c>），已开通时为<b>空数组</b>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaTradeGuaranteeStatusResponse : WxaResponse
{
    /// <summary>是否激活交易保障标（<c>isActived</c>，官方类型 boolean）。</summary>
    [JsonPropertyName("isActived")]
    public bool? IsActived { get; set; }

    /// <summary>保障标状态相关信息（<c>msg</c>，如「已开通但未引入展示组件」）。</summary>
    [JsonPropertyName("msg")]
    public string? Msg { get; set; }

    /// <summary>未能开通保障标的原因列表（<c>reasons</c>，官方类型 array；已开通时为空数组）。</summary>
    [JsonPropertyName("reasons")]
    public List<string>? Reasons { get; set; }
}
