// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「地址」类 DTO（merchant/address/add、get、list、delete、update）。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>地址信息（官方 <c>address_detail</c>，添加 / 更新共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsAddressDetail
{
    /// <summary>获取或设置地址 id（官方 <c>address_id</c>）。</summary>
    [JsonPropertyName("address_id")]
    public string AddressId { get; set; } = string.Empty;

    /// <summary>获取或设置联系人姓名（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置地区信息（官方 <c>address_info</c>）。</summary>
    [JsonPropertyName("address_info")]
    public ChannelsLogisticsAddressInfo? AddressInfo { get; set; }

    /// <summary>获取或设置座机（官方 <c>landline</c>）。</summary>
    [JsonPropertyName("landline")]
    public string? Landline { get; set; }

    /// <summary>获取或设置是否为发货地址（官方 <c>send_addr</c>）。</summary>
    [JsonPropertyName("send_addr")]
    public bool? SendAddr { get; set; }

    /// <summary>获取或设置是否为默认发货地址（官方 <c>default_send</c>）。</summary>
    [JsonPropertyName("default_send")]
    public bool? DefaultSend { get; set; }

    /// <summary>获取或设置是否为收货地址（官方 <c>recv_addr</c>）。</summary>
    [JsonPropertyName("recv_addr")]
    public bool? RecvAddr { get; set; }

    /// <summary>获取或设置是否为默认收货地址（官方 <c>default_recv</c>）。</summary>
    [JsonPropertyName("default_recv")]
    public bool? DefaultRecv { get; set; }

    /// <summary>获取或设置创建时间戳（秒，官方 <c>create_time</c>）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置更新时间戳（秒，官方 <c>update_time</c>）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置线下配送地址类型（官方 <c>address_type</c>）。</summary>
    [JsonPropertyName("address_type")]
    public ChannelsLogisticsAddressType? AddressType { get; set; }
}
