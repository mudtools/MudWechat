// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work.Abstractions.Callback.Payloads;

namespace Mud.Wechat.Work.Callback.Events.Payloads;

/// <summary>
/// 异步任务完成通知载荷（<c>batch_job_result</c>；官方 90973 通讯录 / 95797 上下游）。
/// </summary>
/// <remarks>
/// <para>
/// <b>双报文布局（ADR-12，机制上移上游 <c>ResolveScope</c>）</b>：
/// 通讯录任务（90973）字段为<b>顶层节点</b>；上下游任务（95797，<c>JobType = import_chain_contact</c>）
/// 字段包在 <c>BatchJob</c> 包装节点内。本契约仅声明
/// <c>ScopeFallback = "BatchJob"</c>，三级作用域判定由上游 <c>PayloadFieldMap&lt;T&gt;.ResolveScope</c> 完成：
/// 根命中任一映射元素 ⇒ 根；否则回退容器；皆无 ⇒ 根。
/// </para>
/// <para>
/// 任务提交时的 <c>media_id</c> 结果文件需以 <see cref="JobId"/> 调取结果接口下载（回调不携带）。
/// </para>
/// <para>
/// <b>JobType 级差异属语义过滤，不是安全闸（§3.9.5）</b>：
/// <c>import_chain_contact</c> 仅自建应用会产生，但官方开放面约束的对象是「上下游变更回调」这一事件面，
/// 而非某个 <c>JobType</c> 值 ⇒ 处理器按需自行判别，<b>不</b>在闸上分叉。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter), ScopeFallback = "BatchJob")]
public sealed partial class BatchJobCompletedPayload : WechatCallbackPayload
{
    /// <summary>异步任务 id（官方 <c>JobId</c>；提交任务时返回）。</summary>
    [PayloadField("JobId")]
    public string? JobId { get; set; }

    /// <summary>
    /// 任务类型（官方 <c>JobType</c>）：<c>sync_user</c>（增量更新成员）/
    /// <c>replace_user</c>（全量覆盖成员）/ <c>invite_user</c>（邀请成员加入企业）/
    /// <c>replace_party</c>（全量覆盖部门）/ <c>import_chain_contact</c>（导入上下游联系人，95797）。
    /// </summary>
    [PayloadField("JobType")]
    public string? JobType { get; set; }

    /// <summary>返回码（官方 <c>ErrCode</c>；0 = 任务完成）。</summary>
    [PayloadField("ErrCode")]
    public int? ErrCode { get; set; }

    /// <summary>对返回码的文本描述（官方 <c>ErrMsg</c>）。</summary>
    [PayloadField("ErrMsg")]
    public string? ErrMsg { get; set; }
}
