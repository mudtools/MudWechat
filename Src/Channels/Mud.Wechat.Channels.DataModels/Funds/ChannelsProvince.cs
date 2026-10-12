// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Funds;

/// <summary>省份信息（官方 <c>data</c> 数组元素，<c>getprovince</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsProvince
{
    /// <summary>获取或设置省份简称（官方 <c>province_name</c>）。</summary>
    [JsonPropertyName("province_name")]
    public string? ProvinceName { get; set; }

    /// <summary>获取或设置省份编码（官方 <c>province_code</c>，可用于获取城市列表）。</summary>
    [JsonPropertyName("province_code")]
    public int? ProvinceCode { get; set; }
}
