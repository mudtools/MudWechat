// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Identity;

/// <summary>
/// 获取用户二次验证信息请求体（<c>/cgi-bin/auth/get_tfa_info</c>）。
/// <para><see cref="Code"/> 为官方必填：用户进入二次验证页面时企业微信颁发的 code，
/// 每次授权 code 不同、只能使用一次、5 分钟未被使用自动过期。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Identity")]
public class GetTfaInfoRequest
{
    /// <summary>
    /// 获取或设置二次验证页面颁发的 code（官方必填，仅可使用一次，5 分钟未被使用自动过期）。
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }
}
