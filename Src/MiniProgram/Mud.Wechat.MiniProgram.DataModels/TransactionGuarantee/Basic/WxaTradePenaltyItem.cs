// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>
/// 交易体验分扣分记录（<c>get_penalty_list</c> 的 <c>appealList[]</c> 元素）。
/// </summary>
/// <remarks>
/// <para>
/// <b>类型口径（本仓既定裁决）</b>：官方参数表把 <see cref="IllegalOrderId"/> /
/// <see cref="ComplaintOrderId"/> / <see cref="OrderId"/> 标为 <c>number</c>，而官方<b>返回示例</b>
/// 却写成字符串形态（<c>"illegalOrderId": "12345"</c>）⇒ 本类型<b>以参数表为准</b>取
/// <see cref="long"/>，不在本线做数字/字符串兼容转换器（与公众号 / 小店线同口径，
/// 参见 <c>Mud.Wechat.Work.DataModels.Meeting.GetRecordFileResponse</c> 的同类处理）。
/// 若线上确为字符串形态，属官方报文与自身文档不一致，处置方式是<b>改型</b>而非加本地兼容层。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaTradePenaltyItem
{
    /// <summary>扣分记录 id（<c>illegalOrderId</c>，申诉 <c>/wxaapi/minishop/busiAppeal</c> 侧的定位键来源之一）。</summary>
    [JsonPropertyName("illegalOrderId")]
    public long? IllegalOrderId { get; set; }

    /// <summary>关联投诉单 id（<c>complaintOrderId</c>，可用于 <see cref="WxaComplaintDetailResponse"/> 详情查询）。</summary>
    [JsonPropertyName("complaintOrderId")]
    public long? ComplaintOrderId { get; set; }

    /// <summary>违规行为描述（<c>illegalWording</c>，如「质量缺陷」）。</summary>
    [JsonPropertyName("illegalWording")]
    public string? IllegalWording { get; set; }

    /// <summary>
    /// 扣分记录状态（<c>status</c>；官方枚举：<c>2</c> 扣分审批通过 / <c>4</c> 申诉中 /
    /// <c>5</c> 申述驳回（官方原文拼写）/ <c>6</c> 申诉成功）。
    /// </summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>扣除分数（<c>minusScore</c>）。</summary>
    [JsonPropertyName("minusScore")]
    public long? MinusScore { get; set; }

    /// <summary>订单号（<c>orderId</c>）。</summary>
    [JsonPropertyName("orderId")]
    public long? OrderId { get; set; }

    /// <summary>扣分记录创建时间（<c>illegalTime</c>，秒级时间戳）。</summary>
    [JsonPropertyName("illegalTime")]
    public long? IllegalTime { get; set; }

    /// <summary>更新时间（<c>updateTime</c>，秒级时间戳）。</summary>
    [JsonPropertyName("updateTime")]
    public long? UpdateTime { get; set; }
}
