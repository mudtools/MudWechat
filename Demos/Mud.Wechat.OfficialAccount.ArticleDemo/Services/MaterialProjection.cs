// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.DataModels.Media;

namespace Mud.Wechat.OfficialAccount.ArticleDemo.Services;

/// <summary>
/// 永久图片素材条目（方案 v1.2 §5.1）：封面取 <c>mediaId</c>、正文插图取 <c>url</c>。
/// <b>本 Demo 无上传端点</b> —— 上传面（<c>AddMaterialAsync</c> / <c>UploadImageAsync</c>）在
/// Mud.HttpUtils 3.0.9 的 <c>[MultipartForm] IFormContent</c> 形态下不发射文件体，详见方案 §16。
/// </summary>
public sealed record MaterialSummary(
    string MediaId,
    string? Name,
    string? Url,
    long UpdateTime);

/// <summary>SDK 素材条目 → 前端投影。</summary>
public static class MaterialProjection
{
    public static MaterialSummary FromItem(MpMaterialListItem item)
        => new(
            item.MediaId ?? string.Empty,
            item.Name,
            item.Url,
            item.UpdateTime);

    public static IReadOnlyList<MaterialSummary> FromItems(IReadOnlyList<MpMaterialListItem>? items)
    {
        if (items == null)
        {
            return Array.Empty<MaterialSummary>();
        }

        var views = new List<MaterialSummary>(items.Count);
        foreach (var item in items)
        {
            views.Add(FromItem(item));
        }

        return views;
    }
}
