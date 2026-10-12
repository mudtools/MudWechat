// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>获取直播大屏直播列表（<c>getlivelist</c>）请求体。</summary>
/// <remarks>官方 <c>ds</c> 类型为 number（日期数值如 20240612），目前不得早于 20240101。</remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsGetLiveDashboardListRequest
{
    /// <summary>获取或设置日期（官方 <c>ds</c>，必填，日期数值如 20240612；不得早于 20240101）。</summary>
    [JsonPropertyName("ds")]
    public long Ds { get; set; }
}
