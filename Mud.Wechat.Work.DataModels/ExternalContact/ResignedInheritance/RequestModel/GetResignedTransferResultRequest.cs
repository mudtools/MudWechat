// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.ResignedInheritance;

/// <summary>
/// 查询客户接替状态请求体（<c>/cgi-bin/externalcontact/resigned/transfer_result</c>）。
/// <para>分页查询离职成员客户分配情况，每页返回不超过 1000 条。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ResignedInheritance")]
public class GetResignedTransferResultRequest
{
    /// <summary>
    /// 获取或设置原添加成员的 userid（官方必填）。
    /// </summary>
    [JsonPropertyName("handover_userid")]
    public string? HandoverUserid { get; set; }

    /// <summary>
    /// 获取或设置接替成员的 userid（官方必填）。
    /// </summary>
    [JsonPropertyName("takeover_userid")]
    public string? TakeoverUserid { get; set; }

    /// <summary>
    /// 获取或设置分页查询游标（不填或为空表示获取第一个分页）。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }
}
