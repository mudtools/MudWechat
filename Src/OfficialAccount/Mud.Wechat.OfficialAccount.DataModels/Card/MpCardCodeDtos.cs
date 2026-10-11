// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

// ---------------------------------------------------------------- 核销 / 查询 / 解密券码（POST /card/code/*）

/// <summary>
/// 核销卡券（<c>POST /card/code/consume</c>）请求体。
/// </summary>
/// <remarks>
/// <b><c>card_id</c> 选填</b>：官方允许仅凭 <c>code</c> 核销（由码反查卡），故本型 <c>card_id</c> 可空、
/// <c>code</c> 必填。SDK <b>不做</b>「code 必须配合 card_id」的本地校验。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCodeConsumeRequest
{
    /// <summary>获取或设置卡券模板编号（官方 <c>card_id</c>，选填）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>获取或设置券码（官方 <c>code</c>，必填；加密码须先经 <c>/card/code/decrypt</c> 解密）。</summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
}

/// <summary>
/// 核销卡券（<c>POST /card/code/consume</c>）应答。
/// </summary>
/// <remarks><b>应答只有两字段</b>：<c>card.card_id</c> 与 <c>openid</c>（核销对象用户）——不返回券码本身。</remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCodeConsumeResponse : MpResponse
{
    /// <summary>获取或设置卡券信息（官方 <c>card</c>，仅含 <c>card_id</c>）。</summary>
    [JsonPropertyName("card")]
    public MpCardConsumeCardInfo? Card { get; set; }

    /// <summary>获取或设置核销用户的 openid（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}

/// <summary>核销应答的 <c>card</c> 对象（官方 <c>card</c>，本向仅一个字段）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardConsumeCardInfo
{
    /// <summary>获取或设置卡券模板编号（官方 <c>card_id</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }
}

/// <summary>
/// 查询券码状态（<c>POST /card/code/get</c>）请求体。
/// </summary>
/// <remarks>
/// <para><c>check_consume</c> 为「是否同时校验可否核销」开关（选填），置真时应答携带 <c>can_consume</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCodeGetRequest
{
    /// <summary>获取或设置卡券模板编号（官方 <c>card_id</c>，选填）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>获取或设置券码（官方 <c>code</c>，必填）。</summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>获取或设置是否校验可核销性（官方 <c>check_consume</c>，选填）。</summary>
    [JsonPropertyName("check_consume")]
    public bool? CheckConsume { get; set; }
}

/// <summary>
/// 查询券码状态（<c>POST /card/code/get</c>）应答。
/// </summary>
/// <remarks>
/// <para>
/// <b>字段名三处易错</b>（对齐基准：SKIT <c>CardCodeGetResponse</c>，守卫 CD7 锁定）：
/// 券有效期用 <c>begin_time</c> / <c>end_time</c>（<b>非</b>建卡侧的 <c>begin_timestamp</c>）；
/// 会员积分/余额在<b>顶层</b>用 <c>bonus</c> / <c>balance</c>（与 <c>card</c> 内的同名键是两码事，
/// 故 SDK 属性名区分 <see cref="MpCardCodeInfo.Bonus"/> 与 <see cref="MemberBonus"/>）；
/// 用户信息容器为 <c>user_info</c>（内层是 <c>common_field_list</c> / <c>custom_field_list</c> 两张 name/value 表）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCodeGetResponse : MpResponse
{
    /// <summary>获取或设置券码与卡券信息（官方 <c>card</c>）。</summary>
    [JsonPropertyName("card")]
    public MpCardCodeInfo? Card { get; set; }

    /// <summary>获取或设置券码状态（官方 <c>user_card_status</c>；状态枚举<b>待官方逐页核验</b>）。</summary>
    [JsonPropertyName("user_card_status")]
    public string? CardStatus { get; set; }

    /// <summary>获取或设置持券用户 openid（官方 <c>openid</c>）。</summary>
    /// <remarks><b>官方键名为全小写 <c>openid</c></b>（非公众号用户域惯用的 <c>openid</c> 之外的形态）。</remarks>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置持券用户昵称（官方 <c>nickname</c>）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>获取或设置会员卡编号（官方 <c>membership_number</c>，仅会员卡）。</summary>
    [JsonPropertyName("membership_number")]
    public string? MembershipNumber { get; set; }

    /// <summary>获取或设置会员信息（官方 <c>user_info</c>，仅会员卡）。</summary>
    [JsonPropertyName("user_info")]
    public MpCardMemberUserInfo? UserInfo { get; set; }

    /// <summary>获取或设置会员积分（官方 <c>bonus</c>，顶层会员维度）。</summary>
    [JsonPropertyName("bonus")]
    public int? MemberBonus { get; set; }

    /// <summary>获取或设置会员余额（官方 <c>balance</c>，顶层会员维度，单位：分）。</summary>
    [JsonPropertyName("balance")]
    public int? MemberBalance { get; set; }

    /// <summary>获取或设置订单号（官方 <c>order_id</c>，礼品卡购买场景）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置卡面背景图 URL（官方 <c>background_pic_url</c>）。</summary>
    [JsonPropertyName("background_pic_url")]
    public string? BackgroundPicUrl { get; set; }

    /// <summary>获取或设置当前是否可核销（官方 <c>can_consume</c>，须请求侧 <c>check_consume</c> 为真）。</summary>
    [JsonPropertyName("can_consume")]
    public bool? CanConsume { get; set; }

    /// <summary>获取或设置外部自定义串（官方 <c>outer_str</c>，建码时传入的回显）。</summary>
    [JsonPropertyName("outer_str")]
    public string? OuterStr { get; set; }
}

/// <summary>券码与卡券信息（官方 <c>code/get</c> 应答的 <c>card</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCodeInfo
{
    /// <summary>获取或设置卡券模板编号（官方 <c>card_id</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>获取或设置券码（官方 <c>code</c>）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>获取或设置卡券编号（官方 <c>card_number</c>）。</summary>
    [JsonPropertyName("card_number")]
    public string? CardNumber { get; set; }

    /// <summary>获取或设置生效时间戳（官方 <c>begin_time</c>，秒；<b>本向键名为 <c>begin_time</c></b>）。</summary>
    [JsonPropertyName("begin_time")]
    public long BeginTime { get; set; }

    /// <summary>获取或设置失效时间戳（官方 <c>end_time</c>，秒；<b>本向键名为 <c>end_time</c></b>）。</summary>
    [JsonPropertyName("end_time")]
    public long EndTime { get; set; }

    /// <summary>获取或设置该券码携带的积分（官方 <c>bonus</c>，与顶层会员积分同名不同义）。</summary>
    [JsonPropertyName("bonus")]
    public int? Bonus { get; set; }

    /// <summary>获取或设置该券码携带的余额（官方 <c>balance</c>，单位：分）。</summary>
    [JsonPropertyName("balance")]
    public int? Balance { get; set; }
}

/// <summary>
/// 会员信息（官方 <c>user_info</c>）—— 两张 name/value 列表。
/// </summary>
/// <remarks>
/// <b>为何建模在本域</b>：本对象是 <c>/card/code/get</c> 的应答组成部分，即便「会员卡接口族」本轮不建模，
/// 该形态仍必须承载，否则核销/查询面丢字段。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardMemberUserInfo
{
    /// <summary>获取或设置通用信息类目（官方 <c>common_field_list</c>）。</summary>
    [JsonPropertyName("common_field_list")]
    public List<MpCardUserField>? CommonFieldList { get; set; }

    /// <summary>获取或设置自定义信息类目（官方 <c>custom_field_list</c>）。</summary>
    [JsonPropertyName("custom_field_list")]
    public List<MpCardUserField>? CustomFieldList { get; set; }
}

/// <summary>会员信息项（官方 <c>*_field_list[]</c>，name/value 成对）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUserField
{
    /// <summary>获取或设置信息项名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置信息项取值（官方 <c>value</c>）。</summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

/// <summary>
/// 解密券码（<c>POST /card/code/decrypt</c>）请求体。
/// </summary>
/// <remarks>
/// <b>入参是 <c>encrypt_code</c> 而非 <c>code</c></b>：微信卡券在「扫码」链路上给用户端的是加密码，
/// 核销/查询前必须先解密 ⇒ 本端点是 <see cref="MpCardCodeConsumeRequest"/> 与
/// <see cref="MpCardCodeGetRequest"/> 的前置步骤（SDK 只给端点、<b>不编排</b>两步）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCodeDecryptRequest
{
    /// <summary>获取或设置加密券码（官方 <c>encrypt_code</c>，必填）。</summary>
    [JsonPropertyName("encrypt_code")]
    public string EncryptCode { get; set; } = string.Empty;
}

/// <summary>解密券码（<c>POST /card/code/decrypt</c>）应答。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCodeDecryptResponse : MpResponse
{
    /// <summary>获取或设置解密后的原始券码（官方 <c>code</c>）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }
}
