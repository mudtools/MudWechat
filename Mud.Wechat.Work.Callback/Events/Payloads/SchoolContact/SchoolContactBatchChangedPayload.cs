// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work.Abstractions.Callback.Payloads;

namespace Mud.Wechat.Work.Callback.Events.Payloads;

/// <summary>
/// 家校通讯录批量变更事件载荷（官方事件键 <c>change_school_contact_batch</c>，官方 97281）。
/// </summary>
/// <remarks>
/// <para>
/// <b>批量合并形态</b>：学校在家校通讯录中发生成员变更或部门变更时，短时间内触发的多个变更事件
/// 会被官方合并为一个批量事件推送，以提高回调效率。<see cref="ChangeItems"/> 的官方 <c>ChangeList</c>
/// 是<b>根下重复同名兄弟元素</b>（无包装容器），经根层同名兄弟合并投影 +
/// <see cref="WechatPayloadConverter.RepeatSchoolContactChangeItems"/> 读取（单元素报文同样覆盖）。
/// <b>目前最大支持合并 1000 条，后续可能会根据情况调整</b>，以实际收到数据为准。
/// </para>
/// <para>
/// <b>信封与键</b>：经第三方应用/套件的<b>指令回调 URL</b> 推送，套件信封外层事件值在
/// <c>InfoType</c> 节点（<c>SuiteId</c>/<c>AuthCorpId</c>/<c>InfoType</c>/<c>TimeStamp</c>，
/// 信封字段不入载荷）；信封 <c>InfoType</c> 即事件键（逐键自指，无 <c>ChangeType</c> 顶层分组段——
/// 变更类型在每条 <c>ChangeList</c> 项内）。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/97281">path 97281 家校通讯录批量变更事件（第三方，套件信封）</see>。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.SchoolContactChange,
    SupportedAppTypes = WechatAppTypeSet.ThirdParty,
    RequiredChannel = WechatCallbackChannel.Suite,
    EventTypes = new[] { WechatCallbackEventTypes.ChangeSchoolContactBatch })]
public sealed partial class SchoolContactBatchChangedPayload : WechatCallbackPayload
{
    /// <summary>
    /// 变更列表（官方 <c>ChangeList</c>，根下重复同名兄弟元素；节点缺失 ⇒ 空列表）。
    /// 每条含 <see cref="WechatCallbackSchoolContactBatchChangeItem.ChangeType"/>（11 类值域）/
    /// <see cref="WechatCallbackSchoolContactBatchChangeItem.Id"/>/
    /// <see cref="WechatCallbackSchoolContactBatchChangeItem.NewId"/>（仅 update 项且 userid 变更时）/
    /// <see cref="WechatCallbackSchoolContactBatchChangeItem.TimeStamp"/>。
    /// </summary>
    [PayloadField("ChangeList", Method = nameof(WechatPayloadConverter.RepeatSchoolContactChangeItems))]
    public List<WechatCallbackSchoolContactBatchChangeItem> ChangeItems { get; set; } =
        new List<WechatCallbackSchoolContactBatchChangeItem>();
}
