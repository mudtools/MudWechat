// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 接口许可订单列表项（<c>order_list</c> 元素，<c>/cgi-bin/license/list_order</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "License")]
public class LicenseOrderSummary
{
    /// <summary>获取或设置订单号。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>
    /// 获取或设置订单类型：<c>1</c>-购买账号 / <c>2</c>-续期账号 / <c>5</c>-历史企业迁移订单 /
    /// <c>8</c>-多企业新购订单（只返回父订单，且仅当 corpid 不填时返回）。
    /// </summary>
    [JsonPropertyName("order_type")]
    public int? OrderType { get; set; }
}
