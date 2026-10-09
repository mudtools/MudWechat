// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Pay.DataModels.PayScore;

namespace Mud.Wechat.Pay.Callback;

/// <summary>
/// 回调域源生成 JSON 上下文（<b>手写</b>；覆盖 net6.0 / net8.0 / net10.0）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何需要手写一份，而不是直接用 DataModels 生成的 <c>CallbackJsonContext</c></b>：
/// 本仓的 <c>scripts/GenerateJsonContext.ps1</c>（<c>mud-jsonctx</c>）产出的上下文<b>统一包在
/// <c>#if NET8_0_OR_GREATER</c> 内</b>（因为全仓 AOT 门禁只跑 net8.0/net10.0）。
/// 而支付线的最低 TFM 是 <b>net6.0</b>（受控 TFM 例外，见 <c>Mud.Wechat.Pay.Abstractions.csproj</c>）⇒
/// 支付回调包若直接用那份上下文，<b>net6.0 分支编译失败</b>（CS0103，本轮实测踩到）。
/// </para>
/// <para>
/// <b>手写上下文是既存先例</b>：AGENTS §3 明确「Abstractions 域手写登记进 <c>AuthenticationJsonContext</c>」——
/// 手写<b>新</b>上下文与「手改 <c>Generated/*.g.cs</c>」是两件事，后者仍被禁止。
/// </para>
/// <para>
/// <b>刻意不设 <c>PropertyNamingPolicy</c></b>：<c>JsonKnownNamingPolicy.SnakeCaseLower</c> 自 <b>net8.0</b>
/// 才存在；而本域 DTO 的<b>每个属性都带显式 <c>[JsonPropertyName]</c></b>（官方原文），
/// 命名策略对它们无任何作用 —— 设了反而在 net6.0 上编译失败。
/// </para>
/// <para>
/// 本上下文与 DataModels 生成的 <c>CallbackJsonContext</c> <b>覆盖同一批类型</b>，这是<b>有意的冗余</b>：
/// 前者服务本包（全 TFM）的解析，后者服务组件序列化管线（net8.0+，见 <c>PayJsonResolverExtensions</c>）。
/// 两份都是源生成、零反射，不构成 AOT 风险。
/// </para>
/// </remarks>
[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false)]
[JsonSerializable(typeof(WechatPayNotification))]
[JsonSerializable(typeof(WechatPayNotificationResource))]
[JsonSerializable(typeof(WechatPayTransactionResource))]
[JsonSerializable(typeof(WechatPayTransactionPayer))]
[JsonSerializable(typeof(WechatPayTransactionAmount))]
[JsonSerializable(typeof(WechatPayRefundResource))]
[JsonSerializable(typeof(WechatPayRefundAmount))]
[JsonSerializable(typeof(WechatPayProfitSharingResource))]
[JsonSerializable(typeof(WechatPayProfitSharingReceiver))]
[JsonSerializable(typeof(WechatPayPayScorePaidResource))]
[JsonSerializable(typeof(WechatPayPayScoreConfirmResource))]
[JsonSerializable(typeof(WechatPayPayScoreAuthorizationResource))]
[JsonSerializable(typeof(WechatPayPayScoreCollection))]
[JsonSerializable(typeof(WechatPayPayScoreCollectionDetail))]
// 支付分载荷复用的子类型（定义在 PayScore 域，但本上下文在 net6.0 也须能解析它们）。
[JsonSerializable(typeof(PayScorePostPayment))]
[JsonSerializable(typeof(PayScorePostDiscount))]
[JsonSerializable(typeof(PayScoreRiskFund))]
[JsonSerializable(typeof(PayScoreTimeRange))]
[JsonSerializable(typeof(PayScoreLocation))]
[JsonSerializable(typeof(PayScorePromotionDetail))]
[JsonSerializable(typeof(PayScorePromotionGoodsDetail))]
internal partial class WechatPayCallbackJsonContext : JsonSerializerContext
{
}
