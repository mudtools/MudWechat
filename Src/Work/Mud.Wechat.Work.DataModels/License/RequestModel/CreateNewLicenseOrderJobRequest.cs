// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 创建多企业新购任务请求体（<c>/cgi-bin/license/create_new_order_job</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "License")]
public class CreateNewLicenseOrderJobRequest
{
    /// <summary>
    /// 获取或设置企业新购信息列表（官方必填）。
    /// <para>官方约束：每次最多传 10 个，每个 jobid 最多关联 100000 个 BuyInfo；
    /// 测试企业不支持多企业下单方式。</para>
    /// </summary>
    [JsonPropertyName("buy_list")]
    public List<LicenseBuyInfo> BuyList { get; set; } = new List<LicenseBuyInfo>();

    /// <summary>
    /// 获取或设置多企业新购任务 id（官方可选）。
    /// <para>官方口径：不传默认创建一个新任务；有传必须为第一次调用后返回的 jobid，
    /// 可以通过该接口将该任务关联多个新企业的购买账号信息。</para>
    /// </summary>
    [JsonPropertyName("jobid")]
    public string? Jobid { get; set; }
}
