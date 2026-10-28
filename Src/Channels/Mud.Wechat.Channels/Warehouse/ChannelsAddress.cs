// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Warehouse;

/// <summary>地址行政编码信息（官方 <c>addrs_msg</c> 对象与 <c>next_level_addrs</c> 数组元素共用，<c>addresscode/get</c>）。</summary>
/// <remarks>
/// <b>共用 DTO 裁决</b>：<c>addrs_msg</c> 与 <c>next_level_addrs</c> 元素字段集完全一致
/// （<c>name</c>/<c>code</c>/<c>level</c>），共用同一元素 DTO。
/// 官方返回示例中根节点「中华人民共和国」<c>level</c>=<c>0</c>、<c>code</c>=<c>0</c>（照录，
/// 参数表仅声明 1~4 级，见 <see cref="ChannelsAddressLevels"/>）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsAddress
{
    /// <summary>获取或设置地址名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置地址行政编码（官方 <c>code</c>）。</summary>
    [JsonPropertyName("code")]
    public long? Code { get; set; }

    /// <summary>获取或设置地址级别（官方 <c>level</c>，见 <see cref="ChannelsAddressLevels"/>）。</summary>
    [JsonPropertyName("level")]
    public int? Level { get; set; }
}
