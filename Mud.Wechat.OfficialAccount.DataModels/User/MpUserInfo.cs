// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Abstractions.Contracts;

namespace Mud.Wechat.OfficialAccount.DataModels.User;

/// <summary>
/// 公众号用户基本信息（官方 <c>user/info</c> 平铺响应体 与 <c>user/info/batchget</c> 的
/// <c>user_info_list</c> 元素<b>共用同一字段集</b>，故单一类型承载）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何不建模 <c>nickname</c> / <c>sex</c> / <c>city</c> / <c>province</c> / <c>country</c> /
/// <c>headimgurl</c></b>：官方明示「<b>2021 年 12 月 27 日之后，不再输出头像、昵称信息</b>」，
/// 且批量页把上述字段逐个标注「该字段不再提供」⇒ 建模它们只会给出「也许能读到」的错误暗示
/// （实际恒缺席）。若官方将来恢复输出，须先核对文档再同批补字段与守卫。
/// </para>
/// <para>
/// <b><c>language</c> 已停供但要保留</b>：官方「获取用户基本信息」页在该字段上标注
/// 「【注意：该字段不再提供】」，但字段仍列在响应表中 ⇒ 建模为可空（实际为 <c>null</c>），
/// 避免调用方据官方字段表反查时找不到落点。
/// </para>
/// <para>
/// <b><c>subscribe = 0</c> 语义</b>：官方原文「值为 0 时，代表此用户没有关注该公众号，
/// <b>拉取不到其余信息</b>」⇒ 调用方必须先判 <see cref="Subscribe"/>，不得假定其余字段存在。
/// </para>
/// <para>
/// <b><c>unionid</c> 前提</b>：仅在用户将公众号绑定到微信开放平台账号后才出现（否则 <c>null</c>）。
/// </para>
/// <para>
/// <b><c>subscribe_scene</c> 类型（官方两页不一致的处置）</b>：「获取用户基本信息」页标为
/// <c>string</c> 且示例为 <c>"ADD_SCENE_QR_CODE"</c>；「批量获取用户基本信息」页标为 <c>number</c>。
/// 以<b>带示例的一页</b>为准 ⇒ 本 SDK 按<b>字符串</b>建模（<c>ADD_SCENE_*</c> 枚举值见官方说明）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "User")]
public class MpUserInfo
{
    /// <summary>
    /// 用户是否订阅该公众号（<c>0</c> = 未关注，此时<b>拉取不到其余信息</b>；<c>1</c> = 已关注）。
    /// </summary>
    [JsonPropertyName("subscribe")]
    public int Subscribe { get; set; }

    /// <summary>用户的标识，对当前公众号唯一。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>用户的语言（官方标注<b>该字段不再提供</b>；保留以对齐官方字段表，实际为 <c>null</c>）。</summary>
    [JsonPropertyName("language")]
    public string? Language { get; set; }

    /// <summary>用户关注时间（Unix 时间戳，秒）；多次关注取<b>最后</b>一次关注时间。</summary>
    [JsonPropertyName("subscribe_time")]
    public long SubscribeTime { get; set; }

    /// <summary>微信开放平台账号下的唯一标识（仅当公众号绑定开放平台账号后才出现）。</summary>
    [JsonPropertyName("unionid")]
    public string? UnionId { get; set; }

    /// <summary>公众号运营者对粉丝的备注（由运营者在公众平台用户管理界面设置）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>用户所在的分组 ID（兼容旧的用户分组接口）。</summary>
    [JsonPropertyName("groupid")]
    public int GroupId { get; set; }

    /// <summary>用户被打上的标签 ID 列表。</summary>
    [JsonPropertyName("tagid_list")]
    public List<int>? TagIdList { get; set; }

    /// <summary>
    /// 用户关注的渠道来源（官方 <c>ADD_SCENE_*</c> 取值，如 <c>ADD_SCENE_SEARCH</c> /
    /// <c>ADD_SCENE_QR_CODE</c> / <c>ADD_SCENE_WECHAT_ADVERTISEMENT</c> 等，共 13 项）。
    /// </summary>
    /// <remarks>官方 2020-06-08 起把「微信广告」从「其他（<c>ADD_SCENE_OTHERS</c>）」中拆分单列。</remarks>
    [JsonPropertyName("subscribe_scene")]
    public string? SubscribeScene { get; set; }

    /// <summary>二维码扫码场景（开发者自定义）。</summary>
    [JsonPropertyName("qr_scene")]
    public int QrScene { get; set; }

    /// <summary>二维码扫码场景描述（开发者自定义）。</summary>
    [JsonPropertyName("qr_scene_str")]
    public string? QrSceneStr { get; set; }
}

/// <summary>
/// 获取用户基本信息响应（<c>userInfo</c>，<c>GET /cgi-bin/user/info</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何继承 <see cref="MpUserInfo"/> 而非 <see cref="MpResponse"/></b>：官方本端点响应是
/// 「用户字段<b>平铺</b> + 出错时附带 <c>errcode</c>/<c>errmsg</c>」——成功与失败共用同一层级。
/// 若改为 <c>: MpResponse</c> + 重新声明全部用户字段，会与
/// <see cref="MpUserInfo"/>（批量端点的元素类型）产生<b>两份会漂移的字段声明</b>。
/// 故这里以「用户字段继承 + 判错契约显式实现」表达官方形态。
/// </para>
/// <para>
/// 官方响应字段（逐项核验）：<c>subscribe</c> / <c>openid</c> / <c>language</c> / <c>subscribe_time</c> /
/// <c>unionid</c> / <c>remark</c> / <c>groupid</c> / <c>tagid_list</c> / <c>subscribe_scene</c> /
/// <c>qr_scene</c> / <c>qr_scene_str</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "User")]
public class MpUserInfoResponse : MpUserInfo, IWechatApiResponse
{
    /// <summary>业务错误码（成功响应缺省 ⇒ 0）。</summary>
    [JsonPropertyName("errcode")]
    public virtual int ErrorCode { get; set; }

    /// <summary>业务错误信息（可空）。</summary>
    [JsonPropertyName("errmsg")]
    public virtual string? ErrorMessage { get; set; }

    /// <inheritdoc />
    [JsonIgnore]
    public bool IsSuccess => ErrorCode == 0;
}
