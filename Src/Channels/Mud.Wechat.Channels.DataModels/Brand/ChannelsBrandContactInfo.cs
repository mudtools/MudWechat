// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Brand;

/// <summary>联系方式（官方 <c>contact_info_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsBrandContactInfo
{
    /// <summary>获取或设置联系方式类型（官方 <c>key</c>，如联系电话）。</summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>获取或设置联系方式内容（官方 <c>value</c>）。</summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}
