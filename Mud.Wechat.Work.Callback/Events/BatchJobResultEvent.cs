// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Callback.Events;

/// <summary>
/// 异步任务完成通知事件（<c>batch_job_result</c>；官方 90973）。
/// </summary>
/// <remarks>
/// 对应通讯录异步导入/邀请能力（<c>sync_user</c>/<c>replace_user</c>/<c>invite_user</c>/<c>replace_party</c>）与
/// 上下游联系人导入（<c>import_chain_contact</c>，官方 95797）的完成推送；任务提交时的 <c>media_id</c> 结果文件
/// 需以 <see cref="JobId"/> 调取结果接口下载（回调不携带）。
/// <b>双报文布局</b>：通讯录任务字段为顶层节点，上下游任务字段包在 <c>BatchJob</c> 包装节点内——
/// 解析器同批兼容（同 Event 键、按字段位置自适应）。
/// </remarks>
public class BatchJobResultEvent
{
    /// <summary>异步任务 id（提交任务时返回）。</summary>
    public string? JobId { get; set; }

    /// <summary>任务类型：sync_user（增量更新成员）/ replace_user（全量覆盖成员）/
    /// invite_user（邀请成员加入企业）/ replace_party（全量覆盖部门）/
    /// import_chain_contact（导入上下游联系人，官方 95797）。</summary>
    public string? JobType { get; set; }

    /// <summary>返回码（0 = 任务完成；具体含义见任务提交接口文档）。</summary>
    public string? ErrCode { get; set; }

    /// <summary>对返回码的文本描述。</summary>
    public string? ErrMsg { get; set; }
}
