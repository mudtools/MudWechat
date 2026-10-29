// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>观众画像（官方 <c>portrait</c> 对象，<c>LiveDashboardData2</c>）。</summary>
/// <remarks>
/// 官方原文：各有八种单维度组合（2、3、5、9、10、11、12、13）——JSON 键为维度组合字符串（动态键），
/// 值结构官方未定义，以 <see cref="System.Text.Json.JsonElement"/> 保真透传；
/// <c>online_watch_uv</c>（在线人数画像）仅直播中有效，时间戳字段无意义。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveDashboardPortrait
{
    /// <summary>获取或设置在线人数画像（官方 <c>online_watch_uv</c>，键为维度组合字符串）。</summary>
    [JsonPropertyName("online_watch_uv")]
    public Dictionary<string, System.Text.Json.JsonElement>? OnlineWatchUv { get; set; }

    /// <summary>获取或设置观看人数画像（官方 <c>new_watch_uv</c>，键为维度组合字符串）。</summary>
    [JsonPropertyName("new_watch_uv")]
    public Dictionary<string, System.Text.Json.JsonElement>? NewWatchUv { get; set; }
}
