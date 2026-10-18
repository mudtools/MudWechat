// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductFunds;

/// <summary>账户余额应答（<c>POST /product/funds/getbalance</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>minishopopencomponent/API/funds/get_balance.html</c>。</para>
/// <para><b>敏感面</b>：<c>sub_mchid</c>（二级商户号）与两笔余额金额属结算标识 / 个人金融信息，
/// 不得写入日志、遥测或异常消息。金额单位官方本页未标注（整型口径，禁浮点）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductFunds")]
public class WxaProductFundsBalanceResponse : WxaResponse
{
    /// <summary>可提现余额（<c>available_amount</c>）。金额单位官方本页未标注，整型（分）口径。</summary>
    [JsonPropertyName("available_amount")]
    public long? AvailableAmount { get; set; }

    /// <summary>待结算余额（<c>pending_amount</c>）。金额单位官方本页未标注，整型（分）口径。</summary>
    [JsonPropertyName("pending_amount")]
    public long? PendingAmount { get; set; }

    /// <summary>二级商户号（<c>sub_mchid</c>）。敏感结算标识，禁入日志。</summary>
    [JsonPropertyName("sub_mchid")]
    public long? SubMerchantId { get; set; }
}
