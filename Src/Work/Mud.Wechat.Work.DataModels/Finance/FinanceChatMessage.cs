// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Finance;

/// <summary>
/// 会话内容存档的<b>解密后明文</b>消息（C SDK <c>DecryptData</c> 写回 <c>Slice_t</c> 的 JSON）。
/// </summary>
/// <remarks>
/// <para>
/// <b>一个类型覆盖全部 msgtype</b>：官方把消息体做成「公共头字段 + 与 <c>msgtype</c> 同名的一个对象字段」
/// （如 <c>msgtype == "image"</c> 时正文在 <c>image</c> 键下），而不是多态信封 ⇒ 这里按同名兄弟字段逐个建模为
/// 可空属性，调用侧按 <see cref="MessageType"/> 取对应属性。这与「三种模式共用一份可空超集」的回调载荷纪律同源，
/// 好处是<b>无需反射即可判定形态</b>、AOT 友好；代价是一次反序列化会尝试绑定全部 26 支正文类型。
/// </para>
/// <para>
/// <b>只登记 1 支根类型</b>：<see cref="FinanceChatMessage"/> 标 <c>[HttpJsonSerializable]</c>，
/// 正文类型作为其属性类型被源生成器<b>传递</b>覆盖，故 <c>FinanceJsonContext</c> 不逐条登记它们。
/// </para>
/// <para>
/// <b>字段来源</b>：与本地 SKIT 源码
/// <c>SKIT.FlurlHttpClient.Wechat.Work/ExtendedSDK/Finance/Models/{DecryptChatRecordResponse.cs,__Abstractions/ChatMessage.cs}</c>
/// 对齐（键名逐字照抄，含 <c>sdkfileid</c> / <c>md5sum</c> / <c>pre_msgid</c> 等不规则拼写）。
/// 官方页 <c>https://developer.work.weixin.qq.com/document/path/91774</c> 的逐字段原文**待逐页核验** ⇒
/// 本类型<b>不</b>作为契约守卫的断言对象（守卫只锁「字段名不驼峰化」这类形态约束）。
/// </para>
/// <para>
/// <b>明文即敏感</b>：本类型承载会话正文，任何情况下不得整体写入日志、遥测或异常消息（守卫 FIN-B5）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Finance")]
public class FinanceChatMessage
{
    /// <summary>消息 ID（官方键 <c>msgid</c>）。</summary>
    [JsonPropertyName("msgid")]
    public string? MessageId { get; set; }

    /// <summary>消息动作（官方键 <c>action</c>，如 <c>send</c> / <c>revoke</c>）。</summary>
    [JsonPropertyName("action")]
    public string? Action { get; set; }

    /// <summary>发送方账号（官方键 <c>from</c>）。</summary>
    [JsonPropertyName("from")]
    public string? FromUserId { get; set; }

    /// <summary>接收方账号列表（官方键 <c>tolist</c>）。</summary>
    [JsonPropertyName("tolist")]
    public string[]? ToUserIdList { get; set; }

    /// <summary>企业成员账号（官方键 <c>user</c>）。</summary>
    [JsonPropertyName("user")]
    public string? UserId { get; set; }

    /// <summary>群聊房间 ID（官方键 <c>roomid</c>）。</summary>
    [JsonPropertyName("roomid")]
    public string? RoomId { get; set; }

    /// <summary>消息发送时间戳（官方键 <c>msgtime</c>，毫秒级）。</summary>
    [JsonPropertyName("msgtime")]
    public long MessageTimestampMilli { get; set; }

    /// <summary>消息类型（官方键 <c>msgtype</c>）—— 决定下面哪一支正文属性非空。</summary>
    [JsonPropertyName("msgtype")]
    public string? MessageType { get; set; }

    /// <summary>语音/通话 ID（官方键 <c>voiceid</c>）。</summary>
    [JsonPropertyName("voiceid")]
    public string? VoiceId { get; set; }

    /// <summary>VoIP 通话 ID（官方键 <c>voipid</c>）。</summary>
    [JsonPropertyName("voipid")]
    public string? VoIpId { get; set; }

    /// <summary>文本正文（官方键 <c>text</c>）。</summary>
    [JsonPropertyName("text")]
    public FinanceTextMessageContent? Text { get; set; }

