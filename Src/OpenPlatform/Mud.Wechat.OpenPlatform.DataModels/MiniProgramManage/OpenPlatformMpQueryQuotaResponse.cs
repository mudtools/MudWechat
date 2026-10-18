// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「查询剩余审核 / 加急审核次数」（<c>queryquota</c>）应答。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/setCodeAuditQuota.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpQueryQuotaResponse : OpenPlatformResponse
{
    /// <summary>剩余普通审核次数（官方 <c>rest</c>，number）。</summary>
    [JsonPropertyName("rest")]
    public long Rest { get; set; }

    /// <summary>普通审核次数上限（官方 <c>limit</c>，number）。</summary>
    [JsonPropertyName("limit")]
    public long Limit { get; set; }

    /// <summary>剩余加急审核次数（官方 <c>speedup_rest</c>，number）。</summary>
    [JsonPropertyName("speedup_rest")]
    public long SpeedupRest { get; set; }

    /// <summary>加急审核次数上限（官方 <c>speedup_limit</c>，number）。</summary>
    [JsonPropertyName("speedup_limit")]
    public long SpeedupLimit { get; set; }
}
