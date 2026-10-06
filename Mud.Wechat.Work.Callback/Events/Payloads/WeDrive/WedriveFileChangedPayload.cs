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
/// 微盘文件变更事件载荷（<b>结构族</b>：覆盖 <c>Event = wedrive_file_change</c> 的 5 个 <c>ChangeType</c>：
/// <c>create_file</c> / <c>rename_file</c> / <c>update_file</c> / <c>delete_file</c> / <c>move_file</c>；
/// 官方 97900 自建 · 97975 第三方 · 97934 代开发）。
/// </summary>
/// <remarks>
/// <para>
/// <b>结构族而非逐事件 DTO（ADR-1）</b>：官方仅提供一个概述式事件页（示例 <c>ChangeType</c> 为占位值
/// <c>xxx_file</c>），实际值集在参数表中枚举，5 类变更报文同构（信封 + <see cref="FileIds"/>），
/// 具体类别由信封 <c>ChangeType</c> 判别。
/// </para>
/// <para>
/// <see cref="FileIds"/> 官方明示「可能有多个 <c>FileId</c> 节点，表示多个文件」——
/// <b>根下重复同名兄弟元素</b>形态，经 <see cref="WechatPayloadConverter.RepeatSiblings"/> 读取。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/97900">path 97900 文件变更事件（企业自建）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/97975">path 97975（第三方）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/97934">path 97934（服务商代开发）</see>。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredEvent = WechatCallbackEventTypes.WedriveFileChange,
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.All,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] {
        WechatCallbackEventTypes.CreateFile,
        WechatCallbackEventTypes.RenameFile,
        WechatCallbackEventTypes.UpdateFile,
        WechatCallbackEventTypes.DeleteFile,
        WechatCallbackEventTypes.MoveFile })]
public sealed partial class WedriveFileChangedPayload : WechatCallbackPayload
{
    /// <summary>文件 id 列表（官方 <c>FileId</c>，「可能有多个 FileId 节点，表示多个文件」，根下重复同名兄弟元素）。</summary>
    [PayloadField("FileId", Method = nameof(WechatPayloadConverter.RepeatSiblings))]
    public List<string> FileIds { get; set; } = new List<string>();
}
