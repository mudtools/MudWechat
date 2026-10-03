// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 模板卡片选中项（<c>template_card_event</c> 报文的 <c>SelectedItems/SelectedItem</c> 节点；官方 path 90240）。
/// </summary>
/// <remarks>
/// 由 <c>WechatPayloadConverter.ParseSelectedItems</c> 在转换器内手工组装：
/// 项内含 <c>OptionIds/OptionId</c> 二级嵌套列表，超出生成器「容器 → 单层同构项」的表达力。
/// 投票/多选类卡片才携带该项，其余报文 ⇒ 空列表。
/// </remarks>
public sealed class WechatCallbackTemplateCardSelectedItem
{
    /// <summary>问题的 key 值（官方 <c>QuestionKey</c>）。</summary>
    public string? QuestionKey { get; set; }

    /// <summary>对应问题的选项列表（官方 <c>OptionIds/OptionId</c>）。</summary>
    public List<string> OptionIds { get; set; } = new List<string>();
}