    /// <summary>图片正文（官方键 <c>image</c>）。</summary>
    [JsonPropertyName("image")]
    public FinanceImageMessageContent? Image { get; set; }

    /// <summary>撤回正文（官方键 <c>revoke</c>）。</summary>
    [JsonPropertyName("revoke")]
    public FinanceRevokeMessageContent? Revoke { get; set; }

    /// <summary>同意/不同意存档正文（官方键 <c>agree</c>）。</summary>
    [JsonPropertyName("agree")]
    public FinanceAgreeMessageContent? Agree { get; set; }

    /// <summary>语音正文（官方键 <c>voice</c>）。</summary>
    [JsonPropertyName("voice")]
    public FinanceVoiceMessageContent? Voice { get; set; }

    /// <summary>视频正文（官方键 <c>video</c>）。</summary>
    [JsonPropertyName("video")]
    public FinanceVideoMessageContent? Video { get; set; }

    /// <summary>名片正文（官方键 <c>card</c>）。</summary>
    [JsonPropertyName("card")]
    public FinanceBusinessCardMessageContent? BusinessCard { get; set; }

    /// <summary>位置正文（官方键 <c>location</c>）。</summary>
    [JsonPropertyName("location")]
    public FinanceLocationMessageContent? Location { get; set; }

    /// <summary>表情正文（官方键 <c>emotion</c>）。</summary>
    [JsonPropertyName("emotion")]
    public FinanceEmotionMessageContent? Emotion { get; set; }

    /// <summary>文件正文（官方键 <c>file</c>）。</summary>
    [JsonPropertyName("file")]
    public FinanceFileMessageContent? File { get; set; }

    /// <summary>图文链接正文（官方键 <c>link</c>）。</summary>
    [JsonPropertyName("link")]
    public FinanceLinkMessageContent? Link { get; set; }

    /// <summary>小程序正文（官方键 <c>weapp</c>）。</summary>
    [JsonPropertyName("weapp")]
    public FinanceMiniProgramMessageContent? MiniProgram { get; set; }

    /// <summary>会话记录合集正文（官方键 <c>chatrecord</c>）。</summary>
    [JsonPropertyName("chatrecord")]
    public FinanceChatRecordMessageContent? ChatRecord { get; set; }

    /// <summary>待办正文（官方键 <c>todo</c>）。</summary>
    [JsonPropertyName("todo")]
    public FinanceTodoMessageContent? Todo { get; set; }

    /// <summary>投票正文（官方键 <c>vote</c>）。</summary>
    [JsonPropertyName("vote")]
    public FinanceVoteMessageContent? Vote { get; set; }

    /// <summary>填表正文（官方键 <c>collect</c>）。</summary>
    [JsonPropertyName("collect")]
    public FinanceCollectMessageContent? Collect { get; set; }

    /// <summary>红包正文（官方键 <c>redpacket</c>）。</summary>
    [JsonPropertyName("redpacket")]
    public FinanceRedPacketMessageContent? RedPacket { get; set; }

    /// <summary>互通红包正文（官方键 <c>external_redpacket</c>，与 <c>redpacket</c> 同形）。</summary>
    [JsonPropertyName("external_redpacket")]
    public FinanceRedPacketMessageContent? ExternalRedPacket { get; set; }

    /// <summary>会议正文（官方键 <c>meeting</c>）。</summary>
    [JsonPropertyName("meeting")]
    public FinanceMeetingMessageContent? Meeting { get; set; }

    /// <summary>在线文档正文（官方键 <c>doc</c>）。</summary>
    [JsonPropertyName("doc")]
    public FinanceDocumentMessageContent? Document { get; set; }

    /// <summary>Markdown / 图文 / 音视频通话正文（官方键 <c>info</c>）。</summary>
    [JsonPropertyName("info")]
    public FinanceInfoMessageContent? Info { get; set; }

    /// <summary>日程正文（官方键 <c>calendar</c>）。</summary>
    [JsonPropertyName("calendar")]
    public FinanceCalendarMessageContent? Calendar { get; set; }

    /// <summary>混合消息正文（官方键 <c>mixed</c>）。</summary>
    [JsonPropertyName("mixed")]
    public FinanceMixedMessageContent? Mixed { get; set; }

