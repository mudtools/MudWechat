// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 售后管理（Aftersale）域「售后单主文档」DTO（aftersale/getaftersaleorder、aftersale/getaftersalelist、
// aftersale/reason/get、aftersale/rejectreason/get）+ 售后单状态 / 原因 / 类型常量。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Aftersale。

namespace Mud.Wechat.Channels.DataModels.Aftersale;

/// <summary>本地生活券退款明细（官方 <c>product_info.voucher_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsAftersaleVoucher
{
    /// <summary>获取或设置本地券券码（官方 <c>vourcher_code</c>，券级别 key）。</summary>
    [JsonPropertyName("vourcher_code")]
    public string? VourcherCode { get; set; }

    /// <summary>获取或设置券退款金额（官方 <c>amount</c>，单位分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>获取或设置次卡次数明细（官方 <c>times_list</c>，仅次卡券有值，普通券为空）。</summary>
    [JsonPropertyName("times_list")]
    public List<ChannelsAftersaleVoucherTimes>? TimesList { get; set; }
}
