// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 接口许可账号购买时长（<c>account_duration</c>，下单购买账号 / 下单续期账号 / 多企业新购 BuyInfo / 订单详情共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "License")]
public class LicenseAccountDuration
{
    /// <summary>
    /// 获取或设置购买的月数（官方可选），每个月按照 31 天计算。
    /// <para>官方约束：下单购买账号时总购买时长为 (months*31+days) 天，最少购买 1 个月（31 天），
    /// 最多购买 60 个月（1860 天）；提交续期订单时 months 与 <see cref="NewExpireTime"/> 二者填其一。</para>
    /// </summary>
    [JsonPropertyName("months")]
    public int? Months { get; set; }

    /// <summary>
    /// 获取或设置购买的天数（官方可选）。
    /// <para>官方约束：若企业为服务商测试企业，下单购买账号不支持指定天购买。</para>
    /// </summary>
    [JsonPropertyName("days")]
    public int? Days { get; set; }

    /// <summary>
    /// 获取或设置指定的新到期时间戳（官方可选，提交续期订单时与 <see cref="Months"/> 二者填其一）。
    /// <para>官方约束：不可为今天和过去的时间，不可为 1860 天后的时间；须填当天的 24 时 0 分 0 秒，
    /// 否则系统自动处理为当天的 24 时 0 分 0 秒；若企业为服务商测试企业，不支持指定新的到期时间来续期。</para>
    /// </summary>
    [JsonPropertyName("new_expire_time")]
    public long? NewExpireTime { get; set; }
}