    /// <summary>会议音频存档正文（官方键 <c>meeting_voice_call</c>）。</summary>
    [JsonPropertyName("meeting_voice_call")]
    public FinanceMeetingVoiceCallMessageContent? MeetingVoiceCall { get; set; }

    /// <summary>VoIP 文档共享存档正文（官方键 <c>voip_doc_share</c>）。</summary>
    [JsonPropertyName("voip_doc_share")]
    public FinanceVoIpDocumentShareMessageContent? VoIpDocumentShare { get; set; }

    /// <summary>视频号正文（官方键 <c>sphfeed</c>）。</summary>
    [JsonPropertyName("sphfeed")]
    public FinanceChannelsFeedMessageContent? ChannelsFeed { get; set; }
}

/// <summary>文本消息正文。</summary>
public class FinanceTextMessageContent
{
    /// <summary>文本内容（官方键 <c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

/// <summary>带 <c>sdkfileid</c> 类正文的公共形状（图片/语音/视频/文件/表情等共用其字段名，各自单列类型以保留差异字段）。</summary>
public class FinanceImageMessageContent
{
    /// <summary>媒体文件的 C SDK 下载凭据（官方键 <c>sdkfileid</c>）—— 送 <c>GetMediaData</c> 的入参。</summary>
    [JsonPropertyName("sdkfileid")]
    public string? SdkFileId { get; set; }

    /// <summary>文件 MD5（官方键 <c>md5sum</c>）。</summary>
    [JsonPropertyName("md5sum")]
    public string? FileMd5 { get; set; }

    /// <summary>文件大小（官方键 <c>filesize</c>，单位字节）。</summary>
    [JsonPropertyName("filesize")]
    public long FileSize { get; set; }
}

/// <summary>撤回消息正文。</summary>
public class FinanceRevokeMessageContent
{
    /// <summary>被撤回的原消息 ID（官方键 <c>pre_msgid</c>）。</summary>
    [JsonPropertyName("pre_msgid")]
    public string? PreviousMessageId { get; set; }
}

/// <summary>同意/不同意会话存档正文。</summary>
public class FinanceAgreeMessageContent
{
    /// <summary>操作成员账号（官方键 <c>userid</c>）。</summary>
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }

    /// <summary>同意时间戳（官方键 <c>agree_time</c>，毫秒级）。</summary>
    [JsonPropertyName("agree_time")]
    public long AgreeTimestampMilli { get; set; }
}

/// <summary>语音消息正文。</summary>
public class FinanceVoiceMessageContent
{
    /// <summary>语音时长（官方键 <c>play_length</c>，单位秒）。</summary>
    [JsonPropertyName("play_length")]
    public int DurationSeconds { get; set; }

    /// <summary>媒体文件下载凭据（官方键 <c>sdkfileid</c>）。</summary>
    [JsonPropertyName("sdkfileid")]
    public string? SdkFileId { get; set; }

    /// <summary>文件 MD5（官方键 <c>md5sum</c>）。</summary>
    [JsonPropertyName("md5sum")]
    public string? FileMd5 { get; set; }

    /// <summary>语音大小（官方键 <c>voice_size</c>，单位字节 —— 本页不用 <c>filesize</c>）。</summary>
    [JsonPropertyName("voice_size")]
    public long FileSize { get; set; }
}

/// <summary>视频消息正文。</summary>
public class FinanceVideoMessageContent
{
    /// <summary>视频时长（官方键 <c>play_length</c>，单位秒）。</summary>
    [JsonPropertyName("play_length")]
    public int DurationSeconds { get; set; }

    /// <summary>媒体文件下载凭据（官方键 <c>sdkfileid</c>）。</summary>
    [JsonPropertyName("sdkfileid")]
    public string? SdkFileId { get; set; }

    /// <summary>文件 MD5（官方键 <c>md5sum</c>）。</summary>
    [JsonPropertyName("md5sum")]
    public string? FileMd5 { get; set; }

    /// <summary>文件大小（官方键 <c>filesize</c>，单位字节）。</summary>
    [JsonPropertyName("filesize")]
    public long FileSize { get; set; }
}

/// <summary>名片消息正文。</summary>
public class FinanceBusinessCardMessageContent
{
    /// <summary>企业名称（官方键 <c>corpname</c>）。</summary>
    [JsonPropertyName("corpname")]
    public string? CorpName { get; set; }

