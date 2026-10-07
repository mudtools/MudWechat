// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SubscriptionNotice;

/// <summary>
/// 发送订阅通知（<c>message/subscribe/bizsend</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<c>template_id</c>/<c>touser</c>/<c>data</c> 必填；
/// <c>page</c> 可选（点击模板卡片后的跳转页面，<b>仅限本小程序内页面</b>，不填则无跳转）。
/// </para>
/// <para>
/// <b>官方文档矛盾（照录）</b>：①<c>miniprogram_state</c>/<c>lang</c> 参数表标必填，但说明写
/// 「默认为正式版」「默认为 zh_CN」⇒ SDK 建模为可空（留空由官方取默认）；②请求示例含
/// <c>miniprogram</c>（appid/pagepath）与 <c>client_msg_id</c> 字段但参数表无对应行
/// ⇒ 按示例形态建模；③<c>data</c> 说明示例键为 <c>phrase3</c>/<c>name1</c>/<c>date2</c>、
/// 请求示例用 <c>keyword1/2/3</c>、约束表用 <c>xxx.DATA</c>——三处键形态互不一致
/// ⇒ <b>value-only 字段袋 + 键名原样透传</b>（方案 P0-d 裁决：官方关键词编号可变，强类型化会漏分支）。
/// </para>
/// <para>
/// <b>data 值类型约束（官方约束表 12 前缀）</b>：thing 20 字符内 / number 32 位内 /
/// symbol 5 位内 / character_string 32 位内 / time·date（时间段用 ~ 连接）/
/// amount（1 币种符号 + 10 位内数字可带小数）/ phone_number 17 位内 / car_number 8 位内 /
/// name（中文 10 汉字内、英文 20 字母内）/ phrase 5 个纯汉字内 / letter 32 位内。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SubscriptionNotice")]
public class MpSendSubscribeNoticeRequest
{
    /// <summary>获取或设置接收者（用户）的 openid（官方 <c>touser</c>，必填）。</summary>
    [JsonPropertyName("touser")]
    public string ToUser { get; set; } = string.Empty;

    /// <summary>获取或设置所需下发的订阅模板 id（官方 <c>template_id</c>，必填）。</summary>
    [JsonPropertyName("template_id")]
    public string TemplateId { get; set; } = string.Empty;

    /// <summary>获取或设置点击模板卡片后的跳转页面（官方 <c>page</c>，可选；仅限本小程序内页面，不填则无跳转）。</summary>
    [JsonPropertyName("page")]
    public string? Page { get; set; }

    /// <summary>
    /// 获取或设置模板内容（官方 <c>data</c>，必填；key 为模板关键词编号——
    /// <b>原样透传</b>（官方 thing1.DATA / keyword1 等形态并存，照录），值为 <see cref="MpSubscribeNoticeDataValue"/>）。
    /// </summary>
    [JsonPropertyName("data")]
    public Dictionary<string, MpSubscribeNoticeDataValue> Data { get; set; } = new Dictionary<string, MpSubscribeNoticeDataValue>();

    /// <summary>获取或设置跳转小程序类型（官方 <c>miniprogram_state</c>：developer/trial/formal；官方标必填但写「默认为正式版」，矛盾照录）。</summary>
    [JsonPropertyName("miniprogram_state")]
    public string? MiniProgramState { get; set; }

    /// <summary>获取或设置进入小程序查看的语言（官方 <c>lang</c>：zh_CN/en_US/zh_HK/zh_TW；官方标必填但写「默认为 zh_CN」，矛盾照录）。</summary>
    [JsonPropertyName("lang")]
    public string? Lang { get; set; }

    /// <summary>获取或设置跳转小程序信息（官方 <c>miniprogram</c>；官方请求示例含此字段但参数表无行，矛盾照录——与模板消息域同形共用）。</summary>
    [JsonPropertyName("miniprogram")]
    public Template.MpTemplateMiniProgram? MiniProgram { get; set; }

    /// <summary>获取或设置防重入 id（官方 <c>client_msg_id</c>；官方请求示例含此字段但参数表无行，矛盾照录）。</summary>
    [JsonPropertyName("client_msg_id")]
    public string? ClientMsgId { get; set; }
}

/// <summary>订阅通知 data 字段值（官方 <c>{"value": …}</c> 形态）。</summary>
[HttpJsonSerializable(SerializerClassName = "SubscriptionNotice")]
public class MpSubscribeNoticeDataValue
{
    /// <summary>获取或设置参数值（官方 <c>value</c>；约束随关键词类型前缀，见请求 DTO remarks）。</summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

/// <summary>发送订阅通知（<c>message/subscribe/bizsend</c>）响应。</summary>
/// <remarks>
/// 官方契约：响应仅 <c>errcode</c>/<c>errmsg</c>（本页无独立错误码表，官方指向通用错误码文档，照录）。
/// 发送结果另有回调事件 <c>subscribe_msg_sent_event</c> 异步回执（<see cref="Abstractions.Callback.MpSubscriptionEventTypes.Sent"/>）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SubscriptionNotice")]
public class MpSendSubscribeNoticeResponse : MpResponse
{
}
