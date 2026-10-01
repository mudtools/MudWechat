// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Contacts.Batch;

/// <summary>
/// 异步导入成员请求体（<c>/cgi-bin/batch/syncuser</c> 增量更新成员 与 <c>/cgi-bin/batch/replaceuser</c> 全量覆盖成员 共用，契约官方一致）。
/// </summary>
/// <remarks>
/// <para>须拥有通讯录的写权限；csv 文件先经素材接口上传取得 <see cref="MediaId"/>。</para>
/// <para>全量覆盖成员为危险操作：文件中不存在、通讯录中存在的成员将被删除；当需删除成员多于 50 人且多于现有人数 20% 以上，
/// 或少于 50 人且多于现有人数 80% 以上时，官方将中止导入并返回相应错误码。</para>
/// </remarks>
public class BatchImportUsersRequest
{
    /// <summary>
    /// 获取或设置上传的 csv 文件的 media_id（先经素材上传接口取得）。
    /// </summary>
    [JsonPropertyName("media_id")]
    public string MediaId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置是否邀请新建的成员使用企业微信（经微信服务通知或短信或邮件下发邀请，每天自动下发一次，
    /// 最多持续 3 个工作日）。官方默认值为 <c>true</c>；不填时不出网，由官方按默认值处理。
    /// </summary>
    [JsonPropertyName("to_invite")]
    public bool? ToInvite { get; set; }

    /// <summary>
    /// 获取或设置回调信息（填写后任务完成时经回调推送事件给企业）。
    /// </summary>
    [JsonPropertyName("callback")]
    public BatchCallbackRequest? Callback { get; set; }
}
