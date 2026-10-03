// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Moment;

/// <summary>
/// 创建发表朋友圈任务的请求体（<c>/cgi-bin/externalcontact/add_moment_task</c>）。
/// <para>
/// <see cref="Text"/> 与 <see cref="Attachments"/> 不能同时为空；
/// 企业每个月允许通过 API 创建的朋友圈次数上限为 10 万次，且每分钟最多创建 10 次。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Moment")]
public class AddMomentTaskRequest
{
    /// <summary>
    /// 获取或设置指定的发表范围（不填表示执行者为应用可见范围内所有成员）。
    /// </summary>
    [JsonPropertyName("visible_range")]
    public MomentVisibleRange? VisibleRange { get; set; }

    /// <summary>
    /// 获取或设置文本内容（不能与附件同时为空；最多 2000 字 / 4000 字节，超长报错 invalid text size）。
    /// </summary>
    [JsonPropertyName("text")]
    public MomentText? Text { get; set; }

    /// <summary>
    /// 获取或设置附件列表（不能与文本同时为空；最多 9 个图片、或 1 个视频、或 1 个链接，
    /// 类型三选一，混用报错 invalid attachments msgtype）。
    /// </summary>
    [JsonPropertyName("attachments")]
    public List<MomentAttachment>? Attachments { get; set; }
}
