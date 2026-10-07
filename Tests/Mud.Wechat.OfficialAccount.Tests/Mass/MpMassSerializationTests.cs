// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.DataModels.AutoReply;
using Mud.Wechat.OfficialAccount.DataModels.Mass;
using Mud.Wechat.OfficialAccount.DataModels.Qrcode;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.Mass;

/// <summary>
/// 群发 / 一次性订阅 / 二维码 / 自动回复域 DTO 与官方报文的双向映射（用官方页<b>原文示例</b>作为夹具）。
/// </summary>
public class MpMassSerializationTests
{
    /// <summary>S1：sendall 图文分支（官方示例原文回放）。</summary>
    [Fact]
    public void SendAllRequest_ShouldSerializeMpNewsBranch()
    {
        var request = new MpMassSendAllRequest
        {
            Filter = new MpMassFilter { IsToAll = false, TagId = 2 },
            MsgType = MpMassMsgTypes.MpNews,
            MpNews = new MpMassMediaMessage { MediaId = "123dsdajkasd231jhksad" },
            SendIgnoreReprint = 0,
        };

        using var document = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            request, MassJsonContext.Default.MpMassSendAllRequest));
        var root = document.RootElement;

        root.GetProperty("filter").GetProperty("is_to_all").GetBoolean().Should().BeFalse();
        root.GetProperty("filter").GetProperty("tag_id").GetInt32().Should().Be(2);
        root.GetProperty("mpnews").GetProperty("media_id").GetString().Should().Be("123dsdajkasd231jhksad");
        root.GetProperty("msgtype").GetString().Should().Be("mpnews");
        root.GetProperty("send_ignore_reprint").GetInt32().Should().Be(0);
    }

    /// <summary>S2：mass/send 文本分支（官方示例原文回放）。</summary>
    [Fact]
    public void SendRequest_ShouldSerializeTextBranch()
    {
        var request = new MpMassSendRequest
        {
            ToUserList = new List<string> { "OPENID1", "OPENID2" },
            MsgType = MpMassMsgTypes.Text,
            Text = new MpMassText { Content = "hello from boxer." },
            ClientMsgId = "send_tag_2",
        };

        using var document = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            request, MassJsonContext.Default.MpMassSendRequest));
        var root = document.RootElement;

        root.GetProperty("touser").EnumerateArray().Select(e => e.GetString())
            .Should().Equal(new[] { "OPENID1", "OPENID2" });
        root.GetProperty("text").GetProperty("content").GetString().Should().Be("hello from boxer.");
        root.GetProperty("clientmsgid").GetString().Should().Be("send_tag_2");
    }

    /// <summary>S3：sendall 图片分支（官方 images 多图形态）与预览单图形态互异。</summary>
    [Fact]
    public void ImageBranches_ShouldDifferBetweenMassAndPreview()
    {
        var images = new MpMassImages
        {
            MediaIds = new List<string> { "aaa", "bbb", "ccc" },
            Recommend = "xxx",
            Title = "yyy",
            NeedOpenComment = 1,
            OnlyFansCanComment = 0,
        };
        using var imagesDoc = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            images, MassJsonContext.Default.MpMassImages));
        imagesDoc.RootElement.GetProperty("media_ids").EnumerateArray()
            .Select(e => e.GetString()).Should().Equal(new[] { "aaa", "bbb", "ccc" });
        imagesDoc.RootElement.GetProperty("need_open_comment").GetInt32().Should().Be(1);

        var previewImage = new MpMassMediaMessage { MediaId = "MEDIA_ID" };
        using var previewDoc = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            previewImage, MassJsonContext.Default.MpMassMediaMessage));
        previewDoc.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "media_id" }, "preview 页 image 为单 media_id 形态");
    }

    /// <summary>S4：mass/send 视频分支超集形态（本页含 title/description）。</summary>
    [Fact]
    public void SendRequestVideoBranch_ShouldCarryTitleAndDescription()
    {
        var video = new MpMassVideoMessage { MediaId = "MEDIA_ID", Title = "T", Description = "D" };

        using var document = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            video, MassJsonContext.Default.MpMassVideoMessage));
        document.RootElement.GetProperty("title").GetString().Should().Be("T",
            "mass/send 页 mpvideo 比 sendall 页多 title/description（超集建模）");
    }

    /// <summary>S5：群发提交结果（官方响应示例；msg_data_id 仅图文群发）。</summary>
    [Fact]
    public void MassSendResponse_ShouldParseOfficialSample()
    {
        const string json = """{"errcode":0,"errmsg":"send job submission success","msg_id":34182,"msg_data_id":206227730}""";

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, MassJsonContext.Default.MpMassSendResponse);

        response!.IsSuccess.Should().BeTrue();
        response.MsgId.Should().Be(34182);
        response.MsgDataId.Should().Be(206227730);
    }

    /// <summary>S6：查询群发状态（官方响应示例；仅 msg_id/msg_status 两字段）。</summary>
    [Fact]
    public void MassStatusResponse_ShouldParseOfficialSample()
    {
        const string json = """{"msg_id":201053012,"msg_status":"SEND_SUCCESS"}""";

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, MassJsonContext.Default.MpMassStatusResponse);

        response!.MsgId.Should().Be(201053012, "官方示例 msg_id 为数字形态（响应表标 string 属矛盾，按示例建模）");
        response.MsgStatus.Should().Be("SEND_SUCCESS");
    }

    /// <summary>S7：群发速度档位与删除请求官方形态。</summary>
    [Fact]
    public void SpeedAndDeleteRequests_ShouldSerializeOfficialShape()
    {
        var speed = new MpMassSpeedSetRequest { Speed = 1 };
        using var speedDoc = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            speed, MassJsonContext.Default.MpMassSpeedSetRequest));
        speedDoc.RootElement.GetProperty("speed").GetInt32().Should().Be(1);

        var speedResp = System.Text.Json.JsonSerializer.Deserialize(
            """{"speed":3,"realspeed":15}""", MassJsonContext.Default.MpMassSpeedResponse);
        speedResp!.Speed.Should().Be(3);
        speedResp.RealSpeed.Should().Be(15, "官方示例 realspeed=15（与对照表 30 不一致——照录矛盾）");

        var delete = new MpMassDeleteRequest { MsgId = 30124, ArticleIdx = 2 };
        using var deleteDoc = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            delete, MassJsonContext.Default.MpMassDeleteRequest));
        deleteDoc.RootElement.GetProperty("msg_id").GetInt64().Should().Be(30124);
        deleteDoc.RootElement.GetProperty("article_idx").GetInt32().Should().Be(2);
    }

    /// <summary>S8：一次性订阅请求（官方示例原文回放；data.content 有 color）。</summary>
    [Fact]
    public void OneTimeSubscribeRequest_ShouldSerializeOfficialShape()
    {
        var request = new MpOneTimeSubscribeRequest
        {
            ToUser = "OPENID",
            TemplateId = "TEMPLATE_ID",
            MiniProgram = new Mud.Wechat.OfficialAccount.DataModels.Template.MpTemplateMiniProgram
            {
                AppId = "xiaochengxuappid12345",
                PagePath = "index?foo=bar",
            },
            Scene = "1000",
            Title = "订阅通知",
            Data = new MpOneTimeSubscribeData
            {
                Content = new MpOneTimeSubscribeContent { Value = "您好！", Color = "#FF0000" },
            },
        };

        using var document = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            request, MassJsonContext.Default.MpOneTimeSubscribeRequest));
        var root = document.RootElement;

        root.GetProperty("scene").GetString().Should().Be("1000", "官方示例 scene 为字符串形态");
        root.GetProperty("data").GetProperty("content").GetProperty("value").GetString().Should().Be("您好！");
        root.GetProperty("data").GetProperty("content").GetProperty("color").GetString().Should().Be("#FF0000",
            "一次性订阅的 data.content 有 color 字段（与模板消息 send 的 value-only 不同）");
    }

    /// <summary>S9：带参二维码请求（官方 QR_SCENE 示例原文回放）与响应解析。</summary>
    [Fact]
    public void QrcodeCreate_ShouldRoundTripOfficialShape()
    {
        var request = new MpQrcodeCreateRequest
        {
            ExpireSeconds = 604800,
            ActionName = MpQrcodeActionNames.QrScene,
            ActionInfo = new MpQrcodeActionInfo
            {
                Scene = new MpQrcodeScene { SceneId = 123 },
            },
        };

        using var document = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            request, QrcodeJsonContext.Default.MpQrcodeCreateRequest));
        document.RootElement.GetProperty("expire_seconds").GetInt32().Should().Be(604800);
        document.RootElement.GetProperty("action_info").GetProperty("scene").GetProperty("scene_id").GetInt32().Should().Be(123);

        const string responseJson = """
            {"ticket":"gQH47joAAAAAAAAAASxodHRwOi8vd2VpeGlu...","expire_seconds":60,"url":"http://weixin.qq.com/q/kZgfwMTm72WWPkovabbI"}
            """;
        var response = System.Text.Json.JsonSerializer.Deserialize(
            responseJson, QrcodeJsonContext.Default.MpQrcodeCreateResponse);
        response!.IsSuccess.Should().BeTrue();
        response.Ticket.Should().NotBeNullOrEmpty();
        response.Url.Should().Be("http://weixin.qq.com/q/kZgfwMTm72WWPkovabbI");
    }

    /// <summary>S10：自动回复官方形态回放（关键词规则 + 图文回复）。</summary>
    [Fact]
    public void AutoReplyInfoResponse_ShouldParseOfficialShape()
    {
        const string json = """
            {"is_add_friend_reply_open":1,"is_autoreply_open":0,
            "add_friend_autoreply_info":{"type":"text","content":"欢迎关注"},
            "message_default_autoreply_info":{"type":"img","content":"MEDIA_ID"},
            "keyword_autoreply_info":{"list":[{"rule_name":" textual","create_time":1480392321,
            "reply_mode":"reply_all",
            "keyword_list_info":[{"type":"text","content":"关键字","match_mode":"contain"}],
            "reply_list_info":[{"type":"news","content":"MEDIA_ID",
            "news_info":{"list":[{"title":"TITLE","digest":"DIGEST","author":"AUTHOR",
            "show_cover":1,"cover_url":"COVER","content_url":"CONTENT","source_url":"SOURCE"}]}}]}]}}
            """;

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, AutoReplyJsonContext.Default.MpAutoReplyInfoResponse);

        response!.IsAddFriendReplyOpen.Should().Be(1);
        response.IsAutoReplyOpen.Should().Be(0);
        response.AddFriendAutoReplyInfo!.Type.Should().Be("text");
        response.MessageDefaultAutoReplyInfo!.Content.Should().Be("MEDIA_ID");
        response.KeywordAutoReplyInfo!.List.Should().HaveCount(1);
        var rule = response.KeywordAutoReplyInfo.List[0];
        rule.ReplyMode.Should().Be("reply_all");
        rule.KeywordListInfo![0].MatchMode.Should().Be("contain");
        rule.ReplyListInfo![0].NewsInfo!.List[0].SourceUrl.Should().Be("SOURCE",
            "官方原文「若置空则无查看原文入口」");
    }
}
