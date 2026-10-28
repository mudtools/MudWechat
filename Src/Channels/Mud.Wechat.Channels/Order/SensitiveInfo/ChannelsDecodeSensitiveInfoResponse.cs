// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 订单管理（Order）域「虚拟号 / 真实号 / 敏感信息解密 / 商家私密号」DTO
// （order/virtualnumber/applyagain、order/virtualnumber/delay、order/realnumber/apply、
//  order/realnumberviewaudit/get、order/sensitiveinfo/decode、order/merchant/privatenumber/*）。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Order。

namespace Mud.Wechat.Channels.DataModels.Order;

/// <summary>解密订单中的详细收货信息（<c>order/sensitiveinfo/decode</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsDecodeSensitiveInfoResponse : ChannelsResponse
{
    /// <summary>获取或设置收货信息（官方 <c>address_info</c>，字段集与 <c>ChannelsOrderAddress</c> 一致）。</summary>
    [JsonPropertyName("address_info")]
    public ChannelsOrderAddress? AddressInfo { get; set; }

    /// <summary>获取或设置虚拟号信息（官方 <c>virtual_number_info</c>）。</summary>
    [JsonPropertyName("virtual_number_info")]
    public ChannelsOrderVirtualNumberInfo? VirtualNumberInfo { get; set; }
}
