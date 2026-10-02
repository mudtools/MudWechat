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
/// 标签成员变更事件载荷（<c>update_tag</c>；官方 90972）。
/// </summary>
/// <remarks>
/// <para>
/// 官方列表字段以<b>逗号分隔串</b>承载：
/// <c>AddUserItems</c>/<c>DelUserItems</c> 为 UserId 串（<b>字符串</b>），
/// <c>AddPartyItems</c>/<c>DelPartyItems</c> 为部门 id 串（<b>数值</b>）。
/// 二者<b>刻意不对称</b>地映射为 <c>List&lt;string&gt;</c> 与 <c>List&lt;long&gt;</c>，与官方类型一致。
/// </para>
/// <para>
/// <b>时序不保证</b>（官方明示）：标签的成员变更与成员/部门自身变更事件<b>时序不保证</b>，
/// 须以「获取标签成员」等拉取接口对齐（v1 方案 §10.4）—— 处理器<b>不得</b>依赖本事件与其他事件的相对顺序。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
public sealed partial class ContactTagChangedPayload : WechatCallbackPayload
{
    /// <summary>标签 id（官方 <c>TagId</c>）。</summary>
    [PayloadField("TagId")]
    public string? TagId { get; set; }

    /// <summary>标签中新增的成员 UserId（官方 <c>AddUserItems</c> 逗号串）。</summary>
    [PayloadField("AddUserItems", Format = PayloadFieldFormat.Delimited, Separator = ',')]
    public List<string> AddedUserIds { get; set; } = new List<string>();

    /// <summary>标签中移除的成员 UserId（官方 <c>DelUserItems</c> 逗号串）。</summary>
    [PayloadField("DelUserItems", Format = PayloadFieldFormat.Delimited, Separator = ',')]
    public List<string> RemovedUserIds { get; set; } = new List<string>();

    /// <summary>标签中新增的部门 id（官方 <c>AddPartyItems</c> 逗号串）。</summary>
    [PayloadField("AddPartyItems", Format = PayloadFieldFormat.Delimited, Separator = ',')]
    public List<long> AddedPartyIds { get; set; } = new List<long>();

    /// <summary>标签中移除的部门 id（官方 <c>DelPartyItems</c> 逗号串）。</summary>
    [PayloadField("DelPartyItems", Format = PayloadFieldFormat.Delimited, Separator = ',')]
    public List<long> RemovedPartyIds { get; set; } = new List<long>();
}
