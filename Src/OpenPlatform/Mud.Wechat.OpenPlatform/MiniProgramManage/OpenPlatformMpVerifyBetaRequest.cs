// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「试用小程序快速转正」请求（<c>verifybetaweapp</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/register-management/fast-regist-beta/verfifyBetaMiniprogram.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpVerifyBetaRequest
{
    /// <summary>企业法人认证需要的信息（官方必填 <c>verify_info</c>，object）。</summary>
    [JsonPropertyName("verify_info")]
    public OpenPlatformMpBetaVerifyInfo? VerifyInfo { get; set; }

    /// <summary>接口新旧版本标识：使用新接口须填 <c>true</c>，缺省 <c>false</c>（官方选填 <c>new_version</c>，boolean）。</summary>
    [JsonPropertyName("new_version")]
    public bool? NewVersion { get; set; }
}
