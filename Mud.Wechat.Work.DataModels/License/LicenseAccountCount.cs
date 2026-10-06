// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 接口许可账号个数详情（<c>account_count</c>，下单购买账号 / 多企业新购 BuyInfo / 订单详情共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "License")]
public class LicenseAccountCount
{
    /// <summary>
    /// 获取或设置基础账号个数（官方可选）。
    /// <para>官方约束：最多 1000000 个；若企业为服务商测试企业，最多购买 1000 个。</para>
    /// </summary>
    [JsonPropertyName("base_count")]
    public int? BaseCount { get; set; }

    /// <summary>
    /// 获取或设置互通账号个数（官方可选）。
    /// <para>官方约束：最多 1000000 个；若企业为服务商测试企业，最多购买 1000 个。</para>
    /// </summary>
    [JsonPropertyName("external_contact_count")]
    public int? ExternalContactCount { get; set; }
}
