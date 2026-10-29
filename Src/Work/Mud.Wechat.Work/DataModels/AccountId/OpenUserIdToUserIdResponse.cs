// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.AccountId;

/// <summary>
/// openuserid 转 userid 响应体（<c>/cgi-bin/batch/openuserid_to_userid</c>；
/// 自建应用与第三方/代开发应用对接、自建应用与智能机器人对接两场景共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AccountId")]
public class OpenUserIdToUserIdResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置转换成功的明文 userid 映射列表。
    /// </summary>
    [JsonPropertyName("userid_list")]
    public List<OpenUserIdMapItem>? UserIdList { get; set; }

    /// <summary>
    /// 获取或设置不合法的 open_userid 列表（如果传入的已是明文的 userid，也会在该列表返回）。
    /// </summary>
    [JsonPropertyName("invalid_open_userid_list")]
    public List<string>? InvalidOpenUserIdList { get; set; }
}
