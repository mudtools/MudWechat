// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.CustomerAcquisition;

/// <summary>
/// 查询获客助手剩余使用量响应体（<c>/cgi-bin/externalcontact/customer_acquisition_quota</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "CustomerAcquisition")]
public class GetAcquisitionQuotaResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置历史累计使用量。
    /// </summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>
    /// 获取或设置剩余使用量。
    /// </summary>
    [JsonPropertyName("balance")]
    public long? Balance { get; set; }

    /// <summary>
    /// 获取或设置额度列表（即将过期的额度明细）。
    /// </summary>
    [JsonPropertyName("quota_list")]
    public List<AcquisitionQuotaItem>? QuotaList { get; set; }
}

/// <summary>
/// 获客额度项（查询获客助手剩余使用量响应中 <c>quota_list[]</c> 的元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "CustomerAcquisition")]
public class AcquisitionQuotaItem
{
    /// <summary>
    /// 获取或设置额度过期时间戳（为过期日的零点，实际过期时间取决于额度的购买时间）。
    /// </summary>
    [JsonPropertyName("expire_date")]
    public long? ExpireDate { get; set; }

    /// <summary>
    /// 获取或设置该批即将过期的额度数量。
    /// </summary>
    [JsonPropertyName("balance")]
    public long? Balance { get; set; }
}
