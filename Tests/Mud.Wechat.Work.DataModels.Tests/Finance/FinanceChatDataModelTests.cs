// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;
using Mud.Wechat.Work.DataModels.Finance;

namespace Mud.Wechat.Work.DataModels.Tests.Finance;

/// <summary>
/// 会话内容存档 DTO 的键名与形态测试：官方键逐字锁定，全树样本经 <c>FinanceJsonContext</c> 反序列化。
/// </summary>
/// <remarks>
/// <para>
/// <b>锁的是形态而非语义</b>：官方页 <c>path/91774</c> 的逐字段原文仍<b>待逐页核验</b>，故本文件不断言
/// 「某字段等于某业务含义」，只断言「CLR 属性绑的是哪一个官方键」—— 键名由原生库写回的 JSON 决定，
/// 一旦驼峰化（<c>publickey_ver</c> → <c>publicKeyVer</c>）就是<b>静默丢字段</b>，属最难归因的一类错误。
/// </para>
/// <para>
/// <b>走源生成上下文而非反射版</b>：生产路径是 <c>FinanceJsonContext</c>（AOT 净零），反射版会掩盖
/// 「嵌套类型未被上下文传递覆盖」这类只在源生成路径上暴露的问题。
/// </para>
/// </remarks>
public class FinanceChatDataModelTests
{
    /// <summary>官方消息头键集（10 支）。</summary>
    private static readonly string[] HeaderKeys =
    {
        "msgid", "action", "from", "tolist", "user", "roomid", "msgtime", "msgtype", "voiceid", "voipid",
    };

    /// <summary>官方正文键集（26 支，与 msgtype 同名）。</summary>
    private static readonly string[] BodyKeys =
    {
        "text", "image", "revoke", "agree", "voice", "video", "card", "location", "emotion", "file",
        "link", "weapp", "chatrecord", "todo", "vote", "collect", "redpacket", "external_redpacket",
        "meeting", "doc", "info", "calendar", "mixed", "meeting_voice_call", "voip_doc_share", "sphfeed",
    };

    [Fact]
    public void FinanceChatMessage_ShouldBindOfficialKeyNamesVerbatim()
    {
        var keys = JsonKeysOf(typeof(FinanceChatMessage));

        keys.Should().Equal(Sort(HeaderKeys.Concat(BodyKeys)),
            "消息头与 26 支正文键必须逐字照抄官方（驼峰化或美化改名会让字段静默丢失）");
        keys.Should().HaveCount(HeaderKeys.Length + BodyKeys.Length,
            "一个类型覆盖全部 msgtype ⇒ 少一支键就等于少一种正文形态");
    }

    [Fact]
    public void FinanceChatDataRow_ShouldBindOfficialKeyNamesVerbatim()
    {
        JsonKeysOf(typeof(FinanceChatDataRow)).Should()
            .Equal(Sort(new[] { "seq", "msgid", "publickey_ver", "encrypt_random_key", "encrypt_chat_msg" }),
                "publickey_ver 是私钥映射的键、两段 encrypt_* 是解密的必要输入，改名即断链");
    }

    [Fact]
    public void FinanceChatDataEnvelope_ShouldDeclareOnlyChatData()
    {
        // errcode/errmsg 来自基类 WechatWorkResponse（判错走同一咽喉点），本类型只声明 chatdata。
        JsonKeysOf(typeof(FinanceChatDataEnvelope)).Should().Equal(new[] { "chatdata" },
            "信封不得另立判错字段，否则与企微统一信封形状漂移");
    }

    /// <summary>官方键全为小写下划线形态：出现大写字母即说明有人把键名「修正」成了驼峰。</summary>
    [Fact]
    public void FinanceJsonKeys_ShouldStayLowerCaseWithUnderscore()
    {
        var types = FinanceDtoTypes();
        types.Should().NotBeEmpty();

        foreach (var type in types)
        {
            foreach (var key in JsonKeysOf(type))
            {
                key.Should().MatchRegex("^[a-z][a-z0-9_]*$",
                    $"{type.Name} 的官方键 {key} 形态异常（官方键名无大写字母）");
            }
        }
    }

