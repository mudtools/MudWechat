// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.ShopAccount;

/// <summary>
/// 获取推广员列表应答（<c>POST /shop/promoter/list</c>）。
/// </summary>
/// <remarks>
/// <para><b>回包为顶层平铺（<c>promoters</c> / <c>total_num</c>），既无 <c>data</c> 也无分页游标</b>
/// ⇒ 翻页只能靠 <c>page</c> 递增与 <c>total_num</c> 比较，守卫 SHOPA5 锁定该形态。</para>
/// <para><b>官方回包示例不含 <c>errmsg</c></b>（只有 <c>errcode</c> / <c>total_num</c> / <c>promoters</c>）
/// ⇒ 回包表虽列了 errmsg，实际可能缺省；<see cref="WxaResponse.ErrorMessage"/> 建为可空已覆盖。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopPromoterListResponse : WxaResponse
{
    /// <summary>推广员总数（<c>total_num</c>，number；官方示例 3）。</summary>
    [JsonPropertyName("total_num")]
    public long? TotalNum { get; set; }

    /// <summary>推广员信息列表（<c>promoters</c>，官方回包表类型列写作 <c>Promoter[]</c>）。</summary>
    [JsonPropertyName("promoters")]
    public List<WxaShopPromoter>? Promoters { get; set; }
}
