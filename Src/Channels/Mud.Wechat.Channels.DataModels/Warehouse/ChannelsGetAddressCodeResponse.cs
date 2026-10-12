// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Warehouse;

/// <summary>获取地址行政编码（<c>addresscode/get</c>）响应。</summary>
/// <remarks>
/// 官方契约：<c>addrs_msg</c> 与 <c>next_level_addrs</c> 位于响应<b>顶层</b>（无 <c>data</c> 包裹，照抄官方结构）；
/// 最多获取 4 级地址的行政编码。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsGetAddressCodeResponse : ChannelsResponse
{
    /// <summary>获取或设置本行政编码地址信息（官方 <c>addrs_msg</c>）。</summary>
    [JsonPropertyName("addrs_msg")]
    public ChannelsAddress? AddrsMsg { get; set; }

    /// <summary>获取或设置下一级所有地址信息（官方 <c>next_level_addrs</c>）。</summary>
    [JsonPropertyName("next_level_addrs")]
    public List<ChannelsAddress>? NextLevelAddrs { get; set; }
}