    /// <summary>
    /// 「文件大小」在官方各正文页键名并不统一（<c>voice_size</c> / <c>imagesize</c> / <c>filesize</c> 三形并存），
    /// 会议 ID 更是 <c>meetingid</c> 与 <c>meeting_id</c> 两键 —— 统一成一个键名即破坏契约。
    /// </summary>
    [Fact]
    public void SizeAndMeetingIdKeys_ShouldNotBeUnified()
    {
        JsonNameOf<FinanceVoiceMessageContent>(nameof(FinanceVoiceMessageContent.FileSize))
            .Should().Be("voice_size");
        JsonNameOf<FinanceEmotionMessageContent>(nameof(FinanceEmotionMessageContent.FileSize))
            .Should().Be("imagesize");
        JsonNameOf<FinanceImageMessageContent>(nameof(FinanceImageMessageContent.FileSize))
            .Should().Be("filesize");

        JsonNameOf<FinanceMeetingMessageContent>(nameof(FinanceMeetingMessageContent.MeetingId))
            .Should().Be("meetingid");
        JsonNameOf<FinanceInfoMessageContent>(nameof(FinanceInfoMessageContent.MeetingId))
            .Should().Be("meeting_id");
    }

    /// <summary>官方 <c>seq</c> 为 uint64：取 <c>long</c> 会在高位区间溢出，而游标恰是长期递增的量。</summary>
    [Fact]
    public void Seq_ShouldBeUInt64_AndSurviveBeyondInt64Max()
    {
        typeof(FinanceChatDataRow).GetProperty(nameof(FinanceChatDataRow.Seq))!.PropertyType
            .Should().Be<ulong>("官方 seq 为 uint64");

        var json = """{"seq":18446744073709551615,"msgid":"m-1","publickey_ver":3}""";
        var row = JsonSerializer.Deserialize(json, FinanceJsonContext.Default.FinanceChatDataRow);

        row!.Seq.Should().Be(ulong.MaxValue);
        row.PublicKeyVersion.Should().Be(3);
    }

    /// <summary>
    /// 会话记录合集与混合消息的 <c>content</c> 是「JSON 字符串」而非对象 ——
    /// 建成对象会让官方报文在此处直接解析失败，故类型本身即契约。
    /// </summary>
    [Fact]
    public void NestedContent_ShouldStayJsonString_WhenOfficialSendsEscapedJson()
    {
        typeof(FinanceChatRecordItem).GetProperty(nameof(FinanceChatRecordItem.ContentJson))!.PropertyType
            .Should().Be<string>("chatrecord.item.content 官方形态是转义后的 JSON 字符串");
        typeof(FinanceMixedItem).GetProperty(nameof(FinanceMixedItem.ContentJson))!.PropertyType
            .Should().Be<string>("mixed.item.content 同上");

        var message = Parse(FullMessageJson);

        var inner = message.ChatRecord!.Items![0].ContentJson!;
        using (var document = JsonDocument.Parse(inner))
        {
            document.RootElement.GetProperty("content").GetString().Should().Be("inner",
                "内层正文须由宿主二次解析，SDK 不做隐式解析（隐式即多绑一次明文）");
        }

        message.Mixed!.Items![0].ContentJson.Should().Contain("mixed-inner");
    }

    /// <summary>信封的 <c>chatdata</c>「缺省」与「空数组」是两种事实（官方在无新数据时回哪一种尚未核验）。</summary>
    [Fact]
    public void Envelope_ShouldDistinguishMissingChatDataFromEmptyList()
    {
        var omitted = JsonSerializer.Deserialize(
            """{"errcode":0,"errmsg":"ok"}""", FinanceJsonContext.Default.FinanceChatDataEnvelope);
        var empty = JsonSerializer.Deserialize(
            """{"errcode":0,"errmsg":"ok","chatdata":[]}""", FinanceJsonContext.Default.FinanceChatDataEnvelope);

        omitted!.ChatData.Should().BeNull("字段缺省不得被物化成空列表");
        empty!.ChatData.Should().NotBeNull();
        empty.ChatData!.Should().BeEmpty();
        empty.IsSuccess.Should().BeTrue("判错走 WechatWorkResponse 同一咽喉点");
    }

