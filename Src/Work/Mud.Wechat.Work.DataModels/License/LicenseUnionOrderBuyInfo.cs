// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 多企业购买信息列表项（<c>buy_list</c> 元素，<c>/cgi-bin/license/get_union_order</c> 响应，官方 BuyInfo）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "License")]
public class LicenseUnionOrderBuyInfo
{
    /// <summary>
    /// 获取或设置子订单 id。
    /// <para>官方口径：可以调用获取订单中的账号列表接口（<c>/cgi-bin/license/list_order_account</c>）以获取账号列表。</para>
    /// </summary>
    [JsonPropertyName("sub_order_id")]
    public string? SubOrderId { get; set; }

    /// <summary>
    /// 获取或设置客户企业 id。
    /// <para>官方口径：返回加密的 corpid。</para>
    /// </summary>
    [JsonPropertyName("corpid")]
    public string? Corpid { get; set; }

    /// <summary>获取或设置订单的账号数详情。</summary>
    [JsonPropertyName("account_count")]
    public LicenseAccountCount? AccountCount { get; set; }

    /// <summary>获取或设置账号购买时长。</summary>
    [JsonPropertyName("account_duration")]
    public LicenseAccountDuration? AccountDuration { get; set; }
}
