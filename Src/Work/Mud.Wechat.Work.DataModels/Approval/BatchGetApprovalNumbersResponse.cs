// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 批量获取审批单号响应体（<c>/cgi-bin/oa/getapprovalinfo</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class BatchGetApprovalNumbersResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置审批单号列表，包含满足条件的审批申请。
    /// </summary>
    [JsonPropertyName("sp_no_list")]
    public List<string>? SpNoList { get; set; }

    /// <summary>
    /// 获取或设置后续请求查询的游标；当返回结果没有该字段时表示审批单已经拉取完。
    /// </summary>
    [JsonPropertyName("new_next_cursor")]
    public string? NewNextCursor { get; set; }
}
