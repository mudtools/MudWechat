// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Nodes;
using Mud.AI.Abstractions.Tools.Handlers;
using Mud.AI.Agent.Tools;
using Mud.Wechat.AI.Agent.Results;
using Mud.Wechat.Work.AI.Tools.Curation;
using Mud.Wechat.Work.DataModels.ExternalContact.ProductAlbum;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.AI.Tools.Executors;

/// <summary>
/// 商品图册域只读执行器：商品图册列表 → 商品图册详情。
/// </summary>
/// <remarks>
/// <para>
/// <b>投影边界</b>：商品编码 / 描述 / 价格（分）/ 创建时间 / 图片张数进模型可见面；
/// <c>media_id</c>（商品图片，宿主素材资产）不投影——模型无法消费二进制，与朋友圈、群发附件同一条边界。
/// </para>
/// </remarks>
/// <param name="productAlbum">商品图册域 SDK。</param>
public sealed class ExternalContactProductAlbumToolHandlers(IWechatWorkExternalContactProductAlbumService productAlbum)
{
    /// <summary>官方单页上限。</summary>
    private const int MaxPageSize = 100;

    /// <summary>官方默认单页条数。</summary>
    private const int DefaultPageSize = 50;

    private readonly IWechatWorkExternalContactProductAlbumService _productAlbum =
        productAlbum ?? throw new ArgumentNullException(nameof(productAlbum));

    /// <summary>执行 <c>externalcontact.product_album.list</c>。</summary>
    /// <param name="args">模型侧入参。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkExternalContactProductAlbumListTool))]
    public async Task<ToolResult> ListAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ExternalcontactProductAlbumListArgs.Unpack(args);

        var limit = request.Limit ?? DefaultPageSize;
        if (limit is < 1 or > MaxPageSize)
        {
            return ToolResult.FromError($"参数非法：limit 必须在 1~{MaxPageSize} 之间。");
        }

        var response = await _productAlbum.GetProductAlbumListAsync(
            new GetProductAlbumListRequest { Limit = limit, Cursor = request.Cursor },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "拉取商品图册失败。");
        }

        var products = new JsonArray();
        foreach (var item in outcome.Data?.ProductList ?? [])
        {
            products.Add((JsonNode?)ToProductNode(item));
        }

        // 空列表是合法答案（企业还没有配置商品图册），不转错误。
        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["product_list"] = products,
            ["count"] = products.Count,
            ["next_cursor"] = outcome.Data?.NextCursor,
        }));
    }

    /// <summary>执行 <c>externalcontact.product_album.get</c>。</summary>
    /// <param name="args">模型侧入参（<c>product_id</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkExternalContactProductAlbumGetTool))]
    public async Task<ToolResult> GetAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ExternalcontactProductAlbumGetArgs.Unpack(args);

        var response = await _productAlbum.GetProductAlbumAsync(
            new GetProductAlbumRequest { ProductId = request.ProductId },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok || outcome.Data?.Product is not { } product)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "商品不存在或无权访问（product 为空）。");
        }

        return ToolResult.FromText(ToolResultJson.ToText(ToProductNode(product)));
    }

    /// <summary>商品投影：业务事实 + 图片张数；<c>media_id</c> 不外发。</summary>
    private static JsonNode ToProductNode(ProductAlbumInfo product)
        => new JsonObject
        {
            ["product_id"] = product.ProductId,
            ["product_sn"] = product.ProductSn,
            ["description"] = product.Description,
            ["price"] = JsonValue.Create(product.Price),
            ["create_time"] = JsonValue.Create(product.CreateTime),
            ["image_count"] = product.Attachments?.Count ?? 0,
        };
}