    /// <summary>成员账号（官方键 <c>userid</c>）。</summary>
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }
}

/// <summary>位置消息正文。</summary>
public class FinanceLocationMessageContent
{
    /// <summary>纬度（官方键 <c>latitude</c>）。</summary>
    [JsonPropertyName("latitude")]
    public decimal Latitude { get; set; }

    /// <summary>经度（官方键 <c>longitude</c>）。</summary>
    [JsonPropertyName("longitude")]
    public decimal Longitude { get; set; }

    /// <summary>位置名称（官方键 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>详细地址（官方键 <c>address</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>缩放比例（官方键 <c>zoom</c>）。</summary>
    [JsonPropertyName("zoom")]
    public int Zoom { get; set; }
}

/// <summary>表情消息正文。</summary>
public class FinanceEmotionMessageContent
{
    /// <summary>表情类型（官方键 <c>type</c>）。</summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

    /// <summary>宽度（官方键 <c>width</c>，单位像素）。</summary>
    [JsonPropertyName("width")]
    public int Width { get; set; }

    /// <summary>高度（官方键 <c>height</c>，单位像素）。</summary>
    [JsonPropertyName("height")]
    public int Height { get; set; }

    /// <summary>媒体文件下载凭据（官方键 <c>sdkfileid</c>）。</summary>
    [JsonPropertyName("sdkfileid")]
    public string? SdkFileId { get; set; }

    /// <summary>文件 MD5（官方键 <c>md5sum</c>）。</summary>
    [JsonPropertyName("md5sum")]
    public string? FileMd5 { get; set; }

    /// <summary>图片大小（官方键 <c>imagesize</c>，单位字节 —— 本页不用 <c>filesize</c>）。</summary>
    [JsonPropertyName("imagesize")]
    public long FileSize { get; set; }
}

/// <summary>文件消息正文。</summary>
public class FinanceFileMessageContent
{
    /// <summary>文件名（官方键 <c>filename</c>）。</summary>
    [JsonPropertyName("filename")]
    public string? FileName { get; set; }

    /// <summary>文件后缀（官方键 <c>fileext</c>）。</summary>
    [JsonPropertyName("fileext")]
    public string? FileExtension { get; set; }

    /// <summary>媒体文件下载凭据（官方键 <c>sdkfileid</c>）。</summary>
    [JsonPropertyName("sdkfileid")]
    public string? SdkFileId { get; set; }

    /// <summary>文件 MD5（官方键 <c>md5sum</c>）。</summary>
    [JsonPropertyName("md5sum")]
    public string? FileMd5 { get; set; }

    /// <summary>文件大小（官方键 <c>filesize</c>，单位字节）。</summary>
    [JsonPropertyName("filesize")]
    public long FileSize { get; set; }
}

/// <summary>图文链接消息正文。</summary>
public class FinanceLinkMessageContent
{
    /// <summary>跳转链接（官方键 <c>link_url</c>）。</summary>
    [JsonPropertyName("link_url")]
    public string? LinkUrl { get; set; }

    /// <summary>标题（官方键 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>描述（官方键 <c>description</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>封面图 URL（官方键 <c>image_url</c>）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }
}

/// <summary>小程序消息正文。</summary>
public class FinanceMiniProgramMessageContent
{
    /// <summary>标题（官方键 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>描述（官方键 <c>description</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>小程序名称（官方键 <c>displayname</c>）。</summary>
    [JsonPropertyName("displayname")]
    public string? DisplayName { get; set; }

    /// <summary>发送者名称（官方键 <c>username</c>）。</summary>
    [JsonPropertyName("username")]
    public string? UserName { get; set; }

    /// <summary>小程序 AppId（官方键 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>页面路径（官方键 <c>pagepath</c>）。</summary>
    [JsonPropertyName("pagepath")]
    public string? PagePath { get; set; }
}

/// <summary>会话记录合集正文。</summary>
public class FinanceChatRecordMessageContent
{
    /// <summary>合集标题（官方键 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>合集条目（官方键 <c>item</c>）。</summary>
    [JsonPropertyName("item")]
    public FinanceChatRecordItem[]? Items { get; set; }
}

