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
/// 智能表格字段变更事件载荷（<b>结构族</b>：覆盖 <c>Event = smart_sheet_change</c> 的
/// <c>add_filed</c>/<c>update_filed</c>/<c>delete_filed</c> 三个 <c>ChangeType</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>1 型覆盖 3 键</b>：三个子事件的报文结构逐字一致（除 <c>ChangeType</c> 值），合并为一个载荷，
/// 具体类别由信封 <c>ChangeType</c> 判别。<see cref="FieldIds"/> 为<b>根下重复同名兄弟元素</b>形态
/// （无包装容器），经 <see cref="WechatPayloadConverter.RepeatSiblings"/> 读取。
/// </para>
/// <para>
/// <b>官方拼写陷阱</b>：三个 <c>ChangeType</c> 官方原文即为 <c>add_filed</c>/<c>update_filed</c>/
/// <c>delete_filed</c>（filed，非 field），本 SDK 照抄原文、不做「修正」。
/// </para>
/// <para>
/// <b>官方业务限制（不得弱化）</b>：官方明文「<c>FieldId</c> 一次最多回调 1000 个，超过会分批回调」
/// —— 处理器不得假设一次回调即全量，需按分批语义自行聚合。
/// </para>
/// <para>
/// <b>三模式无关性（ADR-14）</b>：三份文档（自建/第三方/代开发）的报文结构与参数表逐字核对一致；
/// <c>FromUserName</c> 为「本企业成员为 userid，非本企业成员为 tmp_external_userid」（信封字段，不入载荷）。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/100987">path 100987 字段变更事件（企业自建）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/101016">path 101016（第三方）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/101018">path 101018（服务商代开发）</see>。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredEvent = WechatCallbackEventTypes.SmartSheetChange,
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.All,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] {
        WechatCallbackEventTypes.AddFiled,
        WechatCallbackEventTypes.UpdateFiled,
        WechatCallbackEventTypes.DeleteFiled })]
public sealed partial class SmartSheetFieldChangedPayload : WechatCallbackPayload
{
    /// <summary>文档 id（官方 <c>DocId</c>，单个）。</summary>
    [PayloadField("DocId")]
    public string? DocId { get; set; }

    /// <summary>子表 id（官方 <c>SheetId</c>，单个）。</summary>
    [PayloadField("SheetId")]
    public string? SheetId { get; set; }

    /// <summary>
    /// 字段 id 列表（官方 <c>FieldId</c>，根下重复同名兄弟元素）；
    /// 官方限制一次最多回调 1000 个，超过分批回调。
    /// </summary>
    [PayloadField("FieldId", Method = nameof(WechatPayloadConverter.RepeatSiblings))]
    public List<string> FieldIds { get; set; } = new List<string>();
}
