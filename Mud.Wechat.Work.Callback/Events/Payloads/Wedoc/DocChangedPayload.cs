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
/// 文档变更事件载荷（<b>结构族</b>：覆盖 <c>Event = doc_change</c> 的全部 5 个 <c>ChangeType</c>；
/// 仅 API 创建的文档、表格、智能表格触发）。
/// </summary>
/// <remarks>
/// <para>
/// <b>1 型覆盖 5 键</b>：官方 5 类变更的报文结构同构 —— 信封之外仅一个 id 列表节点，
/// 文档类（<see cref="WechatCallbackEventTypes.DocMemberChange"/>/<see cref="WechatCallbackEventTypes.DeleteDoc"/>）携带 <see cref="DocIds"/>、
/// 收集表类（<see cref="WechatCallbackEventTypes.FormComplete"/>/<see cref="WechatCallbackEventTypes.DeleteForm"/>/<see cref="WechatCallbackEventTypes.FormSettingsChange"/>）
/// 携带 <see cref="FormIds"/>，具体类别由信封 <c>ChangeType</c> 判别。
/// 列表为<b>根下重复同名兄弟元素</b>形态（无包装容器，如 <c>&lt;DocId&gt;A&lt;/DocId&gt;&lt;DocId&gt;B&lt;/DocId&gt;</c>），
/// 经 <see cref="WechatPayloadConverter.RepeatSiblings"/> 读取（依赖同名叶兄弟合并投影）。
/// </para>
/// <para>
/// <b>触发边界（官方原文）</b>：<see cref="WechatCallbackEventTypes.DocMemberChange"/> 为「API 创建的文档、表格、智能表格，
/// 有成员添加了其他成员」；<see cref="WechatCallbackEventTypes.DeleteDoc"/>/<see cref="WechatCallbackEventTypes.DeleteForm"/> 由<b>文档管理员</b>删除触发；
/// <see cref="WechatCallbackEventTypes.FormSettingsChange"/> 为「修改收集表设置，包括管理员权限变更事件（增删改）、
/// 收集范围变更事件（增删改）、其他收集表设置调整」。非 API 创建的文档不产生本族事件。
/// </para>
/// <para>
/// <b>三模式无关性（ADR-14）</b>：三份文档（自建/第三方/代开发）的报文结构与参数表逐字核对一致
/// （文档类 <c>DocId</c> 列表、收集表类 <c>FormId</c> 列表），差异只是「值是否出现」。
/// <c>FromUserName</c> 为操作成员 UserID（信封字段，不入载荷）。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/97833">path 97833 修改文档成员事件（企业自建）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/97839">path 97839（第三方）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/97836">path 97836（服务商代开发）</see>；
/// 删除文档 <see href="https://developer.work.weixin.qq.com/document/path/97834">path 97834</see>（自建）/
/// <see href="https://developer.work.weixin.qq.com/document/path/97840">path 97840</see>（第三方）/
/// <see href="https://developer.work.weixin.qq.com/document/path/97837">path 97837</see>（代开发）；
/// 收集表完成 <see href="https://developer.work.weixin.qq.com/document/path/97835">path 97835</see>（自建）/
/// <see href="https://developer.work.weixin.qq.com/document/path/97841">path 97841</see>（第三方）/
/// <see href="https://developer.work.weixin.qq.com/document/path/97838">path 97838</see>（代开发）；
/// 删除收集表 <see href="https://developer.work.weixin.qq.com/document/path/98095">path 98095</see>（自建）/
/// <see href="https://developer.work.weixin.qq.com/document/path/98055">path 98055</see>（第三方）/
/// <see href="https://developer.work.weixin.qq.com/document/path/98097">path 98097</see>（代开发）；
/// 修改收集表设置 <see href="https://developer.work.weixin.qq.com/document/path/98096">path 98096</see>（自建）/
/// <see href="https://developer.work.weixin.qq.com/document/path/98056">path 98056</see>（第三方）/
/// <see href="https://developer.work.weixin.qq.com/document/path/98098">path 98098</see>（代开发）。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredEvent = WechatCallbackEventTypes.DocChange,
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.All,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] {
        WechatCallbackEventTypes.DocMemberChange,
        WechatCallbackEventTypes.DeleteDoc,
        WechatCallbackEventTypes.FormComplete,
        WechatCallbackEventTypes.DeleteForm,
        WechatCallbackEventTypes.FormSettingsChange })]
public sealed partial class DocChangedPayload : WechatCallbackPayload
{
    /// <summary>
    /// 文档 id 列表（官方 <c>DocId</c>，根下重复同名兄弟元素）；
    /// 仅文档类事件（<c>doc_member_change</c>/<c>delete_doc</c>）携带，收集表类事件为空列表。
    /// </summary>
    [PayloadField("DocId", Method = nameof(WechatPayloadConverter.RepeatSiblings))]
    public List<string> DocIds { get; set; } = new List<string>();

    /// <summary>
    /// 收集表 id 列表（官方 <c>FormId</c>，根下重复同名兄弟元素）；
    /// 仅收集表类事件（<c>form_complete</c>/<c>delete_form</c>/<c>form_settings_change</c>）携带，
    /// 文档类事件为空列表。
    /// </summary>
    [PayloadField("FormId", Method = nameof(WechatPayloadConverter.RepeatSiblings))]
    public List<string> FormIds { get; set; } = new List<string>();
}
