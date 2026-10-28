// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.AcquisitionComponent;

/// <summary>
/// 获客助手组件获取代支付流水请求体（<c>/cgi-bin/service/customer_acquisition/get_bill_list</c>）。
/// </summary>
/// <remarks>
/// 官方口径：流水起止间隔不能超过 31 天；企业通过代付产生使用量后，次日可在获客助手中查看代付的订单记录，
/// 若需与企业侧使用量统计周期一致，请按当天的 0 时 0 分 01 秒到第二天的 0 时 0 分 0 秒的时间戳查询。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AcquisitionComponent")]
public class GetAcquisitionBillListRequest
{
    /// <summary>
    /// 获取或设置流水记录开始时间（Unix 秒级时间戳，官方必填）。
    /// </summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>
    /// 获取或设置流水记录结束时间（Unix 秒级时间戳，官方必填）。
    /// </summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>
    /// 获取或设置授权企业 corpid（官方必填）。
    /// </summary>
    [JsonPropertyName("auth_corpid")]
    public string? AuthCorpid { get; set; }

    /// <summary>
    /// 获取或设置用于分页查询的游标，由上一次调用返回，首次调用可不填。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置返回的最大记录数，默认值 100，最大不超过 1000。
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
