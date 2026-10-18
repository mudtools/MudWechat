// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>
/// 地址条目（<c>address/get</c> 应答 <c>address_list[]</c> 元素）。
/// </summary>
/// <remarks>
/// <para><b><c>name</c> 已废弃</b>：官方表标注「联系人姓名（废弃）」，联系人以 <c>address_info.user_name</c> 为准。</para>
/// <para><b>敏感值</b>：姓名 / 电话 / 详细地址禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductOrder")]
public class WxaProductAddress
{
    /// <summary>地址 ID。</summary>
    [JsonPropertyName("address_id")]
    public long? AddressId { get; set; }

    /// <summary>联系人姓名（官方标废弃）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>座机。</summary>
    [JsonPropertyName("landline")]
    public string? Landline { get; set; }

    /// <summary>备注。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>是否为发货地址（1 是 / 0 否）。</summary>
    [JsonPropertyName("send_addr")]
    public int? SendAddr { get; set; }

    /// <summary>是否为默认发货地址（1 是 / 0 否）。</summary>
    [JsonPropertyName("default_send")]
    public int? DefaultSend { get; set; }

    /// <summary>是否为收货地址（1 是 / 0 否）。</summary>
    [JsonPropertyName("recv_addr")]
    public int? RecvAddr { get; set; }

    /// <summary>是否为默认收货地址（1 是 / 0 否）。</summary>
    [JsonPropertyName("default_recv")]
    public int? DefaultRecv { get; set; }

    /// <summary>创建时间（秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>更新时间（秒级时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>地址用途标记。</summary>
    [JsonPropertyName("address_type")]
    public WxaProductAddressType? AddressType { get; set; }

    /// <summary>地址本体。</summary>
    [JsonPropertyName("address_info")]
    public WxaProductAddressInfo? AddressInfo { get; set; }
}
