// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.AccountId;

/// <summary>
/// userid 的转换（未明确企业身份场景）请求体（<c>/cgi-bin/service/batch/userid_to_openuserid</c>）：
/// 将企业主体下的加密 userid 转换成服务商主体下的 open_userid。
/// </summary>
/// <remarks>
/// 官方限制：open_userid_list 最多不超过 1000 个；官方路由沿用 <c>userid_to_openuserid</c> 命名，
/// 实际转换方向为企业主体加密 userid → 服务商主体 open_userid（智能机器人场景，以
/// <c>provider_access_token</c> 调用）。open_userid 需要在智能机器人的可见范围内；
/// 智能机器人所在企业需已安装该服务商的第三方应用或代开发应用，且传入的 open_userid
/// 需要在已安装应用的可见范围内。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AccountId")]
public class ServiceUserIdToOpenUserIdRequest
{
    /// <summary>
    /// 获取或设置企业主体下的加密 userid 列表（官方必填，最多不超过 1000 个）。
    /// </summary>
    [JsonPropertyName("open_userid_list")]
    public List<string>? OpenUserIdList { get; set; }

    /// <summary>
    /// 获取或设置企业智能机器人 ID（官方必填）。
    /// </summary>
    [JsonPropertyName("source_botid")]
    public string? SourceBotId { get; set; }
}
