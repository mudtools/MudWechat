// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.AcquisitionComponent;

/// <summary>
/// 获客助手组件获取代支付流水响应体（<c>/cgi-bin/service/customer_acquisition/get_bill_list</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 本端点官方契约以 <c>suite_access_token</c>（获客助手组件的应用凭证）鉴权，与其余组件端点的企业级
/// <c>access_token</c> 分属两个令牌路由键（接口族拆分依据）。
/// </para>
/// <para>
/// 企业通过代付产生使用量后，次日可在获客助手中查看代付的订单记录；若需与企业侧使用量统计周期一致，
/// 请按当天的 0 时 0 分 01 秒到第二天的 0 时 0 分 0 秒的时间戳查询。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AcquisitionComponent")]
public class GetAcquisitionBillListResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置分页游标，在下次请求时填写以获取之后分页的记录；已无更多数据则不返回该字段。
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    /// <summary>
    /// 获取或设置代支付流水记录列表。
    /// </summary>
    [JsonPropertyName("bill_list")]
    public List<AcquisitionBillRecord>? BillList { get; set; }
}

/// <summary>
/// 获客助手组件代支付流水记录。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AcquisitionComponent")]
public class AcquisitionBillRecord
{
    /// <summary>
    /// 获取或设置消耗时间（Unix 秒级时间戳）。
    /// </summary>
    [JsonPropertyName("timestamp")]
    public long? Timestamp { get; set; }

    /// <summary>
    /// 获取或设置获客链接 id（获客链接已删除的情况不返回此字段）。
    /// </summary>
    [JsonPropertyName("link_id")]
    public string? LinkId { get; set; }

    /// <summary>
    /// 获取或设置加好友时的 state 参数（为空则不返回）。
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>
    /// 获取或设置服务商代付金额，单位分。
    /// </summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }
}
