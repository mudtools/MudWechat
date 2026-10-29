// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.ArticleDemo.Services;
using Mud.Wechat.OfficialAccount.DataModels.Media;

namespace Mud.Wechat.OfficialAccount.ArticleDemo.Endpoints;

/// <summary>
/// 永久图片素材读取端点（方案 v1.2 §5.1）：草稿的 <c>thumb_media_id</c> 与正文插图都取自这里。
/// <b>不做上传</b> —— 上传面在 Mud.HttpUtils 3.0.9 的 <c>[MultipartForm] IFormContent</c> 形态下
/// 不发射文件体（方案 §16），故素材需预先在公众平台官网上传，本 Demo 只列不写。
/// </summary>
public static class MaterialEndpoints
{
    private const string BatchGetPath = "/cgi-bin/material/batchget_material";
    private const int PageMax = 20;

    public static WebApplication MapMaterialEndpoints(this WebApplication app)
    {
        app.MapGet("/api/articles/materials", async (
            IMpMediaService media,
            int offset = 0,
            int count = 20,
            CancellationToken cancellationToken = default) =>
        {
            // MpMediaTypes.News 是查询/删除专用值，不可作为上传类型；这里固定取图片素材。
            var response = await media.BatchGetMaterialAsync(new MpBatchGetMaterialRequest
            {
                Type = MpMediaTypes.Image,
                Offset = Math.Max(0, offset),
                Count = ApiOutcome.Clamp(count, 1, PageMax),
            }, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccess)
            {
                return ApiOutcome.BusinessFailure(response, BatchGetPath);
            }

            return Results.Json(new
            {
                items = MaterialProjection.FromItems(response.Items),
                totalCount = response.TotalCount,
                itemCount = response.ItemCount,

                // 官方永久图片素材上限 10 万；本 Demo 只列不做配额管理（方案 §15.5）。
                hint = "素材需预先在公众平台官网「素材库 - 图片」上传；本 Demo 无上传端点（见 README 的已知边界）",
            });
        });

        return app;
    }
}
