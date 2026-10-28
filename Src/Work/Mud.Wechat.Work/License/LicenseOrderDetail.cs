// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 接口许可单企业订单详情（<c>order</c>，<c>/cgi-bin/license/get_order</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "License")]
public class LicenseOrderDetail
{
    /// <summary>获取或设置订单号。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>
    /// 获取或设置订单类型：<c>1</c>-购买账号 / <c>2</c>-续期账号 / <c>5</c>-应用版本付费迁移订单 /
    /// <c>6</c>-历史合同迁移订单。
    /// </summary>
    [JsonPropertyName("order_type")]
    public int? OrderType { get; set; }

    /// <summary>
    /// 获取或设置订单状态：<c>0</c>-待支付 / <c>1</c>-已支付 / <c>2</c>-已取消（未支付，订单已关闭）/
    /// <c>3</c>-未支付，订单已过期 / <c>4</c>-申请退款中 / <c>5</c>-退款成功 / <c>6</c>-退款被拒绝 /
    /// <c>7</c>-订单已失效（将企业从服务商测试企业列表中移除时会将对应测试企业的所有测试订单置为已失效）。
    /// </summary>
    [JsonPropertyName("order_status")]
    public int? OrderStatus { get; set; }

    /// <summary>
    /// 获取或设置客户企业 id。
    /// <para>官方口径：返回加密的 corpid。</para>
    /// </summary>
    [JsonPropertyName("corpid")]
    public string? Corpid { get; set; }

    /// <summary>获取或设置订单金额（单位分）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>获取或设置订单的账号数详情。</summary>
    [JsonPropertyName("account_count")]
    public LicenseAccountCount? AccountCount { get; set; }

    /// <summary>获取或设置账号购买时长。</summary>
    [JsonPropertyName("account_duration")]
    public LicenseAccountDuration? AccountDuration { get; set; }

    /// <summary>获取或设置创建时间（unix 时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>
    /// 获取或设置支付时间（unix 时间戳）。
    /// <para>官方口径：迁移订单不返回该字段。</para>
    /// </summary>
    [JsonPropertyName("pay_time")]
    public long? PayTime { get; set; }
}
