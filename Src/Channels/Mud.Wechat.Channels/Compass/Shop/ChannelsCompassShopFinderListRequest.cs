// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Compass;

/// <summary>获取带货达人列表（<c>getshopfinderlist</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopFinderListRequest
{
    /// <summary>获取或设置日期（官方 <c>ds</c>，必填，格式 YYYYMMDD）。</summary>
    [JsonPropertyName("ds")]
    public string Ds { get; set; } = string.Empty;

    /// <summary>获取或设置版本号（官方 <c>version</c>，选填；填 <c>v2</c> 会返回带货者身份的 id）。</summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }
}
