// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Auth;

/// <summary>获取手机号请求体（<c>POST /wxa/business/getuserphonenumber</c>）。</summary>
/// <remarks>
/// <b><c>code</c> 一次性</b>：由小程序端 <c>&lt;button open-type="getPhoneNumber"&gt;</c> 回调取得，
/// 有效期与可用次数由官方表达；<b>不可复用</b>（重复使用会被官方拒绝）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Auth")]
public class WxaGetPhoneNumberRequest
{
    /// <summary>手机号获取凭证（<c>code</c>，必填），前端 <c>getPhoneNumber</c> 回调返回。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }
}
