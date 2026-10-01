// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.JobInheritance;

/// <summary>
/// 分配在职成员的客户响应中单个客户的分配结果（<c>customer[]</c> 元素）。
/// </summary>
public class TransferCustomerResultItem
{
    /// <summary>
    /// 获取或设置客户的 external_userid。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserid { get; set; }

    /// <summary>
    /// 获取或设置对该客户分配的结果（参考全局错误码；0 表示成功发起接替，
    /// 待 24 小时后自动接替，并不代表最终接替成功）。
    /// </summary>
    [JsonPropertyName("errcode")]
    public int? ErrorCode { get; set; }
}
