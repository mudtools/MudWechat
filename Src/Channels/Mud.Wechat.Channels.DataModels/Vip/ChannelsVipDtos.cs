// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Vip;

/// <summary>
/// 会员用户 openid 请求体（<c>score/get</c> 与 <c>info/get</c> 请求体字段集一致——均为单个
/// <c>openid</c> 必填字段 ⇒ 共用同一 DTO）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class ChannelsVipOpenidRequest
{
    /// <summary>获取或设置用户 openid（官方 <c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;
}

/// <summary>获取用户积分（<c>score/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class ChannelsVipGetUserScoreResponse : ChannelsResponse
{
    /// <summary>获取或设置用户积分信息（官方 <c>info</c>）。</summary>
    [JsonPropertyName("info")]
    public ChannelsVipUserScoreInfo? Info { get; set; }
}

/// <summary>用户积分信息（官方 <c>info</c> 对象，<c>score/get</c>）。</summary>
/// <remarks><b>官方类型照录</b>：<c>score</c> 官方参数表类型为 string（示例 <c>"100"</c>）——与流水中的
/// number 型积分变动值不一致，照录建模。</remarks>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class ChannelsVipUserScoreInfo
{
    /// <summary>获取或设置用户当前积分余额（官方 <c>score</c>，官方类型为 string）。</summary>
    [JsonPropertyName("score")]
    public string? Score { get; set; }
}

/// <summary>获取用户信息（<c>info/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class ChannelsVipGetUserInfoResponse : ChannelsResponse
{
    /// <summary>获取或设置用户信息（官方 <c>info</c>，结构同用户列表元素 ⇒ 共用 <see cref="ChannelsVipUserInfo"/>）。</summary>
    [JsonPropertyName("info")]
    public ChannelsVipUserInfo? Info { get; set; }
}

/// <summary>
/// 会员用户信息（官方 <c>info</c> 对象，<c>info/get</c>；官方 <c>list</c> 数组元素，<c>list/get</c>）——
/// 两处字段集完全一致（openid / unionid / user_grade_info）⇒ 共用同一 DTO。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class ChannelsVipUserInfo
{
    /// <summary>获取或设置用户 openid（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置用户 unionid（官方 <c>unionid</c>）。</summary>
    [JsonPropertyName("unionid")]
    public string? UnionId { get; set; }

    /// <summary>获取或设置用户等级信息（官方 <c>user_grade_info</c>）。</summary>
    [JsonPropertyName("user_grade_info")]
    public ChannelsVipUserGradeInfo? UserGradeInfo { get; set; }
}

/// <summary>用户等级信息（官方 <c>user_grade_info</c> 对象，<c>info/get</c> 与 <c>list/get</c> 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class ChannelsVipUserGradeInfo
{
    /// <summary>获取或设置会员等级（官方 <c>grade</c>）。</summary>
    [JsonPropertyName("grade")]
    public int? Grade { get; set; }

    /// <summary>获取或设置当前等级经验值（官方 <c>experience_value</c>）。</summary>
    [JsonPropertyName("experience_value")]
    public long? ExperienceValue { get; set; }
}

/// <summary>获取用户列表（<c>list/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class ChannelsVipGetUserListRequest
{
    /// <summary>获取或设置当前页（官方 <c>page_num</c>，必填，从 1 开始）。</summary>
    [JsonPropertyName("page_num")]
    public int PageNum { get; set; }

    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，必填，最大 200）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }
}

/// <summary>获取用户列表（<c>list/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class ChannelsVipGetUserListResponse : ChannelsResponse
{
    /// <summary>获取或设置用户信息列表（官方 <c>list</c>，元素结构同单点用户信息 ⇒ 共用 <see cref="ChannelsVipUserInfo"/>）。</summary>
    [JsonPropertyName("list")]
    public List<ChannelsVipUserInfo>? List { get; set; }

    /// <summary>获取或设置会员用户总数（官方 <c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }
}

/// <summary>获取用户积分流水（<c>score/flowrecord/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class ChannelsVipGetUserScoreFlowRecordRequest
{
    /// <summary>获取或设置用户 openid（官方 <c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;

    /// <summary>获取或设置当前页（官方 <c>page_num</c>，必填，从 1 开始）。</summary>
    [JsonPropertyName("page_num")]
    public int PageNum { get; set; }

    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，必填）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }
}

/// <summary>获取用户积分流水（<c>score/flowrecord/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class ChannelsVipGetUserScoreFlowRecordResponse : ChannelsResponse
{
    /// <summary>获取或设置积分流水列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<ChannelsVipScoreFlowRecord>? List { get; set; }

    /// <summary>获取或设置流水记录总数（官方 <c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }
}

