// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.DataModels.CustomerMessage;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.CustomerMessage;

/// <summary>客服消息子分组 DTO 映射（各 msgtype 分支与聊天记录）。</summary>
public class MpCustomerMessageSerializationTests
{
    /// <summary>C1：文本消息请求体为「扁平 + msgtype 二选一」形态。</summary>
    [Fact]
    public void TextMessage_ShouldSerializeFlatShape()
    {
        var request = new MpSendCustomMessageRequest
        {
            ToUser = "o1",
            MsgType = MpCustomMessageTypes.Text,
            Text = new MpTextMessage { Content = "你好" },
        };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, CustomerMessageJsonContext.Default.MpSendCustomMessageRequest));
        var root = document.RootElement;

        root.GetProperty("touser").GetString().Should().Be("o1");
        root.GetProperty("msgtype").GetString().Should().Be("text");
        root.GetProperty("text").GetProperty("content").GetString().Should().Be("你好");
        root.TryGetProperty("image", out _).Should().BeFalse("未使用的分支不应序列化（可空引用类型为 null）");
    }

    /// <summary>C2：小程序卡片分支（四字段官方均必填）。</summary>
    [Fact]
    public void MiniProgramPage_ShouldSerializeAllFourFields()
    {
        var request = new MpSendCustomMessageRequest
        {
            ToUser = "o1",
            MsgType = MpCustomMessageTypes.MiniProgramPage,
            MiniProgramPage = new MpMiniProgramPageMessage
            {
                Title = "卡片",
                AppId = "wxappid",
                PagePath = "pages/index/index?foo=bar",
                ThumbMediaId = "MEDIA",
            },
        };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, CustomerMessageJsonContext.Default.MpSendCustomMessageRequest));
        var card = document.RootElement.GetProperty("miniprogrampage");

        card.GetProperty("title").GetString().Should().Be("卡片");
        card.GetProperty("appid").GetString().Should().Be("wxappid");
        card.GetProperty("pagepath").GetString().Should().Be("pages/index/index?foo=bar");
        card.GetProperty("thumb_media_id").GetString().Should().Be("MEDIA");
    }

    /// <summary>C3：菜单消息分支（<c>msgmenu.list</c>）。</summary>
    [Fact]
    public void MsgMenu_ShouldSerializeItemList()
    {
        var request = new MpSendCustomMessageRequest
        {
            ToUser = "o1",
            MsgType = MpCustomMessageTypes.MsgMenu,
            MsgMenu = new MpMsgMenuMessage
            {
                HeadContent = "请选择",
                TailContent = "谢谢",
                List = new() { new MpMsgMenuItem { Id = "101", Content = "选项一" } },
            },
        };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, CustomerMessageJsonContext.Default.MpSendCustomMessageRequest));
        var menu = document.RootElement.GetProperty("msgmenu");

        menu.GetProperty("head_content").GetString().Should().Be("请选择");
        menu.GetProperty("list")[0].GetProperty("id").GetString().Should().Be("101");
        menu.GetProperty("list")[0].GetProperty("content").GetString().Should().Be("选项一");
        menu.GetProperty("tail_content").GetString().Should().Be("谢谢");
    }

    /// <summary>C4：输入状态请求（command 取值 <c>Typing</c> / <c>CancelTyping</c>）。</summary>
    [Fact]
    public void TypingRequest_ShouldSerializeCommand()
    {
        var request = new MpTypingRequest { ToUser = "o1", Command = MpTypingCommands.Typing };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, CustomerMessageJsonContext.Default.MpTypingRequest));

        document.RootElement.GetProperty("touser").GetString().Should().Be("o1");
        document.RootElement.GetProperty("command").GetString().Should().Be("Typing");
    }

    /// <summary>C5：聊天记录请求与响应（<c>opercode</c> / <c>worker</c> 形态）。</summary>
    [Fact]
    public void MsgList_ShouldRoundTrip()
    {
        var request = new MpGetMsgListRequest
        {
            StartTime = 1464710400,
            EndTime = 1464796800,
            MsgId = 1,
            Number = 10000,
        };

        using var requestDoc = JsonDocument.Parse(JsonSerializer.Serialize(
            request, CustomerMessageJsonContext.Default.MpGetMsgListRequest));
        requestDoc.RootElement.GetProperty("starttime").GetInt64().Should().Be(1464710400);
        requestDoc.RootElement.GetProperty("number").GetInt32().Should().Be(10000);

        const string json = """
            {"recordlist":[{"openid":"o1","opercode":2002,"text":"你好","time":1464796800,
              "worker":"kf1@gh_abc"}],"number":1,"msgid":2}
            """;

        var response = JsonSerializer.Deserialize(json, CustomerMessageJsonContext.Default.MpGetMsgListResponse);

        response!.RecordList.Should().HaveCount(1);
        response.RecordList![0].OperCode.Should().Be(MpMsgRecordOperCodes.WorkerSent);
        response.RecordList[0].Worker.Should().Be("kf1@gh_abc", "官方格式为「账号前缀@公众号微信号」");
        response.MsgId.Should().Be(2, "下一页请求回填该游标");
    }

    /// <summary>C6：<c>AddCustomerMessageApi</c> 注册后服务可从 DI 解析。</summary>
    [Fact]
    public void AddCustomerMessageApi_ShouldRegisterClient()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMpApp(config =>
        {
            config.AppKey = "mp-kf";
            config.AppId = "wx-kf";
            config.AppSecret = "s-kf";
        });
        services.AddMpServices(builder => builder.AddCustomerMessageApi());

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IMpCustomerMessageService>().Should().NotBeNull(
            "AddCustomerMessageApi 必须完成 AddCustomerMessageWebApiHttpClient() 的注册");
    }
}
