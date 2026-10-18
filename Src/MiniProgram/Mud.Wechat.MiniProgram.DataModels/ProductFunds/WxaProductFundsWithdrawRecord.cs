// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductFunds;

/// <summary>提现记录列表条目（scanwithdraw 的 <c>list</c> 元素）。</summary>
/// <remarks>
/// <para>官方文档：<c>scan_withdraw.html</c>。时间键为 <c>wxpay_create_time</c> / <c>wxpay_update_time</c>
/// （单笔提现记录 getwithdrawdetail 用 <c>create_time</c> / <c>update_time</c>）⇒ 两套形态不同，守卫 PF5 锁定，不得互抄。</para>
/// <para><b>敏感面</b>：<c>amount</c>（提现金额）/ <c>bank_memo</c>（银行附言）属敏感结算信息，
/// 不得写入日志、遥测或异常消息。金额单位官方本页未标注，整型（分）口径。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductFunds")]
public class WxaProductFundsWithdrawRecord
{
    /// <summary>提现单号（<c>out_request_no</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>提现金额（<c>amount</c>）。单位官方本页未标注，整型（分）口径。敏感结算信息。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>提现状态（<c>status</c>，string）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>提现失败原因（<c>reason</c>）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>备注（<c>remark</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>银行附言（<c>bank_memo</c>）。敏感结算信息。</summary>
    [JsonPropertyName("bank_memo")]
    public string? BankMemo { get; set; }

    /// <summary>更新时间（<c>wxpay_update_time</c>，unix 秒时间戳）。</summary>
    [JsonPropertyName("wxpay_update_time")]
    public long? WxpayUpdateTime { get; set; }

    /// <summary>开始时间（<c>wxpay_create_time</c>，unix 秒时间戳）。</summary>
    [JsonPropertyName("wxpay_create_time")]
    public long? WxpayCreateTime { get; set; }
}
