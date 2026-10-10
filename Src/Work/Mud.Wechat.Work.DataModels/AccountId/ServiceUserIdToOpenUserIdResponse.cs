// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.AccountId;

/// <summary>
/// userid 的转换（未明确企业身份场景）响应体（<c>/cgi-bin/service/batch/userid_to_openuserid</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AccountId")]
public class ServiceUserIdToOpenUserIdResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置智能机器人所在企业 ID。
    /// </summary>
    [JsonPropertyName("open_corpid")]
    public string? OpenCorpId { get; set; }

    /// <summary>
    /// 获取或设置 ID 转换结果列表（企业主体加密 userid → 服务商主体 open_userid）。
    /// </summary>
    [JsonPropertyName("items")]
    public List<OpenUserIdMapItem>? Items { get; set; }

    /// <summary>
    /// 获取或设置无法转换的 open_userid 列表（如果传入的是明文 userid，也会在该列表中返回）。
    /// </summary>
    [JsonPropertyName("invalid_open_userid_list")]
    public List<string>? InvalidOpenUserIdList { get; set; }
}
