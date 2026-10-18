// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopFunds;

/// <summary>发起提现请求（<c>POST /shop/funds/submitwithdraw</c>）。</summary>
/// <remarks>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/wxafunds/API/funds/submitwithdraw.html"/>。
/// <para><b><c>amount</c> 单位「分」是官方参数表原文</b>（本域唯一标注单位的金额字段）⇒ 本仓不做任何换算。</para>
/// <para><b>非幂等</b>：官方返回码 <c>9700101</c>「超出每日只能提现一次的限制」⇒ 同一自然日只可成功一次，禁止自动重试。</para>
/// <para><b>敏感面（AGENTS §8）</b>：提现金额、提现备注与银行附言不得进日志、遥测或异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopFunds")]
public class WxaShopFundsSubmitWithdrawRequest
{
    /// <summary>提现金额（<c>amount</c>，必填，<b>单位：分</b>，官方参数表原文）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>提现备注（<c>remark</c>，选填）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>银行附言（<c>bank_memo</c>，选填）。</summary>
    [JsonPropertyName("bank_memo")]
    public string? BankMemo { get; set; }

    /// <summary>业务类型（<c>biz_type</c>，必填，官方参数表原文「业务类型，填 1」⇒ 官方只公布 1）。</summary>
    [JsonPropertyName("biz_type")]
    public int? BizType { get; set; }

    /// <summary>商户号（<c>mchid</c>，必填，官方 JSON 名无下划线、全小写）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }
}
