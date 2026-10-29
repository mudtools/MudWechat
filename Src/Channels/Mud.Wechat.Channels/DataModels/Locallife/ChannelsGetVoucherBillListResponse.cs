// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Locallife;

/// <summary>查询券账单（<c>getvoucherbilllist</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsGetVoucherBillListResponse : ChannelsResponse
{
    /// <summary>获取或设置用户券账单列表（官方 <c>bill_list</c>）。</summary>
    [JsonPropertyName("bill_list")]
    public List<ChannelsVoucherBill>? BillList { get; set; }

    /// <summary>获取或设置翻页上下文（官方 <c>page_ctx</c>；如果为空，说明没有下一页）。</summary>
    [JsonPropertyName("page_ctx")]
    public string? PageCtx { get; set; }
}
