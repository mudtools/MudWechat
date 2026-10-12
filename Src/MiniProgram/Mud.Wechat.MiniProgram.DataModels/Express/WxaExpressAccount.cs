// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>快递账号信息（<c>account/getall</c> 的 <c>list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressAccount
{
    /// <summary>快递公司客户编码（<c>biz_id</c>）。</summary>
    [JsonPropertyName("biz_id")]
    public string? BusinessId { get; set; }

    /// <summary>快递公司 ID（<c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>账号状态（<c>status_code</c>；0 未绑定，1 绑定中，2 已绑定，3 绑定失败）。</summary>
    [JsonPropertyName("status_code")]
    public int StatusCode { get; set; }

    /// <summary>账号别名（<c>alias</c>）。</summary>
    [JsonPropertyName("alias")]
    public string? Alias { get; set; }

    /// <summary>绑定失败原因（<c>remark_wrong_msg</c>）。</summary>
    [JsonPropertyName("remark_wrong_msg")]
    public string? RemarkWrongMessage { get; set; }

    /// <summary>绑定备注（<c>remark_content</c>）。</summary>
    [JsonPropertyName("remark_content")]
    public string? RemarkContent { get; set; }

    /// <summary>账号单量配额（<c>quota_num</c>；-1 表示不限制）。</summary>
    [JsonPropertyName("quota_num")]
    public int? QuotaNumber { get; set; }

    /// <summary>配额更新时间（<c>quota_update_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("quota_update_time")]
    public long? QuotaUpdateTime { get; set; }

    /// <summary>服务类型列表（<c>service_type</c>），见 <see cref="WxaExpressService"/>。</summary>
    [JsonPropertyName("service_type")]
    public List<WxaExpressService>? ServiceList { get; set; }

    /// <summary>账号更新时间（<c>update_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long UpdateTime { get; set; }

    /// <summary>账号创建时间（<c>create_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long CreateTime { get; set; }
}
