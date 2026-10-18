// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductFunds;

/// <summary>商户提现请求（<c>POST /product/funds/submitwithdraw</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>submit_withdraw.html</c>。资金级不可逆动作，官方限制「每天最多允许提现一次」⇒
/// 绝对不得放进自动重试管道。</para>
/// <para><b>金额单位</b>：<c>amount</c> 官方明写「提现金额（单位：分）」，是本域唯一标注单位的端点；建为 <see cref="long"/>。</para>
/// <para><b>敏感面</b>：提现金额与银行附言（<c>bank_memo</c>）属敏感结算信息，禁入日志、遥测或异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductFunds")]
public class WxaProductFundsSubmitWithdrawRequest
{
    /// <summary>提现金额（<c>amount</c>，number，<b>必填</b>，<b>单位：分</b>）。敏感结算信息。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>提现备注（<c>remark</c>，string，选填）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>银行附言（<c>bank_memo</c>，string，选填）。敏感结算信息。</summary>
    [JsonPropertyName("bank_memo")]
    public string? BankMemo { get; set; }
}
