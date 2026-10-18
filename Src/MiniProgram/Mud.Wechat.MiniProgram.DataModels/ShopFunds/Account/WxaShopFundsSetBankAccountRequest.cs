// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopFunds;

/// <summary>修改结算账户请求（<c>POST /shop/funds/setbankaccount</c>）。</summary>
/// <remarks>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/funds/setbankaccount.html"/>。
/// <para><b>官方请求体只有 <c>account_info</c> 一支</b>（必填）⇒ 不得把子字段平铺到顶层。</para>
/// <para><b>覆盖式写入 + 每日 5 次上限</b>：本请求整体替换结算账户，账户须与开店主体一致（对公为公司名称、对私为经营者个人银行卡）。</para>
/// <para><b>敏感面（AGENTS §8）</b>：<see cref="WxaShopFundsBankAccountInfo.AccountNumber"/> 属个人金融信息，禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopFunds")]
public class WxaShopFundsSetBankAccountRequest
{
    /// <summary>账户信息（<c>account_info</c>，必填），见 <see cref="WxaShopFundsBankAccountInfo"/>。</summary>
    [JsonPropertyName("account_info")]
    public WxaShopFundsBankAccountInfo? AccountInfo { get; set; }
}
