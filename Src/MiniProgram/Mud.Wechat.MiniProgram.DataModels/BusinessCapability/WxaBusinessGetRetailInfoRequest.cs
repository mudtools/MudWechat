// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.BusinessCapability;

/// <summary>
/// 门店信息查询请求体（<c>POST /wxa/business/getretailinfo</c>）。
/// </summary>
/// <remarks>
/// <para><b>二选一</b>：<c>openid</c> 与 <c>mobile_phone</c> 至少给一个；两者同时给时以 <c>openid</c> 为准（官方原文）。</para>
/// <para><b>敏感面</b>：<c>openid</c> 与手机号一律禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BusinessCapability")]
public class WxaBusinessGetRetailInfoRequest
{
    /// <summary>门店成员 openid（与手机号二选一，优先）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>门店联系人手机号（与 openid 二选一）。</summary>
    [JsonPropertyName("mobile_phone")]
    public string? MobilePhone { get; set; }
}
