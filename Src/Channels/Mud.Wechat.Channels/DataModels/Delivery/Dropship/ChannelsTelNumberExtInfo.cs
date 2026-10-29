// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>虚拟号码联系方式信息（官方 <c>tel_number_ext_info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsTelNumberExtInfo
{
    /// <summary>获取或设置脱敏手机号（官方 <c>real_tel_number</c>）。</summary>
    [JsonPropertyName("real_tel_number")]
    public string? RealTelNumber { get; set; }

    /// <summary>获取或设置完整的虚拟号码（官方 <c>virtual_tel_number</c>）。</summary>
    [JsonPropertyName("virtual_tel_number")]
    public string? VirtualTelNumber { get; set; }

    /// <summary>获取或设置主动兑换虚拟号码次数（官方 <c>get_virtual_tel_cnt</c>）。</summary>
    [JsonPropertyName("get_virtual_tel_cnt")]
    public int? GetVirtualTelCnt { get; set; }

    /// <summary>获取或设置主动兑换的虚拟号码过期时间（官方 <c>virtual_tel_expire_time</c>）。</summary>
    [JsonPropertyName("virtual_tel_expire_time")]
    public long? VirtualTelExpireTime { get; set; }
}
