// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Mass;

/// <summary>群发接收者过滤条件（官方 <c>filter</c> 对象，sendall 专用）。</summary>
/// <remarks>
/// 官方契约（逐页核验 2026-10-07）：<c>is_to_all</c> 为 true 时群发给所有用户（可不填 tag_id）；
/// false 时按 <c>tag_id</c> 发送给指定群组。官方示例中 tag_id 数字与字符串形态并存（照录）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpMassFilter
{
    /// <summary>获取或设置是否向全部用户发送（官方 <c>is_to_all</c>：true 全部 / false 按 tag_id）。</summary>
    [JsonPropertyName("is_to_all")]
    public bool IsToAll { get; set; }

    /// <summary>获取或设置群发到的标签 id（官方 <c>tag_id</c>；is_to_all = true 时可不填）。</summary>
    [JsonPropertyName("tag_id")]
    public int? TagId { get; set; }
}

/// <summary>
/// 单 media_id 载体的群发消息分支（官方 mpnews / voice / mpvideo（sendall 页）/
/// music、image（preview 页）的请求体形态均为单个 <c>media_id</c> ⇒ 共用 DTO，由守卫双向锁定）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpMassMediaMessage
{
    /// <summary>获取或设置用于群发的素材 media_id（官方 <c>media_id</c>）。</summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }
}