    /// <summary>
    /// 全 26 支正文一次反序列化：验证源生成上下文<b>传递覆盖</b>了全部嵌套类型
    /// （漏覆盖只在这里才炸，而不是编译期）。
    /// </summary>
    [Fact]
    public void FullMessage_ShouldDeserializeEveryBodyType_ThroughFinanceJsonContext()
    {
        var message = Parse(FullMessageJson);

        message.MessageId.Should().Be("msg-1");
        message.Action.Should().Be("send");
        message.FromUserId.Should().Be("woabc");
        message.ToUserIdList!.Should().Equal(new[] { "zhangsan" }, "tolist 是数组而非竖线分隔串");
        message.UserId.Should().Be("lisi");
        message.RoomId.Should().Be("wrxyz");
        message.MessageTimestampMilli.Should().Be(1700000000000L);
        message.MessageType.Should().Be("text");
        message.VoiceId.Should().Be("voice-1");
        message.VoIpId.Should().Be("voip-1");

        message.Text!.Content.Should().Be("hello");
        message.Image!.SdkFileId.Should().Be("img-id");
        message.Image.FileMd5.Should().Be("m1");
        message.Image.FileSize.Should().Be(1024);
        message.Revoke!.PreviousMessageId.Should().Be("msg-0");
        message.Agree!.UserId.Should().Be("lisi");
        message.Agree.AgreeTimestampMilli.Should().Be(1700000000001L);
        message.Voice!.DurationSeconds.Should().Be(12);
        message.Voice.FileSize.Should().Be(2048);
        message.Video!.FileSize.Should().Be(4096);
        message.BusinessCard!.CorpName.Should().Be("ACME");
        message.Location!.Latitude.Should().Be(31.23m);
        message.Location.Longitude.Should().Be(121.47m);
        message.Location.Zoom.Should().Be(15);
        message.Emotion!.Type.Should().Be(1);
        message.Emotion.FileSize.Should().Be(512);
        message.File!.FileName.Should().Be("a.pdf");
        message.File.FileExtension.Should().Be("pdf");
        message.Link!.LinkUrl.Should().Be("https://example.com");
        message.MiniProgram!.AppId.Should().Be("wx123");
        message.MiniProgram.PagePath.Should().Be("pages/a");
        message.ChatRecord!.Title.Should().Be("CR");
        message.ChatRecord.Items![0].IsFromChatRoom.Should().BeTrue();
        message.ChatRecord.Items![0].MessageTimestampMilli.Should().Be(1700000000002L);
        message.Todo!.Content.Should().Be("TC");
        message.Vote!.Options!.Should().Equal(new[] { "a", "b" });
        message.Collect!.Details![0].Id.Should().Be("12345678901234567890123",
            "官方 id 为长数字串，以 string 承载避免溢出");
        message.Collect.CreateTimeString.Should().Be("2023-11-14 15:00:00",
            "官方时间字段是文本形态，不做 DateTimeOffset 解析");
        message.RedPacket!.TotalAmount.Should().Be(1000);
        message.ExternalRedPacket!.TotalCount.Should().Be(5);
        message.Meeting!.MeetingId.Should().Be("150000000000");
        message.Meeting.Status.Should().Be(2);
        message.Document!.CreatorUserId.Should().Be("lisi");
        message.Info!.NewsItems![0].PictureUrl.Should().Be("https://np");
        message.Info.VoIpCallDuration.Should().Be(60);
        message.Info.MeetingId.Should().Be("150000000001");
        message.Calendar!.AttendeeNameList!.Should().Equal(new[] { "p1", "p2" });
        message.Mixed!.Items![0].Type.Should().Be("text");
        message.MeetingVoiceCall!.ShareFileDataList![0].OperatorUserId.Should().Be("lisi");
        message.MeetingVoiceCall.ShareScreenDataList![0].SharerUserId.Should().Be("lisi");
        message.VoIpDocumentShare!.FileMd5.Should().Be("m6");
        message.ChannelsFeed!.FeedType.Should().Be(2);
    }

    /// <summary>官方会新增正文类型；未建模的键必须被忽略，而不是让整条记录解析失败。</summary>
    [Fact]
    public void UnknownKeys_ShouldBeIgnored_WhenOfficialAddsNewMessageTypes()
    {
        var json = """{"msgid":"m-2","msgtype":"newest","newest":{"foo":1},"brand_new_key":"x"}""";

        var message = JsonSerializer.Deserialize(json, FinanceJsonContext.Default.FinanceChatMessage);

        message!.MessageId.Should().Be("m-2",
            "未建模的 msgtype 只应丢掉正文，不应让已知的消息头一并失败");
        message.MessageType.Should().Be("newest");
    }

    private static FinanceChatMessage Parse(string json)
        => JsonSerializer.Deserialize(json, FinanceJsonContext.Default.FinanceChatMessage)!;

