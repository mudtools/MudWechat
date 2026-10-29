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
/// 查询作品授权请求
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelQueryBookAuthRequest
{
    /// <summary>
    /// 填 0 或者不设置: 查授权列表, 填 1: 查被授权列表（查看被授权关系列表时必填）
    /// </summary>
    [JsonPropertyName("type")]
    public long? Type { get; set; }

    /// <summary>
    /// 结果数据偏移位置 从 0 开始，用于翻页
    /// </summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>
    /// 获取记录的数量, 上限一次 30 条，用于翻页
    /// </summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>
    /// 填 true，返回总数，填 false 不返回（查看被授权关系列表时无需填写）
    /// </summary>
    [JsonPropertyName("is_sum")]
    public bool? IsSum { get; set; }

    /// <summary>
    /// 过滤出等于该 book_id 的授权结果（查看被授权关系列表时无需填写）
    /// </summary>
    [JsonPropertyName("book_id")]
    public string? BookId { get; set; }
}

/// <summary>
/// 查询作品授权响应
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelQueryBookAuthResponse : WxaResponse
{
    /// <summary>
    /// 授权结果列表
    /// </summary>
    [JsonPropertyName("results")]
    public List<WxaNovelAuthResultItem>? Results { get; set; }
}

/// <summary>
/// 授权结果项（querybookauth 版本）
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelAuthResultItem
{
    /// <summary>
    /// 作品id
    /// </summary>
    [JsonPropertyName("book_id")]
    public string? BookId { get; set; }

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
    /// 总数，is_sum = true 返回（查看被授权关系列表时不返回）
    /// </summary>
    [JsonPropertyName("sum")]
    public long? Sum { get; set; }
}
