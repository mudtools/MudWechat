// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.AccountId;

/// <summary>
/// tmp_external_userid 的转换请求体（<c>/cgi-bin/idconvert/convert_tmp_external_userid</c>）：
/// 将应用获取的外部用户临时 id（tmp_external_userid）转换为 external_userid 或 userid。
/// </summary>
/// <remarks>
/// 官方限制：tmp_external_userid_list 最多不超过 100 个；调用此接口的应用，和获取到
/// tmp_external_userid 的应用必须是同一个；支持自建应用、代开发自建应用和第三方应用调用；
/// user_type 为 1（客户类型）时应用还需具有「客户联系」权限。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AccountId")]
public class ConvertTmpExternalUserIdRequest
{
    /// <summary>
    /// 获取或设置业务类型（官方必填）：1 - 会议，2 - 收集表，3 - 智能表（文档）。
    /// </summary>
    [JsonPropertyName("business_type")]
    public int? BusinessType { get; set; }

    /// <summary>
    /// 获取或设置转换的目标用户类型（官方必填）：1 - 客户，2 - 企业互联，3 - 上下游，4 - 互联企业（圈子）。
    /// </summary>
    [JsonPropertyName("user_type")]
    public int? UserType { get; set; }

    /// <summary>
    /// 获取或设置外部用户临时 id 列表（官方必填，最多不超过 100 个）。
    /// </summary>
    [JsonPropertyName("tmp_external_userid_list")]
    public List<string>? TmpExternalUserIdList { get; set; }
}
