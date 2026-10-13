// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>获取小程序交易体验分违规记录应答（<c>GET /wxaapi/wxamptrade/get_penalty_list</c>）。</summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/basic/api_getpenaltylist.html"/>。
/// </para>
/// <para>
/// <b>官方字段名照录 <c>appealList</c></b>：该列表承载的是<b>扣分（违规）记录</b>，官方却以「申诉列表」命名，
/// 元素类型名为 <see cref="WxaTradePenaltyItem"/>（本仓语义命名）而 JSON 名仍为 <c>appealList</c>——
/// 命名与语义的错位属官方原文，不得「顺手改成 <c>penaltyList</c>」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaTradePenaltyListResponse : WxaResponse
{
    /// <summary>扣分记录列表（<c>appealList</c>，官方类型 objarray），见 <see cref="WxaTradePenaltyItem"/>。</summary>
    [JsonPropertyName("appealList")]
    public List<WxaTradePenaltyItem>? AppealList { get; set; }

    /// <summary>当前小程序的交易体验分（<c>currentScore</c>）。</summary>
    [JsonPropertyName("currentScore")]
    public long? CurrentScore { get; set; }

    /// <summary>当前小程序的总扣分记录数（<c>totalNum</c>，分页终止判据）。</summary>
    [JsonPropertyName("totalNum")]
    public long? TotalNum { get; set; }
}
