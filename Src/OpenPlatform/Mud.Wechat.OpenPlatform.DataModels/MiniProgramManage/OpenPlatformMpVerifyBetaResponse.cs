// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「试用小程序快速转正」应答（<c>verifybetaweapp</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/register-management/fast-regist-beta/verfifyBetaMiniprogram.html"/>。
/// </summary>
/// <remarks><para>新版本（<c>new_version=true</c>）返回 <c>authorized_url</c> 认证链接，由服务商引导法人访问；旧版本不下发本字段。</para></remarks>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpVerifyBetaResponse : OpenPlatformResponse
{
    /// <summary>认证链接（官方字段名 <c>authorized_url</c>，string，新版本返回）。</summary>
    [JsonPropertyName("authorized_url")]
    public string? AuthorizedUrl { get; set; }
}
