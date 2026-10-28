// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「运费模板」类 DTO（merchant/addfreighttemplate、getfreighttemplatedetail、
// getfreighttemplatelist、updatefreighttemplate）。命名空间恒为 Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>条件包邮详情（官方 <c>all_condition_free_detail</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsAllConditionFreeDetail
{
    /// <summary>获取或设置计费规则列表（官方 <c>condition_free_detail_list</c>）。</summary>
    [JsonPropertyName("condition_free_detail_list")]
    public List<ChannelsLogisticsConditionFreeDetail>? ConditionFreeDetailList { get; set; }
}