/// <summary>积分流水记录（官方 <c>list</c> 数组元素，<c>score/flowrecord/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class ChannelsVipScoreFlowRecord
{
    /// <summary>获取或设置积分变动值（官方 <c>score</c>，正数为增加，负数为扣减）。</summary>
    [JsonPropertyName("score")]
    public int? Score { get; set; }

    /// <summary>获取或设置流水来源（官方 <c>source</c>，见 <see cref="ChannelsVipScoreFlowSources"/>）。</summary>
    [JsonPropertyName("source")]
    public int? Source { get; set; }

    /// <summary>获取或设置流水备注说明（官方 <c>remark</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }
}

/// <summary>小店获取关联小程序信息（<c>v3/wxa/info/get</c>）响应。</summary>
/// <remarks>官方契约：<b>POST</b> + 空请求体；令牌为小店 access_token（第三方代调用权限集 id 186）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class ChannelsVipGetWxaInfoResponse : ChannelsResponse
{
    /// <summary>获取或设置关联的小程序 appid（官方 <c>wxa_appid</c>）。</summary>
    [JsonPropertyName("wxa_appid")]
    public string? WxaAppId { get; set; }

    /// <summary>获取或设置关联小程序信息（官方 <c>info</c>）。</summary>
    [JsonPropertyName("info")]
    public ChannelsVipWxaInfo? Info { get; set; }

    /// <summary>获取或设置上线状态（官方 <c>state</c>，见 <see cref="ChannelsVipWxaStates"/>；为 2/3 时仅白名单用户可在小店看到会员入口）。</summary>
    [JsonPropertyName("state")]
    public int? State { get; set; }
}

/// <summary>关联小程序信息（官方 <c>info</c> 对象，<c>v3/wxa/info/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class ChannelsVipWxaInfo
{
    /// <summary>获取或设置小程序会员信息（官方 <c>paths</c>）。</summary>
    [JsonPropertyName("paths")]
    public ChannelsVipWxaPaths? Paths { get; set; }

    /// <summary>获取或设置小程序测试白名单用户 openid 列表（官方 <c>whitelist_openids</c>）。</summary>
    [JsonPropertyName("whitelist_openids")]
    public List<string>? WhitelistOpenIds { get; set; }

    /// <summary>获取或设置下单页权益（官方 <c>display_info</c>）。</summary>
    [JsonPropertyName("display_info")]
    public ChannelsVipWxaDisplayInfo? DisplayInfo { get; set; }
}

/// <summary>小程序会员页面路径（官方 <c>paths</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class ChannelsVipWxaPaths
{
    /// <summary>获取或设置小程序会员注册页面路径（官方 <c>register_path</c>）。</summary>
    [JsonPropertyName("register_path")]
    public string? RegisterPath { get; set; }

    /// <summary>获取或设置小程序会员权益页面路径（官方 <c>rights_path</c>）。</summary>
    [JsonPropertyName("rights_path")]
    public string? RightsPath { get; set; }
}

/// <summary>下单页权益（官方 <c>display_info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class ChannelsVipWxaDisplayInfo
{
    /// <summary>获取或设置下单页权益展示开关（官方 <c>order_page_display</c>，见 <see cref="ChannelsVipOrderPageDisplaySwitches"/>）。</summary>
    [JsonPropertyName("order_page_display")]
    public int? OrderPageDisplay { get; set; }
}

/// <summary>积分流水来源常量（官方 <c>source</c> 枚举值，<c>score/flowrecord/get</c>）。</summary>
public static class ChannelsVipScoreFlowSources
{
    /// <summary>下单-增加积分。</summary>
    public const int OrderEarn = 2;

    /// <summary>下单-抵扣积分。</summary>
    public const int OrderDeduct = 5;
}

/// <summary>关联小程序上线状态常量（官方 <c>state</c> 枚举值，<c>v3/wxa/info/get</c>）。</summary>
public static class ChannelsVipWxaStates
{
    /// <summary>上线。</summary>
    public const int Online = 1;

    /// <summary>未上线。</summary>
    public const int Offline = 2;

    /// <summary>上线前测试状态。</summary>
    public const int PreOnlineTesting = 3;
}

/// <summary>下单页权益展示开关常量（官方 <c>order_page_display</c> 枚举值，<c>v3/wxa/info/get</c>）。</summary>
public static class ChannelsVipOrderPageDisplaySwitches
{
    /// <summary>开。</summary>
    public const int On = 1;

    /// <summary>关。</summary>
    public const int Off = 2;
}
