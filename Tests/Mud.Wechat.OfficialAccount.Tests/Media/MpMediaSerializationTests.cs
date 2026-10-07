// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.DataModels.Media;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.Media;

/// <summary>
/// 素材域 DTO 与官方报文的双向映射（用官方页给出的<b>原文示例</b>作为夹具）。
/// </summary>
public class MpMediaSerializationTests
{
    /// <summary>S1：<c>media/upload</c> 官方返回示例解析（官方示例形态：<c>type</c>/<c>media_id</c>/<c>created_at</c>，无 errcode）。</summary>
    [Fact]
    public void UploadTempMediaResponse_ShouldParseOfficialSample()
    {
        const string json = """
            {"type":"image","media_id":"MEDIA_ID","created_at":1380000000}
            """;

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, MediaJsonContext.Default.MpUploadTempMediaResponse);

        response.Should().NotBeNull();
        response!.IsSuccess.Should().BeTrue("成功响应不带 errcode ⇒ 缺省 0");
        response.Type.Should().Be("image");
        response.MediaId.Should().Be("MEDIA_ID");
        response.CreatedAt.Should().Be(1380000000);
    }

    /// <summary>S2：上传响应错误形态（<c>40004 invalid media type</c>）。</summary>
    [Fact]
    public void UploadTempMediaResponse_ShouldCarryErrcodeWhenFailed()
    {
        const string json = """{"errcode":40004,"errmsg":"invalid media type"}""";

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, MediaJsonContext.Default.MpUploadTempMediaResponse);

        response!.IsSuccess.Should().BeFalse();
        response.ErrorCode.Should().Be(40004);
        response.ErrorMessage.Should().Be("invalid media type");
    }

    /// <summary>S3：<c>add_material</c> 官方返回示例解析（图片形态含 url；视频形态 url 为空串）。</summary>
    [Fact]
    public void AddMaterialResponse_ShouldParseOfficialSample()
    {
        const string imageJson = """{"media_id":"MEDIA_ID_654321","url":"https://mmbiz.qpic.cn/xxx"}""";

        var image = System.Text.Json.JsonSerializer.Deserialize(
            imageJson, MediaJsonContext.Default.MpAddMaterialResponse);
        image!.IsSuccess.Should().BeTrue();
        image.MediaId.Should().Be("MEDIA_ID_654321");
        image.Url.Should().Be("https://mmbiz.qpic.cn/xxx");

        // 官方视频示例中 url 为空串 ⇒ 非图片类型不得以 url 有值作判断（SDK 保留原值，由调用方判空）。
        const string videoJson = """{"media_id":"MEDIA_ID_123456","url":""}""";
        var video = System.Text.Json.JsonSerializer.Deserialize(
            videoJson, MediaJsonContext.Default.MpAddMaterialResponse);
        video!.Url.Should().BeEmpty("官方视频示例 url 为空串（照录）");
    }

    /// <summary>S4：<c>get_material</c> 官方图文返回示例解析（news_item 八字段）。</summary>
    [Fact]
    public void PermanentMaterialResponse_ShouldParseNewsShape()
    {
        const string json = """
            {"news_item":[{"title":"TITLE","thumb_media_id":"THUMB_MEDIA_ID","show_cover_pic":1,
            "author":"AUTHOR","digest":"DIGEST","content":"CONTENT","url":"URL","content_source_url":"SOURCE_URL"}]}
            """;

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, MediaJsonContext.Default.MpPermanentMaterialResponse);

        response!.NewsItems.Should().HaveCount(1, "图文形态经 news_item 承载");
        response.DownUrl.Should().BeNull();
        response.NewsItems![0].Title.Should().Be("TITLE");
        response.NewsItems[0].ThumbMediaId.Should().Be("THUMB_MEDIA_ID");
        response.NewsItems[0].ShowCoverPic.Should().Be(1);
        response.NewsItems[0].ContentSourceUrl.Should().Be("SOURCE_URL");
        response.NewsItems[0].ThumbUrl.Should().BeNull("get_material 页无 thumb_url 字段（仅列表页有）");
    }

    /// <summary>S5：<c>get_material</c> 官方视频返回示例解析（title/description/down_url）。</summary>
    [Fact]
    public void PermanentMaterialResponse_ShouldParseVideoShape()
    {
        const string json = """{"title":"TITLE","description":"DESCRIPTION","down_url":"DOWN_URL"}""";

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, MediaJsonContext.Default.MpPermanentMaterialResponse);

        response!.NewsItems.Should().BeNull("视频形态无 news_item");
        response.Title.Should().Be("TITLE");
        response.Description.Should().Be("DESCRIPTION");
        response.DownUrl.Should().Be("DOWN_URL");
    }

    /// <summary>S6：<c>get_materialcount</c> 官方返回示例解析（四类计数）。</summary>
    [Fact]
    public void GetMaterialCountResponse_ShouldParseOfficialSample()
    {
        const string json = """{"voice_count":5,"video_count":3,"image_count":10,"news_count":7}""";

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, MediaJsonContext.Default.MpGetMaterialCountResponse);

        response!.IsSuccess.Should().BeTrue("成功响应不带 errcode ⇒ 缺省 0");
        response.VoiceCount.Should().Be(5);
        response.VideoCount.Should().Be(3);
        response.ImageCount.Should().Be(10);
        response.NewsCount.Should().Be(7);
    }

    /// <summary>S7：<c>batchget_material</c> 官方 news 形态返回示例解析（item.content.news_item）。</summary>
    [Fact]
    public void BatchGetMaterialResponse_ShouldParseNewsShape()
    {
        const string json = """
            {"total_count":100,"item_count":20,"item":[{"media_id":"MEDIA_ID",
            "content":{"news_item":[{"title":"TITLE","thumb_media_id":"THUMB_MEDIA_ID","show_cover_pic":1,
            "author":"AUTHOR","digest":"DIGEST","content":"CONTENT","url":"URL","content_source_url":"SOURCE_URL",
            "thumb_url":"THUMB_URL"}]},"update_time":1620000000}]}
            """;

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, MediaJsonContext.Default.MpBatchGetMaterialResponse);

        response!.TotalCount.Should().Be(100);
        response.ItemCount.Should().Be(20);
        response.Items.Should().HaveCount(1);
        response.Items![0].MediaId.Should().Be("MEDIA_ID");
        response.Items[0].Name.Should().BeNull("news 形态条目无 name");
        response.Items[0].Content.Should().NotBeNull();
        response.Items[0].Content!.NewsItems.Should().HaveCount(1);
        response.Items[0].Content!.NewsItems![0].ThumbUrl.Should().Be("THUMB_URL", "thumb_url 仅列表页出现");
        response.Items[0].UpdateTime.Should().Be(1620000000);
    }

    /// <summary>S8：<c>batchget_material</c> 官方 image 形态返回示例解析（item.name/url）。</summary>
    [Fact]
    public void BatchGetMaterialResponse_ShouldParseImageShape()
    {
        const string json = """
            {"total_count":50,"item_count":10,"item":[{"media_id":"MEDIA_ID","name":"IMAGE.jpg",
            "update_time":1620000000,"url":"http://mmbiz.qpic.cn/xxx"}]}
            """;

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, MediaJsonContext.Default.MpBatchGetMaterialResponse);

        response!.Items.Should().HaveCount(1);
        response.Items![0].Name.Should().Be("IMAGE.jpg");
        response.Items[0].Url.Should().Be("http://mmbiz.qpic.cn/xxx");
        response.Items[0].Content.Should().BeNull("image 形态条目无 content");
    }

    /// <summary>S9：<c>batchget_material</c> 请求体序列化必须为官方形态。</summary>
    [Fact]
    public void BatchGetMaterialRequest_ShouldSerializeToOfficialShape()
    {
        var request = new MpBatchGetMaterialRequest { Type = "news", Offset = 0, Count = 20 };

        using var document = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            request, MediaJsonContext.Default.MpBatchGetMaterialRequest));
        var root = document.RootElement;

        root.EnumerateObject().Select(p => p.Name).Should().BeEquivalentTo(new[] { "type", "offset", "count" });
        root.GetProperty("type").GetString().Should().Be("news");
        root.GetProperty("count").GetInt32().Should().Be(20);
    }

    /// <summary>S10：<c>uploadimg</c> 官方返回示例解析（url 与 errcode/errmsg 并列，照录）。</summary>
    [Fact]
    public void UploadImageResponse_ShouldParseOfficialSample()
    {
        const string json = """{"url":"http://mmbiz.qpic.cn/XXXXX","errcode":0,"errmsg":"ok"}""";

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, MediaJsonContext.Default.MpUploadImageResponse);

        response!.IsSuccess.Should().BeTrue();
        response.Url.Should().Be("http://mmbiz.qpic.cn/XXXXX");
    }

    /// <summary>S11：视频 description 表单值序列化（官方 <c>-F description='{"title":…,"introduction":…}'</c> 形态）。</summary>
    [Fact]
    public void MaterialDescription_ShouldSerializeToOfficialShape()
    {
        var description = new MpMaterialDescription { Title = "TITLE", Introduction = "INTRODUCTION" };

        using var document = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            description, MediaJsonContext.Default.MpMaterialDescription));
        var root = document.RootElement;

        root.EnumerateObject().Select(p => p.Name).Should().BeEquivalentTo(new[] { "title", "introduction" });
        root.GetProperty("title").GetString().Should().Be("TITLE");
    }
}
