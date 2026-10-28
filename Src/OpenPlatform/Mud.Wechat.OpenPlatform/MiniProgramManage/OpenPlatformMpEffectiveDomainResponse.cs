// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「获取有效服务器域名」应答（<c>get_effective_domain</c>，官方为 POST、无请求体）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/domain-management/getEffectiveServerDomain.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpEffectiveDomainResponse : OpenPlatformResponse
{
    /// <summary>公众平台后台配置的域名（官方 <c>mp_domain</c>）。</summary>
    [JsonPropertyName("mp_domain")]
    public OpenPlatformMpDomainSet? MpDomain { get; set; }

    /// <summary>第三方接口配置的域名（官方 <c>third_domain</c>）。</summary>
    [JsonPropertyName("third_domain")]
    public OpenPlatformMpDomainSet? ThirdDomain { get; set; }

    /// <summary><c>modify_domain_directly</c> 配置的域名（官方 <c>direct_domain</c>）。</summary>
    [JsonPropertyName("direct_domain")]
    public OpenPlatformMpDomainSet? DirectDomain { get; set; }

    /// <summary>实际生效的域名列表（官方 <c>effective_domain</c>）。</summary>
    [JsonPropertyName("effective_domain")]
    public OpenPlatformMpDomainSet? EffectiveDomain { get; set; }
}
