// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Schedule;

/// <summary>
/// 创建日程请求体（<c>/cgi-bin/oa/schedule/add</c>）。
/// </summary>
/// <remarks>
/// <para>官方限制：管理员须在共享成员内且最多 3 人；参与者最多 1000 人；
/// 自建应用 cal_id 不填时写入应用默认日历，第三方应用必须指定 cal_id。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Schedule")]
public class AddScheduleRequest
{
    /// <summary>获取或设置日程信息（官方必填）。</summary>
    [JsonPropertyName("schedule")]
    public ScheduleInfo? Schedule { get; set; }

    /// <summary>获取或设置授权方安装的应用 agentid（仅旧的第三方多应用套件需填此参数）。</summary>
    [JsonPropertyName("agentid")]
    public int? AgentId { get; set; }
}
