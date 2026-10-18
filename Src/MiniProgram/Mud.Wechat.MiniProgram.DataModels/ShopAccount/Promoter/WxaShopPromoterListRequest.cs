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
/// 获取推广员列表请求（<c>POST /shop/promoter/list</c>）。
/// </summary>
/// <remarks>
/// <para><b>翻页口径是「页码 + 页大小」且两支均必填</b>（官方原文：page 最小填 1、page_size 不超过 20），
/// 本域唯一有数字规模的列表端点；无 cursor / next_page 形态 ⇒ 越界页返回空列表而非报错。</para>
/// <para>官方本页小节标题写作「# 请求参数」（其余端点为「# 请求参数说明」），属正文排版差异、不影响契约。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopPromoterListRequest
{
    /// <summary>页码（<c>page</c>，number，<b>必填</b>，官方原文「最小填 1」）。</summary>
    [JsonPropertyName("page")]
    public long? Page { get; set; }

    /// <summary>每页数量（<c>page_size</c>，number，<b>必填</b>，官方原文「不超过 20」）。</summary>
    [JsonPropertyName("page_size")]
    public long? PageSize { get; set; }
}