/// <summary>会话记录合集的一条条目。</summary>
public class FinanceChatRecordItem
{
    /// <summary>条目发送时间戳（官方键 <c>msgtime</c>）。</summary>
    [JsonPropertyName("msgtime")]
    public long MessageTimestampMilli { get; set; }

    /// <summary>条目的原始消息类型（官方键 <c>type</c>）。</summary>
    [JsonPropertyName("type")]
    public string? MessageType { get; set; }

    /// <summary>条目正文的 <b>JSON 字符串</b>（官方键 <c>content</c> —— 是字符串而非对象，需二次解析）。</summary>
    [JsonPropertyName("content")]
    public string? ContentJson { get; set; }

    /// <summary>是否来自群聊（官方键 <c>from_chatroom</c>）。</summary>
    [JsonPropertyName("from_chatroom")]
    public bool IsFromChatRoom { get; set; }
}

/// <summary>待办消息正文。</summary>
public class FinanceTodoMessageContent
{
    /// <summary>待办来源文本（官方键 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>待办内容（官方键 <c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

/// <summary>投票消息正文。</summary>
public class FinanceVoteMessageContent
{
    /// <summary>投票 ID（官方键 <c>voteid</c>）。</summary>
    [JsonPropertyName("voteid")]
    public string? VoteId { get; set; }

    /// <summary>投票类型（官方键 <c>votetype</c>）。</summary>
    [JsonPropertyName("votetype")]
    public int Type { get; set; }

    /// <summary>投票主题（官方键 <c>votetitle</c>）。</summary>
    [JsonPropertyName("votetitle")]
    public string? Title { get; set; }

    /// <summary>投票选项（官方键 <c>voteitem</c>）。</summary>
    [JsonPropertyName("voteitem")]
    public string[]? Options { get; set; }
}

/// <summary>填表消息正文。</summary>
public class FinanceCollectMessageContent
{
    /// <summary>群聊名称（官方键 <c>room_name</c>）。</summary>
    [JsonPropertyName("room_name")]
    public string? RoomName { get; set; }

    /// <summary>创建者名称（官方键 <c>creator</c>）。</summary>
    [JsonPropertyName("creator")]
    public string? CreatorName { get; set; }

    /// <summary>创建时间（官方键 <c>create_time</c>，官方为 <c>yyyy-MM-dd HH:mm:ss</c> 文本）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTimeString { get; set; }

    /// <summary>表名（官方键 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>表项（官方键 <c>details</c>）。</summary>
    [JsonPropertyName("details")]
    public FinanceCollectDetail[]? Details { get; set; }
}

/// <summary>填表消息的一个表项。</summary>
public class FinanceCollectDetail
{
    /// <summary>表项 ID（官方键 <c>id</c>，官方为数字串 ⇒ 以 <see cref="string"/> 承载避免溢出）。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>表项类型（官方键 <c>type</c>）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>表项名称（官方键 <c>ques</c>）。</summary>
    [JsonPropertyName("ques")]
    public string? Question { get; set; }
}

/// <summary>红包消息正文（<c>redpacket</c> 与 <c>external_redpacket</c> 共用）。</summary>
public class FinanceRedPacketMessageContent
{
    /// <summary>红包类型（官方键 <c>type</c>）。</summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

    /// <summary>祝福语（官方键 <c>wish</c>）。</summary>
    [JsonPropertyName("wish")]
    public string? Wishing { get; set; }

    /// <summary>总个数（官方键 <c>totalcnt</c>）。</summary>
    [JsonPropertyName("totalcnt")]
    public int TotalCount { get; set; }

    /// <summary>总金额（官方键 <c>totalamount</c>，单位分）。</summary>
    [JsonPropertyName("totalamount")]
    public long TotalAmount { get; set; }
}

/// <summary>会议消息正文。</summary>
public class FinanceMeetingMessageContent
{
    /// <summary>会议类型（官方键 <c>meetingtype</c>）。</summary>
    [JsonPropertyName("meetingtype")]
    public int Type { get; set; }

    /// <summary>会议 ID（官方键 <c>meetingid</c>，官方为数字串）。</summary>
    [JsonPropertyName("meetingid")]
    public string? MeetingId { get; set; }

