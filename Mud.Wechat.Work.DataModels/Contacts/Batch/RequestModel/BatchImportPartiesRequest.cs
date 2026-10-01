// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Contacts.Batch;

/// <summary>
/// 全量覆盖部门请求体（<c>/cgi-bin/batch/replaceparty</c>）。
/// </summary>
/// <remarks>
/// <para>须拥有通讯录的写权限；csv 文件先经素材接口上传取得 <see cref="MediaId"/>。</para>
/// <para>覆盖规则：文件中不存在的部门当其下无任何成员或子部门时删除；仍有成员或子部门的暂缓删除，
/// 待下次导入成员把人移出后自动删除。csv 中部门名称、部门 ID、父部门 ID 必填，根部门 ID 默认为 1；
/// 排序可选（置空或 0 不修改，order 值大的排序靠前）。</para>
/// </remarks>
public class BatchImportPartiesRequest
{
    /// <summary>
    /// 获取或设置上传的 csv 文件的 media_id（先经素材上传接口取得）。
    /// </summary>
    [JsonPropertyName("media_id")]
    public string MediaId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置回调信息（填写后任务完成时经回调推送事件给企业）。
    /// </summary>
    [JsonPropertyName("callback")]
    public BatchCallbackRequest? Callback { get; set; }
}
