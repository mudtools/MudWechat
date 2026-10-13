// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.RealName;

/// <summary>校验实名信息请求。</summary>
[HttpJsonSerializable(SerializerClassName = "RealName")]
public class WxaRealNameCheckRequest
{
    /// <summary>用户 openid。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>真实姓名。</summary>
    [JsonPropertyName("real_name")]
    public string? RealName { get; set; }

    /// <summary>证件号码。</summary>
    [JsonPropertyName("cred_id")]
    public string? CredId { get; set; }

    /// <summary>证件类型：1 身份证，2 护照，3 军官证，4 回乡证，5 台胞证，6 港澳居民来往内地通行证。</summary>
    [JsonPropertyName("cred_type")]
    public string? CredType { get; set; }

    /// <summary>业务方页面获得的 code。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }
}

/// <summary>校验实名信息响应。</summary>
[HttpJsonSerializable(SerializerClassName = "RealName")]
public class WxaRealNameCheckResponse : WxaResponse
{
    /// <summary>校验后的 openid。</summary>
    [JsonPropertyName("verify_openid")]
    public string? VerifyOpenId { get; set; }

    /// <summary>校验后的真实姓名。</summary>
    [JsonPropertyName("verify_real_name")]
    public string? VerifyRealName { get; set; }
}
