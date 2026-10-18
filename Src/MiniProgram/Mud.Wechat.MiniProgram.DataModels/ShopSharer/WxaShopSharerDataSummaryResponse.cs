// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.ShopSharer;

/// <summary>
/// 分享员总带货数据应答（<c>POST /shop/sharer/get_sharer_data_summary</c>）。
/// </summary>
/// <remarks>
/// <para><b>金额单位分</b>（官方原文「分享员有效带货额（单位分）」）：<c>gmv</c> 为分 ⇒ 宿主展示前须除 100，
/// 不得与元口径字段混算；本域两支含 <c>gmv</c> 的应答（本类与 <see cref="WxaShopSharerLiveInfo"/>）单位同为一分。</para>
/// <para><b>「有效」口径</b>：官方措辞是<b>有效</b>带货额（剔除退款等无效单），与商家自有统计不同源，不得直接对账。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopSharer")]
public class WxaShopSharerDataSummaryResponse : WxaResponse
{
    /// <summary>分享员有效带货额（<c>gmv</c>，number，<b>单位分</b>）。个人经营数据，禁落日志。</summary>
    [JsonPropertyName("gmv")]
    public long? Gmv { get; set; }

    /// <summary>订单数（<c>order_cnt</c>，number）。</summary>
    [JsonPropertyName("order_cnt")]
    public long? OrderCnt { get; set; }

    /// <summary>下单用户数（<c>user_cnt</c>，number）。</summary>
    [JsonPropertyName("user_cnt")]
    public long? UserCnt { get; set; }
}
