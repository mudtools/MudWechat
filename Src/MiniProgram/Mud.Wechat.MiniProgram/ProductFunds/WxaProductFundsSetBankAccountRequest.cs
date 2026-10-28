// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductFunds;

/// <summary>修改结算账户请求（<c>POST /product/funds/setbankaccount</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>set_bank_account.html</c>。请求体为 <c>account_info</c> 单对象包装；覆盖式写入，
/// 官方限制「每天允许修改结算账户的次数为 5 次」⇒ 非幂等，不得放进自动重试管道。</para>
/// <para><b>敏感面</b>：内嵌 <see cref="WxaProductFundsBankAccountInfo"/> 含银行账号等个人金融信息，请求体整体禁入日志、遥测或异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductFunds")]
public class WxaProductFundsSetBankAccountRequest
{
    /// <summary>结算账户信息（<c>account_info</c>，单对象包装），见 <see cref="WxaProductFundsBankAccountInfo"/>。</summary>
    [JsonPropertyName("account_info")]
    public WxaProductFundsBankAccountInfo? AccountInfo { get; set; }
}
