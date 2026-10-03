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
/// 模板卡片事件载荷（<c>template_card_event</c> / <c>template_card_menu_event</c> 两键；官方 path 90240）。
/// </summary>
/// <remarks>
/// <para>
/// <b>结构族（可空超集，ADR-14）</b>：两事件的报文字段集合一致 ——
/// <c>template_card_event</c>（点击卡片按钮）在投票/多选类卡片下<b>额外</b>携带 <c>SelectedItems</c>，
/// 右上角菜单事件不携带该节点 ⇒ 解析为空列表。按「结构族 = 可空超集」合并为一个载荷，
/// 具体类别由信封 <c>Event</c> 判别。
/// </para>
/// <para>
/// <b>处置语义</b>：<c>ResponseCode</c> 调用更新卡片接口时 72 小时内有效且<b>只能使用一次</b>
/// ⇒ 处理器须幂等（超时重推场景不得重复消费同一 ResponseCode）。
/// </para>
    /// <para>
    /// <c>SelectedItems</c> 为二级嵌套（<c>SelectedItem/OptionIds/OptionId</c>），
    /// 经 G-ADR-17 <c>ItemsObject</c> 通道声明化。
    /// </para>
    /// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.All,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] {
        WechatCallbackEventTypes.TemplateCardEvent,
        WechatCallbackEventTypes.TemplateCardMenuEvent })]
public sealed partial class TemplateCardEventPayload : WechatCallbackPayload
{
    /// <summary>按钮 key（官方 <c>EventKey</c>，与发送模板卡片时指定的 <c>btn:key</c> 相同）。</summary>
    [PayloadField("EventKey")]
    public string? EventKey { get; set; }

    /// <summary>任务 id（官方 <c>TaskId</c>，与发送模板卡片时指定的 <c>task_id</c> 相同）。</summary>
    [PayloadField("TaskId")]
    public string? TaskId { get; set; }

    /// <summary>
    /// 通用模板卡片类型（官方 <c>CardType</c>：<c>text_notice</c> / <c>news_notice</c> /
    /// <c>button_interaction</c> / <c>vote_interaction</c> / <c>multiple_interaction</c>）。
    /// </summary>
    [PayloadField("CardType")]
    public string? CardType { get; set; }

    /// <summary>
    /// 用于调用更新卡片接口的 ResponseCode（官方 <c>ResponseCode</c>；72 小时内有效且只能使用一次）。
    /// </summary>
    [PayloadField("ResponseCode")]
    public string? ResponseCode { get; set; }

    /// <summary>企业应用 id（官方 <c>AgentID</c>，整型；可在应用设置页面查看）。</summary>
    [PayloadField("AgentID")]
    public string? AgentId { get; set; }

    /// <summary>
    /// 选中项列表（官方 <c>SelectedItems/SelectedItem</c>；投票/多选类卡片按钮点击时携带，
    /// 右上角菜单事件与单选卡片不携带 ⇒ 空列表）。
    /// </summary>
    [PayloadField("SelectedItems", Format = PayloadFieldFormat.ItemsObject, ItemName = "SelectedItem")]
    public List<WechatCallbackTemplateCardSelectedItem> SelectedItems { get; set; }
        = new List<WechatCallbackTemplateCardSelectedItem>();
}
