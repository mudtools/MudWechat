// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Novel;

/// <summary>
/// 添加作品授权（按appid）请求
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelAddBookAuthByAppIdRequest
{
    /// <summary>
    /// 被授权账号列表。上限一次 20 条
    /// </summary>
    [JsonPropertyName("infos")]
    public List<WxaNovelAccountAuthInfo>? Infos { get; set; }
}

/// <summary>
/// 账号授权信息项
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelAccountAuthInfo
{
    /// <summary>
    /// 被授权账号appid
    /// </summary>
    [JsonPropertyName("grantee_appid")]
    public string? GranteeAppId { get; set; }

    /// <summary>
    /// 授权到期时间，unix时间戳，单位秒，按CST时区
    /// </summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }
}

/// <summary>
/// 添加作品授权（按appid）响应
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelAddBookAuthByAppIdResponse : WxaResponse
{
    /// <summary>
    /// 授权结果列表
    /// </summary>
    [JsonPropertyName("results")]
    public List<WxaNovelAuthResult>? Results { get; set; }
}

/// <summary>
/// 查询作品授权V2请求
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelQueryBookAuthV2Request
{
    /// <summary>
    /// 填 0 或者不设置: 查授权列表, 填 1: 查被授权列表
    /// </summary>
    [JsonPropertyName("type")]
    public long? Type { get; set; }

    /// <summary>
    /// 获取记录的数量, 上限一次 100 条，用于分页
    /// </summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>
    /// 分页游标。不填或填空字符串，标识从第一页开始查询。后一页查询填充上一页响应的next_cursor字段值
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 用于查询指定授权方授权给当前账户的小说列表，当查询指定授权方所授权的小说列表时需要传
    /// </summary>
    [JsonPropertyName("grantor_appid")]
    public string? GrantorAppId { get; set; }

    /// <summary>
    /// 用于查询指定小说的授权信息，上限一次 30 条。查询指定小说的授权信息场景下需要传递此参数。优先级高于 grantor_appid，当 book_ids 不为空时，grantor_appid 参数无效。
    /// </summary>
    [JsonPropertyName("book_ids")]
    public List<string>? BookIds { get; set; }
}

/// <summary>
/// 查询作品授权V2响应
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelQueryBookAuthV2Response : WxaResponse
{
    /// <summary>
    /// 被授权方列表, 结果按授权过期时间从早到晚排序
    /// </summary>
    [JsonPropertyName("appid_results")]
    public List<WxaNovelAppIdResult>? AppIdResults { get; set; }

    /// <summary>
    /// 下一页游标
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }
}

/// <summary>
/// 被授权方结果项
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelAppIdResult
{
    /// <summary>
    /// 主授权账号appid
    /// </summary>
    [JsonPropertyName("grantor_appid")]
    public string? GrantorAppId { get; set; }

    /// <summary>
    /// 被授权账号appid
    /// </summary>
    [JsonPropertyName("grantee_appid")]
    public string? GranteeAppId { get; set; }

    /// <summary>
    /// 授权到期时间，unix时间戳，单位秒，按CST时区
    /// </summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }

    /// <summary>
    /// 作品id，查询指定授权方授权给当前账户的小说列表时会返回
    /// </summary>
    [JsonPropertyName("book_id")]
    public string? BookId { get; set; }
}

/// <summary>
/// 删除作品授权（按appid）请求
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelDelBookAuthByAppIdRequest
{
    /// <summary>
    /// 作品id
    /// </summary>
    [JsonPropertyName("book_id")]
    public string? BookId { get; set; }

    /// <summary>
    /// 被授权账号appid
    /// </summary>
    [JsonPropertyName("grantee_appid")]
    public string? GranteeAppId { get; set; }
}

/// <summary>
/// 删除作品授权（按appid）响应
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelDelBookAuthByAppIdResponse : WxaResponse
{
}
