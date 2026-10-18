// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductFunds;

/// <summary>单笔提现记录应答（<c>POST /product/funds/getwithdrawdetail</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>get_withdraw_detail.html</c>。应答为<b>平铺单对象</b>（无 <c>list</c> 包装），时间键为
/// <c>create_time</c> / <c>update_time</c>（提现列表侧 scanwithdraw 用 <c>wxpay_create_time</c> / <c>wxpay_update_time</c>）⇒ 两套形态不同，不得互抄。</para>
/// <para><b>敏感面</b>：<c>bank_num</c>（银行账户）/ <c>bank_name</c>（银行名称）/ <c>amount</c>（金额）属个人金融信息，
/// 不得写入日志、遥测或异常消息。金额单位官方本页未标注，整型（分）口径。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductFunds")]
public class WxaProductFundsWithdrawDetailResponse : WxaResponse
{
    /// <summary>提现金额（<c>amount</c>）。单位官方本页未标注，整型（分）口径。敏感结算信息。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>创建时间（<c>create_time</c>，unix 秒时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>更新时间（<c>update_time</c>，unix 秒时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>失败原因（<c>reason</c>）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>备注（<c>remark</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>银行附言（<c>bank_memo</c>）。敏感结算信息。</summary>
    [JsonPropertyName("bank_memo")]
    public string? BankMemo { get; set; }

    /// <summary>银行名称（<c>bank_name</c>）。敏感结算信息。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>银行账户（<c>bank_num</c>）。<b>敏感个人金融信息</b>，禁入日志、遥测或异常消息。</summary>
    [JsonPropertyName("bank_num")]
    public string? BankNum { get; set; }

    /// <summary>提现状态（<c>status</c>，string）：CREATE_SUCCESS / SUCCESS / FAIL / REFUND / CLOSE / INIT。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }
}
