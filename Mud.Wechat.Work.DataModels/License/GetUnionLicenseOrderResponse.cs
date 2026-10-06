// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 获取多企业订单详情响应体（<c>/cgi-bin/license/get_union_order</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "License")]
public class GetUnionLicenseOrderResponse : WechatWorkResponse
{
    /// <summary>获取或设置订单详情。</summary>
    [JsonPropertyName("order")]
    public LicenseUnionOrderDetail? Order { get; set; }

    /// <summary>获取或设置是否有更多：<c>0</c>-没有，<c>1</c>-有。</summary>
    [JsonPropertyName("has_more")]
    public int? HasMore { get; set; }

    /// <summary>获取或设置分页游标，下次请求时填写到 cursor 以获取之后分页的记录。</summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    /// <summary>获取或设置多企业购买信息列表。</summary>
    [JsonPropertyName("buy_list")]
    public List<LicenseUnionOrderBuyInfo>? BuyList { get; set; }
}
