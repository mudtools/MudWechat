// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「查询签约关系」响应：签约状态。
/// </summary>
/// <remarks>
/// <para><b>三状态里只有 <c>TERMINATED</c> 是终态</b>（官方原文「该协议无法再次进行签约；可更换协议号再发起签约」）⇒ 解约后复用同一 <c>out_contract_code</c> 必然失败，须新协议号。</para>
/// <para><b>取值集官方写在说明列</b>（SIGNED / TERMINATED / UNBINDUSER，无枚举表）⇒ 照录文档、不建常量类，与守卫 XP5 的「只有官方枚举表才建常量类」口径一致。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_subscribe_contract.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayQuerySubscribeContractResponse : WxaResponse
{
    /// <summary>authorization_state（官方类型 <c>string</c>）：SIGNED: 签约生效中。TERMINATED: 生效的签约协议已被解约。此时协议已经到达终态，该协议无法再次进行签约；可更换协议号再发起签约。UNBINDUSER: 从未签约过</summary>
    [JsonPropertyName("authorization_state")]
    public string? AuthorizationState { get; set; }
}
