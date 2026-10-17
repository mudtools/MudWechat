// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// DNS 预解析域名项（<c>prefetch_dns_domain</c> 元素）：<c>set</c> 请求仅需 <c>url</c>，<c>get</c> 应答回显 <c>url</c> + <c>status</c>（0=正常），故两支共用一支 DTO。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/domain-management/getPrefetchDomain.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpPrefetchDomainItem
{
    /// <summary>预解析域名 URL（官方 <c>url</c>，string）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>域名状态（官方应答 <c>status</c>，number，0=正常；请求侧不填）。</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }
}