    /// <summary>会议主题（官方键 <c>topic</c>）。</summary>
    [JsonPropertyName("topic")]
    public string? Topic { get; set; }

    /// <summary>开始时间戳（官方键 <c>starttime</c>）。</summary>
    [JsonPropertyName("starttime")]
    public long StartTimestamp { get; set; }

    /// <summary>结束时间戳（官方键 <c>endtime</c>）。</summary>
    [JsonPropertyName("endtime")]
    public long EndTimestamp { get; set; }

    /// <summary>会议地址（官方键 <c>address</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>备注（官方键 <c>remarks</c>）。</summary>
    [JsonPropertyName("remarks")]
    public string? Remark { get; set; }

    /// <summary>邀请处理状态（官方键 <c>status</c>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }
}

/// <summary>在线文档消息正文。</summary>
public class FinanceDocumentMessageContent
{
    /// <summary>文档标题（官方键 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>文档链接（官方键 <c>link_url</c>）。</summary>
    [JsonPropertyName("link_url")]
    public string? LinkUrl { get; set; }

    /// <summary>创建者账号（官方键 <c>doc_creator</c>）。</summary>
    [JsonPropertyName("doc_creator")]
    public string? CreatorUserId { get; set; }
}

/// <summary>Markdown / 图文 / 音视频通话正文（官方把多形态收敛在同一 <c>info</c> 键下）。</summary>
public class FinanceInfoMessageContent
{
    /// <summary>内容（官方键 <c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>图文列表（官方键 <c>item</c>）。</summary>
    [JsonPropertyName("item")]
    public FinanceInfoNewsItem[]? NewsItems { get; set; }

    /// <summary>VoIP 通话时长（官方键 <c>callduration</c>，单位秒）。</summary>
    [JsonPropertyName("callduration")]
    public int? VoIpCallDuration { get; set; }

    /// <summary>VoIP 邀请类型（官方键 <c>invitetype</c>）。</summary>
    [JsonPropertyName("invitetype")]
    public int? VoIpInviteType { get; set; }

    /// <summary>微盘文件名（官方键 <c>filename</c>）。</summary>
    [JsonPropertyName("filename")]
    public string? WedriveFileName { get; set; }

    /// <summary>会议 ID（官方键 <c>meeting_id</c>，注意此处带下划线、与 <c>meetingid</c> 不同键）。</summary>
    [JsonPropertyName("meeting_id")]
    public string? MeetingId { get; set; }

    /// <summary>通知类型（官方键 <c>notification_type</c>）。</summary>
    [JsonPropertyName("notification_type")]
    public int? NotificationType { get; set; }
}

/// <summary><c>info</c> 正文中的一条图文。</summary>
public class FinanceInfoNewsItem
{
    /// <summary>链接（官方键 <c>url</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>标题（官方键 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>描述（官方键 <c>description</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>配图 URL（官方键 <c>picurl</c>）。</summary>
    [JsonPropertyName("picurl")]
    public string? PictureUrl { get; set; }
}

/// <summary>日程消息正文。</summary>
public class FinanceCalendarMessageContent
{
    /// <summary>日程主题（官方键 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>组织者名称（官方键 <c>creatorname</c>）。</summary>
    [JsonPropertyName("creatorname")]
    public string? CreatorName { get; set; }

    /// <summary>开始时间戳（官方键 <c>starttime</c>）。</summary>
    [JsonPropertyName("starttime")]
    public long StartTimestamp { get; set; }

    /// <summary>结束时间戳（官方键 <c>endtime</c>）。</summary>
    [JsonPropertyName("endtime")]
    public long EndTimestamp { get; set; }

    /// <summary>参与人名称（官方键 <c>attendeename</c>）。</summary>
    [JsonPropertyName("attendeename")]
    public string[]? AttendeeNameList { get; set; }

    /// <summary>地点（官方键 <c>place</c>）。</summary>
    [JsonPropertyName("place")]
    public string? Place { get; set; }

