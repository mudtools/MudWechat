// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 售后管理（Aftersale）域「纠纷单」DTO（aftersale/getcomplaintorder、aftersale/addcomplaintmaterial、
// aftersale/addcomplaintproof、aftersale/syncworkorder）+ 纠纷单状态常量。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Aftersale。

namespace Mud.Wechat.Channels.DataModels.Aftersale;

/// <summary>工单信息（官方 <c>syncworkorder.work_order_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsWorkOrderInfo
{
    /// <summary>获取或设置递增版本号（官方 <c>version</c>，必填，每次全量同步递增）。</summary>
    [JsonPropertyName("version")]
    public long Version { get; set; }

    /// <summary>获取或设置工单列表（官方 <c>items</c>，必填，全量替换）。</summary>
    [JsonPropertyName("items")]
    public List<ChannelsWorkOrderItem> Items { get; set; } = new();
}
