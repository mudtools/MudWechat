// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;

namespace Mud.Wechat.MiniProgram.DataModels.CityService;

/// <summary>
/// 城市服务消息通路发消息请求体（<c>POST /cityservice/sendmsgdata</c>）。
/// </summary>
/// <remarks>
/// <para><b>样式 id 与场景对应</b>：结果页填 <c>result_page_style_id</c>、办事记录填
/// <c>deal_msg_style_id</c>、页卡填 <c>card_style_id</c>；含结果页时 <c>url</c> 必填。</para>
/// <para><b>小程序场景的令牌归属</b>：通过小程序提供服务时，须使用与小程序关联、且申请了「消息通路」的
/// 公众号的 <c>access_token</c>（官方原文）。</para>
/// <para><b><c>data</c> 是模板驱动的任意对象</b>：键由模板定义，值通常含 <c>value</c> / <c>color</c>，
/// 数组字段可用 <c>[]</c> 包裹 ⇒ 建模为 <c>JsonElement</c>，不做强类型投影。</para>
/// <para><b>敏感面</b>：<c>openid</c> 与 <c>data</c>（可含用户业务信息）一律禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CityService")]
public class WxaCityServiceSendMsgDataRequest
{
    /// <summary>用户唯一标识（必填）。敏感字段，禁入日志。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>城市服务分配给公众号的模板 id（必填）。</summary>
    [JsonPropertyName("biz_template_id")]
    public string? BizTemplateId { get; set; }

    /// <summary>结果页样式 id（含结果页时必填）。</summary>
    [JsonPropertyName("result_page_style_id")]
    public string? ResultPageStyleId { get; set; }

    /// <summary>办事记录样式 id（含办事记录时必填）。</summary>
    [JsonPropertyName("deal_msg_style_id")]
    public string? DealMsgStyleId { get; set; }

    /// <summary>页卡样式 id（含页卡时必填）。</summary>
    [JsonPropertyName("card_style_id")]
    public string? CardStyleId { get; set; }

    /// <summary>订单号（必填）；同一订单号的办事记录会合并。敏感字段，禁入日志。</summary>
    [JsonPropertyName("order_no")]
    public string? OrderNo { get; set; }

    /// <summary>跳转链接（用于服务通知、结果页、待办提醒；含结果页时必填）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>模板 JSON 数据（必填），结构随模板而定。</summary>
    [JsonPropertyName("data")]
    public JsonElement? Data { get; set; }
}