    /// <summary>备注（官方键 <c>remarks</c>）。</summary>
    [JsonPropertyName("remarks")]
    public string? Remark { get; set; }
}

/// <summary>混合消息正文。</summary>
public class FinanceMixedMessageContent
{
    /// <summary>混合条目（官方键 <c>item</c>）。</summary>
    [JsonPropertyName("item")]
    public FinanceMixedItem[]? Items { get; set; }
}

/// <summary>混合消息的一条条目。</summary>
public class FinanceMixedItem
{
    /// <summary>条目类型（官方键 <c>type</c>）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>条目正文的 <b>JSON 字符串</b>（官方键 <c>content</c> —— 是字符串而非对象，需二次解析）。</summary>
    [JsonPropertyName("content")]
    public string? ContentJson { get; set; }
}

/// <summary>会议音频存档正文。</summary>
public class FinanceMeetingVoiceCallMessageContent
{
    /// <summary>音频文件下载凭据（官方键 <c>sdkfileid</c>）。</summary>
    [JsonPropertyName("sdkfileid")]
    public string? SdkFileId { get; set; }

    /// <summary>结束时间戳（官方键 <c>endtime</c>）。</summary>
    [JsonPropertyName("endtime")]
    public long EndTimestamp { get; set; }

    /// <summary>文档共享条目（官方键 <c>demofiledata</c>）。</summary>
    [JsonPropertyName("demofiledata")]
    public FinanceShareFileData[]? ShareFileDataList { get; set; }

    /// <summary>屏幕共享条目（官方键 <c>sharescreendata</c>）。</summary>
    [JsonPropertyName("sharescreendata")]
    public FinanceShareScreenData[]? ShareScreenDataList { get; set; }
}

/// <summary>会议 / VoIP 存档中的文档共享条目。</summary>
public class FinanceShareFileData
{
    /// <summary>文件名（官方键 <c>filename</c>）。</summary>
    [JsonPropertyName("filename")]
    public string? FileName { get; set; }

    /// <summary>演示操作者账号（官方键 <c>demooperator</c>）。</summary>
    [JsonPropertyName("demooperator")]
    public string? OperatorUserId { get; set; }

    /// <summary>开始时间戳（官方键 <c>starttime</c>）。</summary>
    [JsonPropertyName("starttime")]
    public long StartTimestamp { get; set; }

    /// <summary>结束时间戳（官方键 <c>endtime</c>）。</summary>
    [JsonPropertyName("endtime")]
    public long EndTimestamp { get; set; }
}

/// <summary>会议存档中的屏幕共享条目。</summary>
public class FinanceShareScreenData
{
    /// <summary>分享者账号（官方键 <c>share</c>）。</summary>
    [JsonPropertyName("share")]
    public string? SharerUserId { get; set; }

    /// <summary>开始时间戳（官方键 <c>starttime</c>）。</summary>
    [JsonPropertyName("starttime")]
    public long StartTimestamp { get; set; }

    /// <summary>结束时间戳（官方键 <c>endtime</c>）。</summary>
    [JsonPropertyName("endtime")]
    public long EndTimestamp { get; set; }
}

/// <summary>VoIP 文档共享存档正文。</summary>
public class FinanceVoIpDocumentShareMessageContent
{
    /// <summary>音频文件名（官方键 <c>filename</c>）。</summary>
    [JsonPropertyName("filename")]
    public string? FileName { get; set; }

    /// <summary>音频文件下载凭据（官方键 <c>sdkfileid</c>）。</summary>
    [JsonPropertyName("sdkfileid")]
    public string? SdkFileId { get; set; }

    /// <summary>文件 MD5（官方键 <c>md5sum</c>）。</summary>
    [JsonPropertyName("md5sum")]
    public string? FileMd5 { get; set; }

    /// <summary>文件大小（官方键 <c>filesize</c>，单位字节）。</summary>
    [JsonPropertyName("filesize")]
    public long FileSize { get; set; }
}

/// <summary>视频号消息正文。</summary>
public class FinanceChannelsFeedMessageContent
{
    /// <summary>动态类型（官方键 <c>feed_type</c>）。</summary>
    [JsonPropertyName("feed_type")]
    public int FeedType { get; set; }

    /// <summary>视频号账号名称（官方键 <c>sph_name</c>）。</summary>
    [JsonPropertyName("sph_name")]
    public string? ChannelsNickName { get; set; }

    /// <summary>动态描述（官方键 <c>feed_desc</c>）。</summary>
    [JsonPropertyName("feed_desc")]
    public string? Description { get; set; }
}
