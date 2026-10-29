// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School.ClassPay;

/// <summary>
/// 学生付款详情（<c>get_payment_result</c> 响应 <c>payment_result</c> 元素结构，官方 PayInfo）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ClassPay")]
public class ClassPayPaymentResultItem
{
    /// <summary>
    /// 获取或设置学生账号。
    /// </summary>
    [JsonPropertyName("student_userid")]
    public string? StudentUserId { get; set; }

    /// <summary>
    /// 获取或设置付款状态（0：未付款；1：已付款）。
    /// </summary>
    [JsonPropertyName("trade_state")]
    public int? TradeState { get; set; }

    /// <summary>
    /// 获取或设置订单号（可用于调用获取订单详情接口）。
    /// </summary>
    [JsonPropertyName("trade_no")]
    public string? TradeNo { get; set; }

    /// <summary>
    /// 获取或设置付款家长账号。
    /// </summary>
    [JsonPropertyName("payer_parent_userid")]
    public string? PayerParentUserId { get; set; }
}
