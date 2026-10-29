// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「地址」类 DTO（merchant/address/add、get、list、delete、update）。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>线下配送地址类型（官方 <c>address_detail.address_type</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsAddressType
{
    /// <summary>获取或设置是否同城配送（官方 <c>same_city</c>，1 表示同城配送）。</summary>
    [JsonPropertyName("same_city")]
    public int? SameCity { get; set; }

    /// <summary>获取或设置是否用户自提（官方 <c>pickup</c>，1 表示用户自提）。</summary>
    [JsonPropertyName("pickup")]
    public int? Pickup { get; set; }
}
