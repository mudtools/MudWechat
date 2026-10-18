// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Mud.Wechat.OpenPlatform.DataModels.Component;

/// <summary>
/// 快速创建试用小程序应答（<c>POST /wxa/component/fastregisterbetaweapp</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/register-management/fast-regist-beta/registerBetaMiniprogram.html"/></para>
/// <para><c>unique_id</c> 用于关联微信用户与后续产生的 appid；<c>authorize_url</c> 需发给用户在微信中打开完成授权。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformFastRegisterBetaWeappResponse : OpenPlatformResponse
{
    /// <summary>该请求的唯一标识符，用于关联微信用户和后面产生的 appid。</summary>
    [JsonPropertyName("unique_id")]
    public string? UniqueId { get; set; }

    /// <summary>用户授权确认 URL（需在微信中打开）。</summary>
    [JsonPropertyName("authorize_url")]
    public string? AuthorizeUrl { get; set; }
}
