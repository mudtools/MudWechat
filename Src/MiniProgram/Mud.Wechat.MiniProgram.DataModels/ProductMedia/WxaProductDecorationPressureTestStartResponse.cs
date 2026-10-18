// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductMedia;

/// <summary>
/// 发起压测回包（<c>POST /product/decoration/wxaproduct_decoration_pressure_test</c>）。
/// </summary>
/// <remarks>
/// <para><b>空请求体</b>：官方请求示例为 <c>{ }</c>、无任何参数 ⇒ 方法不接收请求 DTO。</para>
/// <para><b>配额</b>：每个小商店每月 4 次压测配额（官方原文），超出返回 10037。</para>
/// <para><b>请求方式</b>：压测系统以 200qps 速率打开小商店首页。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductMedia")]
public class WxaProductDecorationPressureTestStartResponse : WxaResponse
{
    /// <summary>压测 id（后续查询状态 / 报告使用）。</summary>
    [JsonPropertyName("pressure_id")]
    public long? PressureId { get; set; }
}
