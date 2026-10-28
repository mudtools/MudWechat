// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「获取小程序代码隐私信息」应答（<c>get_code_privacy_info</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/getCodePrivacyInfo.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpCodePrivacyInfoResponse : OpenPlatformResponse
{
    /// <summary>未声明用户隐私授权用途的隐私接口列表（官方 <c>without_auth_list</c>，array）。</summary>
    [JsonPropertyName("without_auth_list")]
    public List<string>? WithoutAuthList { get; set; }

    /// <summary>未配置隐私协议的隐私接口列表（官方 <c>without_conf_list</c>，array）。</summary>
    [JsonPropertyName("without_conf_list")]
    public List<string>? WithoutConfList { get; set; }
}
