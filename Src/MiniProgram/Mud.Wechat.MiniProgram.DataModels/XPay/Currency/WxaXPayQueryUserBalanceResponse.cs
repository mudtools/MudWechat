// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「查询代币余额」响应：有价/赠送两账户余额与四项累计量。
/// </summary>
/// <remarks>
/// <para><b><c>balance</c> 与 <c>present_balance</c> 是包含关系而非并列</b>（官方原文「代币总余额，包括有价和赠送部分」）⇒ 有价余额须自行相减，不得直接取 <c>balance</c> 做营收口径。</para>
/// <para><b><c>first_save_flag</c> 官方类型是 <c>boolean</c>、说明文字却写 0/1</b> ⇒ 线格式按类型列落 <c>bool?</c>。</para>
/// <para><b>敏感值</b>：全部余额与累计量属用户资产数据 ⇒ 不得进日志、遥测或异常消息。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_user_balance.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayQueryUserBalanceResponse : WxaResponse
{
    /// <summary>balance（官方类型 <c>number</c>）：代币总余额，包括有价和赠送部分</summary>
    [JsonPropertyName("balance")]
    public long? Balance { get; set; }

    /// <summary>present_balance（官方类型 <c>number</c>）：赠送账户的代币余额</summary>
    [JsonPropertyName("present_balance")]
    public long? PresentBalance { get; set; }

    /// <summary>sum_save（官方类型 <c>number</c>）：累计有价货币充值数量</summary>
    [JsonPropertyName("sum_save")]
    public long? SumSave { get; set; }

    /// <summary>sum_present（官方类型 <c>number</c>）：累计赠送无价货币数量</summary>
    [JsonPropertyName("sum_present")]
    public long? SumPresent { get; set; }

    /// <summary>sum_balance（官方类型 <c>number</c>）：历史总增加的代币金额</summary>
    [JsonPropertyName("sum_balance")]
    public long? SumBalance { get; set; }

    /// <summary>sum_cost（官方类型 <c>number</c>）：历史总消耗代币金额</summary>
    [JsonPropertyName("sum_cost")]
    public long? SumCost { get; set; }

    /// <summary>first_save_flag（官方类型 <c>boolean</c>）：是否满足首充活动标记。0:不满足。1:满足</summary>
    [JsonPropertyName("first_save_flag")]
    public bool? FirstSaveFlag { get; set; }
}
