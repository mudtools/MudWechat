// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.DataCube;

/// <summary>
/// 「结算收入数据」(<c>publisher_settlement</c>) 内单条结算区间按广告位的收入拆分（官方 <c>slot_revenue</c> 元素）。
/// </summary>
/// <remarks>注意：此处 <c>slot_id</c> 官方示例回传的是广告位<b>类型名称字符串</b>（如 <c>SLOT_ID_WEAPP_BANNER</c>），
/// 与「分广告位数据」<c>list.slot_id</c> 的 16 位整数形态不同——照各自页面示例，不做统一。</remarks>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpPublisherSlotRevenue
{
    /// <summary>产生收入的广告位（官方 <c>slot_id</c>，示例回传为广告位类型名称字符串）。</summary>
    [JsonPropertyName("slot_id")]
    public string? SlotId { get; set; }

    /// <summary>该广告位结算金额(分)（官方 <c>slot_settled_revenue</c>）。</summary>
    [JsonPropertyName("slot_settled_revenue")]
    public long SlotSettledRevenue { get; set; }
}
