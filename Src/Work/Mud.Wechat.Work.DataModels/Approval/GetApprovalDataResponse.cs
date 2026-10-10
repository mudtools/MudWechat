// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 获取审批数据（旧）响应体（<c>/cgi-bin/corp/getapprovaldata</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class GetApprovalDataResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置拉取的审批单个数，最大值为 100；当 total 参数大于 100 时，可运用 next_spnum 参数进行多次拉取。
    /// </summary>
    [JsonPropertyName("count")]
    public int? Count { get; set; }

    /// <summary>
    /// 获取或设置时间段内的总审批单个数。
    /// </summary>
    [JsonPropertyName("total")]
    public int? Total { get; set; }

    /// <summary>
    /// 获取或设置拉取列表的最后一个审批单号（作为下一次调用的 next_spnum 进行分页拉取）。
    /// </summary>
    [JsonPropertyName("next_spnum")]
    public long? NextSpnum { get; set; }

    /// <summary>
    /// 获取或设置审批记录列表。
    /// </summary>
    [JsonPropertyName("data")]
    public List<GetApprovalDataItem>? Data { get; set; }
}
