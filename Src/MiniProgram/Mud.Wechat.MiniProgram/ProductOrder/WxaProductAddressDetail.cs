// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>
/// 地址详情（add / update 请求体 <c>address_detail</c> 的并集）。
/// </summary>
/// <remarks>
/// <para><b>添加与更新共用</b>：官方两页参数表仅差 <c>address_id</c>（更新必填、添加无此键）⇒</para>
/// <para>本类按可空并集建模，添加时留空即可；<b>更新是整体覆盖</b>，漏传字段即失去原值。</para>
/// <para><b>敏感值</b>：<c>address_info</c> 内姓名 / 电话 / 详细地址禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductOrder")]
public class WxaProductAddressDetail
{
    /// <summary>地址 ID（更新必填；添加不填）。</summary>
    [JsonPropertyName("address_id")]
    public long? AddressId { get; set; }

    /// <summary>座机。</summary>
    [JsonPropertyName("landline")]
    public string? Landline { get; set; }

    /// <summary>备注。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>是否为发货地址（1 是 / 0 否）。</summary>
    [JsonPropertyName("send_addr")]
    public int? SendAddr { get; set; }

    /// <summary>是否为默认发货地址（1 是 / 0 否）。</summary>
    [JsonPropertyName("default_send")]
    public int? DefaultSend { get; set; }

    /// <summary>是否为收货地址（1 是 / 0 否）。</summary>
    [JsonPropertyName("recv_addr")]
    public int? RecvAddr { get; set; }

    /// <summary>是否为默认收货地址（1 是 / 0 否）。</summary>
    [JsonPropertyName("default_recv")]
    public int? DefaultRecv { get; set; }

    /// <summary>地址用途标记。</summary>
    [JsonPropertyName("address_type")]
    public WxaProductAddressType? AddressType { get; set; }

    /// <summary>地址本体（必填）。</summary>
    [JsonPropertyName("address_info")]
    public WxaProductAddressInfo? AddressInfo { get; set; }
}