    /// <summary>
    /// 本域全部 DTO（按命名空间取，新增类型自动纳入键形校验）。
    /// </summary>
    /// <remarks>
    /// 排除源生成的 <c>FinanceJsonContext</c>（同命名空间但不是 DTO）与编译器闭包类型（名含 <c>&lt;</c>）。
    /// </remarks>
    private static Type[] FinanceDtoTypes()
        => typeof(FinanceChatMessage).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract
                               && t.Namespace == "Mud.Wechat.Work.DataModels.Finance"
                               && t != typeof(FinanceJsonContext)
                               && t.Name.IndexOf('<') < 0)
            .OrderBy(static t => t.Name, StringComparer.Ordinal)
            .ToArray();

    /// <summary>声明属性 → 官方键名（升序）。基类属性不在此列（由基类自己的测试锁）。</summary>
    private static string[] JsonKeysOf(Type type)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        properties.Should().NotBeEmpty($"{type.Name} 应声明自有字段");

        return properties.Select(p => JsonName(p, type)).OrderBy(static n => n, StringComparer.Ordinal).ToArray();
    }

    private static string[] Sort(IEnumerable<string> keys)
        => keys.OrderBy(static n => n, StringComparer.Ordinal).ToArray();

    private static string JsonName(PropertyInfo property, Type owner)
    {
        var attribute = property.GetCustomAttribute<JsonPropertyNameAttribute>();
        attribute.Should().NotBeNull($"{owner.Name}.{property.Name} 必须显式声明官方键名（不依赖命名策略推断）");
        return attribute!.Name;
    }

    private static string JsonNameOf<T>(string propertyName)
    {
        var property = typeof(T).GetProperty(propertyName);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 不存在");
        return JsonName(property!, typeof(T));
    }

    private const string FullMessageJson = """
{
  "msgid": "msg-1",
  "action": "send",
  "from": "woabc",
  "tolist": ["zhangsan"],
  "user": "lisi",
  "roomid": "wrxyz",
  "msgtime": 1700000000000,
  "msgtype": "text",
  "voiceid": "voice-1",
  "voipid": "voip-1",
  "text": { "content": "hello" },
  "image": { "sdkfileid": "img-id", "md5sum": "m1", "filesize": 1024 },
  "revoke": { "pre_msgid": "msg-0" },
  "agree": { "userid": "lisi", "agree_time": 1700000000001 },
  "voice": { "play_length": 12, "sdkfileid": "voice-id", "md5sum": "m2", "voice_size": 2048 },
  "video": { "play_length": 30, "sdkfileid": "video-id", "md5sum": "m3", "filesize": 4096 },
  "card": { "corpname": "ACME", "userid": "lisi" },
  "location": { "latitude": 31.23, "longitude": 121.47, "title": "T", "address": "A", "zoom": 15 },
  "emotion": { "type": 1, "width": 100, "height": 200, "sdkfileid": "emo-id", "md5sum": "m4", "imagesize": 512 },
  "file": { "filename": "a.pdf", "fileext": "pdf", "sdkfileid": "file-id", "md5sum": "m5", "filesize": 8192 },
  "link": { "link_url": "https://example.com", "title": "L", "description": "D", "image_url": "https://example.com/i.png" },
  "weapp": { "title": "MP", "description": "D", "displayname": "AppName", "username": "gh_xxx", "appid": "wx123", "pagepath": "pages/a" },
  "chatrecord": { "title": "CR", "item": [ { "msgtime": 1700000000002, "type": "text", "content": "{\"content\":\"inner\"}", "from_chatroom": true } ] },
  "todo": { "title": "TD", "content": "TC" },
  "vote": { "voteid": "v-1", "votetype": 1, "votetitle": "VT", "voteitem": ["a", "b"] },
  "collect": { "room_name": "RM", "creator": "CR", "create_time": "2023-11-14 15:00:00", "title": "CT", "details": [ { "id": "12345678901234567890123", "type": "text", "ques": "Q" } ] },
  "redpacket": { "type": 1, "wish": "W", "totalcnt": 10, "totalamount": 1000 },
  "external_redpacket": { "type": 2, "wish": "EW", "totalcnt": 5, "totalamount": 500 },
  "meeting": { "meetingtype": 1, "meetingid": "150000000000", "topic": "TP", "starttime": 1700000000, "endtime": 1700003600, "address": "MA", "remarks": "MR", "status": 2 },
  "doc": { "title": "DOC", "link_url": "https://doc", "doc_creator": "lisi" },
  "info": { "content": "MD", "item": [ { "url": "https://n", "title": "NT", "description": "ND", "picurl": "https://np" } ], "callduration": 60, "invitetype": 1, "filename": "wf", "meeting_id": "150000000001", "notification_type": 3 },
  "calendar": { "title": "CAL", "creatorname": "CN", "starttime": 1700000000, "endtime": 1700003600, "attendeename": ["p1", "p2"], "place": "PL", "remarks": "CR2" },
  "mixed": { "item": [ { "type": "text", "content": "{\"content\":\"mixed-inner\"}" } ] },
  "meeting_voice_call": { "sdkfileid": "mvc-id", "endtime": 1700003600, "demofiledata": [ { "filename": "d1", "demooperator": "lisi", "starttime": 1700000000, "endtime": 1700001000 } ], "sharescreendata": [ { "share": "lisi", "starttime": 1700001000, "endtime": 1700002000 } ] },
  "voip_doc_share": { "filename": "vd", "sdkfileid": "vds-id", "md5sum": "m6", "filesize": 1024 },
  "sphfeed": { "feed_type": 2, "sph_name": "SN", "feed_desc": "SD" }
}
""";
}
