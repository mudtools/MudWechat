// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Pay;

/// <summary>
/// 获取资金流水响应体（<c>/cgi-bin/externalpay/get_fund_flow</c>）。
/// <para>官方业务限制：仅返回在企业微信开通的商户号资金流水；
/// 无 next_cursor 返回时表示已拉取完全部数据。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class GetPayFundFlowResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置分页游标；无更多数据时不返回该字段。
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    /// <summary>
    /// 获取或设置资金流水记录列表。
    /// </summary>
    [JsonPropertyName("fund_flow_list")]
    public List<PayFundFlowItem>? FundFlowList { get; set; }
}

/// <summary>
/// 资金流水记录（<see cref="GetPayFundFlowResponse.FundFlowList"/> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayFundFlowItem
{
    /// <summary>
    /// 获取或设置动账时间（Unix 时间戳，秒）。
    /// </summary>
    [JsonPropertyName("timestamp")]
    public long? Timestamp { get; set; }

    /// <summary>
    /// 获取或设置关联单号（微信支付资金流水单号）。
    /// </summary>
    [JsonPropertyName("request_no")]
    public string? RequestNo { get; set; }

    /// <summary>
    /// 获取或设置动账类型：1 - 退款、2 - 交易手续费、3 - 收款、4 - 提现、5 - 其他。
    /// </summary>
    [JsonPropertyName("transaction_type")]
    public int? TransactionType { get; set; }

    /// <summary>
    /// 获取或设置收支类型：1 - 收入、2 - 支出。
    /// </summary>
    [JsonPropertyName("fund_flow_type")]
    public int? FundFlowType { get; set; }

    /// <summary>
    /// 获取或设置动账金额（单位分）。
    /// </summary>
    [JsonPropertyName("transaction_amount")]
    public long? TransactionAmount { get; set; }

    /// <summary>
    /// 获取或设置账户余额（单位分）。
    /// </summary>
    [JsonPropertyName("account_balance")]
    public long? AccountBalance { get; set; }

    /// <summary>
    /// 获取或设置商户单号（业务凭证号）。
    /// </summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>
    /// 获取或设置商户号 ID。
    /// </summary>
    [JsonPropertyName("mch_id")]
    public string? MchId { get; set; }

    /// <summary>
    /// 获取或设置操作人 userid。
    /// </summary>
    [JsonPropertyName("operator_userid")]
    public string? OperatorUserId { get; set; }

    /// <summary>
    /// 获取或设置所属规则组列表。
    /// </summary>
    [JsonPropertyName("group_list")]
    public List<PayFundFlowGroup>? GroupList { get; set; }

    /// <summary>
    /// 获取或设置备注。
    /// </summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }
}

/// <summary>
/// 资金流水所属规则组（<see cref="PayFundFlowItem.GroupList"/> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayFundFlowGroup
{
    /// <summary>
    /// 获取或设置规则组名称。
    /// </summary>
    [JsonPropertyName("group_name")]
    public string? GroupName { get; set; }
}
