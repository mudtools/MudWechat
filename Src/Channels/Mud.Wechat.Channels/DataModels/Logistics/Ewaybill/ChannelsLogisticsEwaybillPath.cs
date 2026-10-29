// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「电子面单」类 DTO（logistics/ewaybill/biz/*）。命名空间恒为
// Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>电子面单轨迹信息（官方 <c>order_info.path_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillPath
{
    /// <summary>获取或设置轨迹状态（官方 <c>status</c>）：0 待揽件；1 揽收；2 运输中；3 派件；4 入柜；5 签收；6 退回；7 转寄；8 异常；9 出柜；99 其他未知。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置轨迹描述（官方 <c>desc</c>）。</summary>
    [JsonPropertyName("desc")]
    public string? Desc { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }
}
