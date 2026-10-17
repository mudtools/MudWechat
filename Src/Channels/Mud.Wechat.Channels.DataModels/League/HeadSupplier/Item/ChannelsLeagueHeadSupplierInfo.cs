// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>供货机构信息（官方 <c>item.head_supplier_info</c> 对象）。</summary>
/// <remarks>官方变更日志 2025-11-03：调整 <c>head_supplier_info</c> 回参，新增 <c>head_img</c>（机构头像）字段。</remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierInfo
{
    /// <summary>获取或设置供货机构昵称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置机构头像（官方 <c>head_img</c>）。</summary>
    [JsonPropertyName("head_img")]
    public string? HeadImg { get; set; }

    /// <summary>获取或设置机构 appid（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }
}