/// <summary>文本消息分支（官方 <c>text</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpMassText
{
    /// <summary>获取或设置文本内容（官方 <c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

/// <summary>图片消息分支（官方 <c>images</c>；多图 + 推荐语形态，与 preview 页的单图 image 分支不同）。</summary>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpMassImages
{
    /// <summary>获取或设置用于群发的 media_id 列表（官方 <c>media_ids</c>；官方示例 3 个）。</summary>
    [JsonPropertyName("media_ids")]
    public List<string>? MediaIds { get; set; }

    /// <summary>获取或设置推荐语（官方 <c>recommend</c>）。</summary>
    [JsonPropertyName("recommend")]
    public string? Recommend { get; set; }

    /// <summary>获取或设置标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置是否开启评论（官方 <c>need_open_comment</c>：1 开启 / 0 关闭）。</summary>
    [JsonPropertyName("need_open_comment")]
    public int? NeedOpenComment { get; set; }

    /// <summary>获取或设置是否仅粉丝可评论（官方 <c>only_fans_can_comment</c>：1 开启 / 0 关闭）。</summary>
    [JsonPropertyName("only_fans_can_comment")]
    public int? OnlyFansCanComment { get; set; }
}

/// <summary>视频消息分支（官方 <c>mpvideo</c>；mass/send 页比 sendall 页多 title/description 子字段 ⇒ 超集建模）。</summary>
/// <remarks>
/// <b>官方文档矛盾（照录）</b>：mass/send 页 msgtype 说明写「视频为 video」但请求体对象名为
/// <c>mpvideo</c>（sendall 页写「视频为 mpvideo」）——以请求体对象名为准。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpMassVideoMessage
{
    /// <summary>获取或设置用于群发的视频素材 media_id（官方 <c>media_id</c>）。</summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>获取或设置视频标题（官方 <c>title</c>；仅 mass/send 页字段表出现）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置视频描述（官方 <c>description</c>；仅 mass/send 页字段表出现）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

/// <summary>卡券消息分支（官方 <c>wxcard</c>；preview 页多 card_ext 子对象 ⇒ 超集建模）。</summary>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpMassWxCard
{
    /// <summary>获取或设置卡券 ID（官方 <c>card_id</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>获取或设置卡券属性（官方 <c>card_ext</c>；仅 preview 页字段表出现）。</summary>
    [JsonPropertyName("card_ext")]
    public MpMassWxCardExt? CardExt { get; set; }
}

/// <summary>卡券属性（官方 <c>card_ext</c>，仅 preview 页 wxcard 分支出现）。</summary>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpMassWxCardExt
{
    /// <summary>获取或设置卡券 code（官方 <c>code</c>）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>获取或设置用户 openid（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置时间戳（官方 <c>timestamp</c>）。</summary>
    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }

    /// <summary>获取或设置签名（官方 <c>signature</c>）。</summary>
    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

/// <summary>群发 / 预览提交结果（官方 sendall 与 mass/send 两页响应字段表一致 ⇒ 共用）。</summary>
/// <remarks>
/// 官方原文：「在返回成功时，意味着群发任务提交成功，并不意味着此时群发已经结束」——
/// 最终结果经回调事件 <c>masssendjobfinish</c>（MASSSENDJOBFINISH）异步推送。
/// 官方响应表另有 <c>type</c> 行（说明原文「次数为news」疑为笔误，照录）——示例响应未携带该字段，不建模。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpMassSendResponse : MpResponse
{
    /// <summary>获取或设置消息发送任务的 ID（官方 <c>msg_id</c>）。</summary>
    [JsonPropertyName("msg_id")]
    public long MsgId { get; set; }

    /// <summary>
    /// 获取或设置消息的数据 ID（官方 <c>msg_data_id</c>；<b>仅群发图文消息时出现</b>，
    /// 用于图文分析数据接口——官方说明原文含叠字「，，」，照录）。
    /// </summary>
    [JsonPropertyName("msg_data_id")]
    public long? MsgDataId { get; set; }
}

/// <summary>删除群发消息（<c>message/mass/delete</c>）请求体。</summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<c>msg_id</c>/<c>url</c> 必须有一个传值（都有值时仅 msg_id 有效）；
/// <c>article_idx</c> 仅在 msg_id 有值时生效。
/// </para>
/// <para>
/// <b>可删除条件（官方「注意事项」原文）</b>：①「只有通过 api 发送的并且已经发送成功的消息才能删除」；
/// ②「删除消息是将消息的正文内容失效，已经收到的用户，还是能在其本地看到消息卡片」；
/// ③「<b>删除群发消息只能删除文章和视频消息</b>，其他类型的消息一经发送，无法删除」；
/// ④「如果多次群发发送的是一个文章，那么删除其中一次群发，就会删除掉这个内容，导致所有群发都失效」。
/// </para>
/// <para>
/// <b>官方文档矛盾（照录）</b>：<c>article_idx</c> 必填列标「是」但说明写「该字段不填或填 0 会删除全部内容」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpMassDeleteRequest
{
    /// <summary>获取或设置发送出去的消息 ID（官方 <c>msg_id</c>，number 形态；与 url 二选一，都有值时仅 msg_id 有效）。</summary>
    [JsonPropertyName("msg_id")]
    public long? MsgId { get; set; }

    /// <summary>获取或设置要删除的内容位置（官方 <c>article_idx</c>，第一篇编号为 1；不填或填 0 删除全部——官方必填标注矛盾，照录）。</summary>
    [JsonPropertyName("article_idx")]
    public int ArticleIdx { get; set; }

    /// <summary>获取或设置要删除的内容 url（官方 <c>url</c>；仅 msg_id 未指定时生效）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>查询群发消息发送状态（<c>message/mass/get</c>）请求体。</summary>
/// <remarks>官方契约（逐页核验 2026-10-07）：请求体 <c>msg_id</c> 必填；官方示例为字符串形态 "201053012"。</remarks>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpMassStatusRequest
{
    /// <summary>获取或设置群发消息后返回的消息 id（官方 <c>msg_id</c>；官方示例为字符串形态）。</summary>
    [JsonPropertyName("msg_id")]
    public string MsgId { get; set; } = string.Empty;
}

/// <summary>查询群发消息发送状态（<c>message/mass/get</c>）响应。</summary>
/// <remarks>
/// <b>逐页核验重要偏差</b>：本接口响应<b>仅 msg_id / msg_status 两字段</b>——
/// TotalCount/FilterCount/SentCount/ErrorCount 只出现在 MASSSENDJOBFINISH 事件推送 XML 中，
/// 不在本查询接口的响应字段表里（方案文档预判已按核验修正）。
/// <b>官方文档矛盾（照录）</b>：响应表 msg_id 标 string、示例为数字 201053012 ⇒ 按示例数字形态建模
/// （请求体示例为字符串 "201053012"——两侧形态本就不一致）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpMassStatusResponse : MpResponse
{
    /// <summary>获取或设置群发消息后返回的消息 id（官方 <c>msg_id</c>；响应表标 string、示例为数字，按示例建模）。</summary>
    [JsonPropertyName("msg_id")]
    public long? MsgId { get; set; }

    /// <summary>获取或设置发送状态（官方 <c>msg_status</c>：SEND_SUCCESS / SENDING / SEND_FAIL / DELETE）。</summary>
    [JsonPropertyName("msg_status")]
    public string? MsgStatus { get; set; }
}

/// <summary>设置群发速度（<c>message/mass/speed/set</c>）请求体。</summary>
/// <remarks>官方 speed 档位原文：「0 为 80w/分钟、1 为 60w/分钟、2 为 45w/分钟、3 为 30w/分钟、4 为 10w/分钟」。</remarks>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpMassSpeedSetRequest
{
    /// <summary>获取或设置群发速度级别（官方 <c>speed</c>：0~4，档位见类型 remarks；官方错误码 45083 表达越界）。</summary>
    [JsonPropertyName("speed")]
    public int Speed { get; set; }
}

/// <summary>获取群发速度（<c>message/mass/speed/get</c>）响应。</summary>
/// <remarks>
/// 官方契约（逐页核验 2026-10-07）：请求体官方声明「无」但代码示例给了 <c>{"speed":3}</c>（矛盾照录——
/// SDK 按参数表建模无请求体）；示例 realspeed:15 与注意事项对照表「speed=3 → 30w/分钟」数值不一致（照录）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpMassSpeedResponse : MpResponse
{
    /// <summary>获取或设置群发速度的级别（官方 <c>speed</c>，0~4）。</summary>
    [JsonPropertyName("speed")]
    public int Speed { get; set; }

    /// <summary>获取或设置群发速度的真实值（官方 <c>realspeed</c>，单位万/分钟）。</summary>
    [JsonPropertyName("realspeed")]
    public int RealSpeed { get; set; }
}
