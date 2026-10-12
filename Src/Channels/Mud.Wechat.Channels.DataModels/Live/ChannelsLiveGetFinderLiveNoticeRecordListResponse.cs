// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>
/// 视频号获取当前的直播预约记录（<c>getfinderlivenoticerecordlist</c>）响应。
/// </summary>
/// <remarks>
/// 官方契约：<b>POST</b> + 空请求体；权限集 id：133、160、177（第三方平台代调用）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveGetFinderLiveNoticeRecordListResponse : ChannelsResponse
{
    /// <summary>获取或设置直播预约记录列表（官方 <c>live_notice_list</c>；官方参数表类型列为 object、示例为数组——按数组容器建模，矛盾照录）。</summary>
    [JsonPropertyName("live_notice_list")]
    public List<ChannelsLiveNoticeRecord>? LiveNoticeList { get; set; }
}
