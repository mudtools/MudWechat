// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.ProfitSharing;

/// <summary>
/// 删除分账接收方（<c>POST /v3/profitsharing/receivers/delete</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012529590"/>
/// （2026-10-09 逐字段核验；更新时间 2025.09.29；支持商户：普通商户）。
/// </para>
/// <para>
/// <b>⚠️ <c>appid</c> 在本接口是必填</b>（与「请求分账」里它是选填、且仅 <c>PERSONAL_OPENID</c> 时必填
/// <b>不同</b>）—— 同一参数在两个接口的必填性不一致是官方原样，不得「统一」。
/// </para>
/// <para>
/// <b>业务含义（官方原文）</b>：删除后<b>不再支持</b>把该商户结算后的资金分给该接收方。
/// 商户接收方数量上限 2 万，达上限时须先删除未使用的接收方。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProfitSharing")]
public class ProfitSharingDeleteReceiverRequest
{
    /// <summary>公众账号 ID（<c>appid</c>，<b>必填</b> string(32)）：须与 <c>mchid</c> 有绑定关系。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>接收方类型（<c>type</c>，必填）：见 <see cref="ProfitSharingReceiverTypes"/>。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>接收方账号（<c>account</c>，必填 string(64)）。</summary>
    [JsonPropertyName("account")]
    public string? Account { get; set; }
}

/// <summary>
/// 删除分账接收方应答（<b>仅</b> <c>type</c> / <c>account</c> 两字段）。
/// </summary>
/// <remarks>
/// <b>为何不直接复用 <see cref="ProfitSharingReceiver"/></b>：官方本页应答字段表只有两项，
/// 而添加接收方的应答还含 <c>name</c> / <c>relation_type</c> / <c>custom_relation</c>。
/// 复用会得到「本端点永远不会返回的字段」，让调用方误以为删除后仍能从应答读到关系信息 ——
/// 故按官方表精确建模（两页字段表不同即两个类型）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProfitSharing")]
public class ProfitSharingDeleteReceiverResponse : WechatPayResponse
{
    /// <summary>接收方类型（<c>type</c>，必填）：见 <see cref="ProfitSharingReceiverTypes"/>。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>接收方账号（<c>account</c>，必填 string(64)）。</summary>
    [JsonPropertyName("account")]
    public string? Account { get; set; }
}
