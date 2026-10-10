// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 获取「客户数据统计」接待人员明细数据请求体（<c>/cgi-bin/kf/get_servicer_statistic</c>）。
/// <para>
/// 查询区间为闭区间且最大跨度 31 天，最多获取最近 180 天数据；
/// 传入非 0 点时间戳会被官方向下取整到当天 0 点；当天数据次日才可获取（建议次日早上六点后调用）。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class GetKfServicerStatisticRequest
{
    /// <summary>
    /// 获取或设置客服账号 ID（官方必填）。
    /// </summary>
    [JsonPropertyName("open_kfid")]
    public string? OpenKfId { get; set; }

    /// <summary>
    /// 获取或设置接待人员 userid（第三方 / 代开发应用填密文 userid 即 open_userid）。
    /// <para>
    /// 不指定则返回客服账号维度的汇总数据（并非全体接待人员的明细）。
    /// </para>
    /// </summary>
    [JsonPropertyName("servicer_userid")]
    public string? ServicerUserId { get; set; }

    /// <summary>
    /// 获取或设置起始日期的时间戳（官方必填，须填当天 0 时 0 分 0 秒；
    /// 取值范围为「昨天至前 180 天」）。
    /// </summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>
    /// 获取或设置结束日期的时间戳（官方必填，须填当天 0 时 0 分 0 秒；
    /// 取值范围为「昨天至前 180 天」）。
    /// </summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }
}
