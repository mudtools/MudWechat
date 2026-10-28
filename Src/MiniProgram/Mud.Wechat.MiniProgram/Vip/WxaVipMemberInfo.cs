// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vip;

/// <summary>小程序会员信息（请求侧 <c>info</c>；<c>add</c> / <c>update</c> 两端点参数表完全一致 ⇒ 共用）。</summary>
/// <remarks>
/// <para>
/// <b>历史字段禁建模（官方变更日志，2026-10-12 核验）</b>：官方曾提供
/// <c>receive_register_coupon</c>（注册领券开关），现已从参数表<b>删除</b>——
/// 不得建模该字段，也不得回潮。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class WxaVipMemberInfo
{
    /// <summary>小程序定义的用户会员身份（<c>identity</c>，必填）。</summary>
    [JsonPropertyName("identity")]
    public string? Identity { get; set; }

    /// <summary>小程序定义的用户会员 id（<c>outer_userid</c>，选填；小程序侧自有会员体系的外部主键）。</summary>
    [JsonPropertyName("outer_userid")]
    public string? OuterUserId { get; set; }
}
