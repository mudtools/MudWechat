// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.Abstractions.Contracts;

namespace Mud.Wechat.MiniProgram.DataModels.Game;

/// <summary>
/// 小游戏域 DTO（20 端点：帧同步房间 4 + 用户数据存储 3 + 对局匹配 5 + 米大师支付 1 + 内容安全 2 + 米大师 V2 支付 5）。
/// </summary>

// ==================== 帧同步游戏房间（4 端点）====================

/// <summary>创建帧同步游戏房间请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GameCreateRoomRequest
{
    /// <summary>游戏 ID。</summary>
    [JsonPropertyName("game_id")]
    public string? GameId { get; set; }

    /// <summary>房间名称。</summary>
    [JsonPropertyName("room_name")]
    public string? RoomName { get; set; }

    /// <summary>最大玩家数。</summary>
    [JsonPropertyName("max_players")]
    public int? MaxPlayers { get; set; }
}

/// <summary>创建帧同步游戏房间响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GameCreateRoomResponse : WxaResponse
{
    /// <summary>房间 ID。</summary>
    [JsonPropertyName("room_id")]
    public string? RoomId { get; set; }
}

/// <summary>获取帧数据响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GameGetFrameResponse : WxaResponse
{
    /// <summary>帧数据列表。</summary>
    [JsonPropertyName("frames")]
    public List<GameFrameItem>? Frames { get; set; }
}

/// <summary>帧数据项。</summary>
public sealed class GameFrameItem
{
    /// <summary>帧 ID。</summary>
    [JsonPropertyName("frame_id")]
    public long FrameId { get; set; }

    /// <summary>帧数据。</summary>
    [JsonPropertyName("data")]
    public string? Data { get; set; }
}

/// <summary>获取对局玩家位次信息响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GameGetIdentityInfoResponse : WxaResponse
{
    /// <summary>玩家位次列表。</summary>
    [JsonPropertyName("players")]
    public List<GamePlayerItem>? Players { get; set; }
}

/// <summary>玩家位次项。</summary>
public sealed class GamePlayerItem
{
    /// <summary>玩家 openid。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>位次。</summary>
    [JsonPropertyName("position")]
    public int Position { get; set; }
}

/// <summary>获取指定房间信息响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GameGetRoomInfoResponse : WxaResponse
{
    /// <summary>房间 ID。</summary>
    [JsonPropertyName("room_id")]
    public string? RoomId { get; set; }

    /// <summary>房间状态。</summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>当前玩家数。</summary>
    [JsonPropertyName("current_players")]
    public int CurrentPlayers { get; set; }
}

// ==================== 用户数据存储（3 端点）====================

/// <summary>删除已上报KV数据请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GameRemoveUserStorageRequest
{
    /// <summary>用户 openid。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>要删除的 key 列表。</summary>
    [JsonPropertyName("keys")]
    public List<string>? Keys { get; set; }
}

/// <summary>写用户关系链互动数据存储请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GameSetUserInteractiveDataRequest
{
    /// <summary>用户 openid。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>互动数据。</summary>
    [JsonPropertyName("data")]
    public string? Data { get; set; }
}

/// <summary>上报用户数据请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GameSetUserStorageRequest
{
    /// <summary>用户 openid。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>KV 数据。</summary>
    [JsonPropertyName("kv_list")]
    public List<GameKvItem>? KvList { get; set; }
}

/// <summary>KV 数据项。</summary>
public sealed class GameKvItem
{
    /// <summary>键。</summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>值。</summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

// ==================== 对局匹配规则（5 端点）====================

/// <summary>创建对局匹配规则请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GameCreateMatchRuleRequest
{
    /// <summary>规则名称。</summary>
    [JsonPropertyName("rule_name")]
    public string? RuleName { get; set; }

    /// <summary>最小玩家数。</summary>
    [JsonPropertyName("min_players")]
    public int MinPlayers { get; set; }

    /// <summary>最大玩家数。</summary>
    [JsonPropertyName("max_players")]
    public int MaxPlayers { get; set; }
}

/// <summary>创建对局匹配规则响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GameCreateMatchRuleResponse : WxaResponse
{
    /// <summary>匹配规则 ID。</summary>
    [JsonPropertyName("match_id")]
    public string? MatchId { get; set; }
}

/// <summary>获取对局匹配规则响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GameGetAllMatchRuleResponse : WxaResponse
{
    /// <summary>匹配规则列表。</summary>
    [JsonPropertyName("rules")]
    public List<GameMatchRuleItem>? Rules { get; set; }
}

/// <summary>匹配规则项。</summary>
public sealed class GameMatchRuleItem
{
    /// <summary>规则 ID。</summary>
    [JsonPropertyName("match_id")]
    public string? MatchId { get; set; }

    /// <summary>规则名称。</summary>
    [JsonPropertyName("rule_name")]
    public string? RuleName { get; set; }

    /// <summary>是否开启。</summary>
    [JsonPropertyName("is_open")]
    public bool IsOpen { get; set; }
}

/// <summary>修改对局规则打开状态请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GameSetMatchOpenStateRequest
{
    /// <summary>匹配规则 ID。</summary>
    [JsonPropertyName("match_id")]
    public string? MatchId { get; set; }

    /// <summary>是否开启。</summary>
    [JsonPropertyName("is_open")]
    public bool IsOpen { get; set; }
}

/// <summary>修改对局匹配规则请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GameUpdateMatchRuleRequest
{
    /// <summary>匹配规则 ID。</summary>
    [JsonPropertyName("match_id")]
    public string? MatchId { get; set; }

    /// <summary>规则名称。</summary>
    [JsonPropertyName("rule_name")]
    public string? RuleName { get; set; }
}

// ==================== 米大师虚拟支付 business 族（1 端点）====================

/// <summary>查询订单支付信息请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GameGetPayForOrderRequest
{
    /// <summary>订单号。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}

/// <summary>查询订单支付信息响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GameGetPayForOrderResponse : WxaResponse
{
    /// <summary>支付状态。</summary>
    [JsonPropertyName("pay_status")]
    public int PayStatus { get; set; }

    /// <summary>金额（分）。</summary>
    [JsonPropertyName("amount")]
    public long Amount { get; set; }
}

// ==================== 内容安全（2 端点）====================

/// <summary>文本内容安全识别请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GameMsgSecCheckRequest
{
    /// <summary>待检测的文本内容。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

/// <summary>文本内容安全识别响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GameMsgSecCheckResponse : WxaResponse
{
    /// <summary>检测结果：0-正常，1-违规。</summary>
    [JsonPropertyName("result")]
    public int Result { get; set; }

    /// <summary>违规标签。</summary>
    [JsonPropertyName("label")]
    public string? Label { get; set; }
}

/// <summary>图片内容安全识别请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GameMediaCheckRequest
{
    /// <summary>图片 URL。</summary>
    [JsonPropertyName("img_url")]
    public string? ImgUrl { get; set; }
}

/// <summary>图片内容安全识别响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GameMediaCheckResponse : WxaResponse
{
    /// <summary>检测结果：0-正常，1-违规。</summary>
    [JsonPropertyName("result")]
    public int Result { get; set; }
}

// ==================== 米大师 V2 虚拟支付 game 族（5 端点）====================

/// <summary>查询余额请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GameGetBalanceRequest
{
    /// <summary>用户 openid。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}

/// <summary>查询余额响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GameGetBalanceResponse : WxaResponse
{
    /// <summary>余额（分）。</summary>
    [JsonPropertyName("balance")]
    public long Balance { get; set; }
}

/// <summary>支付请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GamePayRequest
{
    /// <summary>用户 openid。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>金额（分）。</summary>
    [JsonPropertyName("amount")]
    public long Amount { get; set; }

    /// <summary>商品描述。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

/// <summary>支付响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GamePayResponse : WxaResponse
{
    /// <summary>订单号。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}

/// <summary>取消支付请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GameCancelPayRequest
{
    /// <summary>订单号。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}

/// <summary>取消支付响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GameCancelPayResponse : WxaResponse
{
    /// <summary>取消结果。</summary>
    [JsonPropertyName("cancel_result")]
    public string? CancelResult { get; set; }
}

/// <summary>赠送请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GamePresentRequest
{
    /// <summary>接收者 openid。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>赠送数量。</summary>
    [JsonPropertyName("amount")]
    public long Amount { get; set; }
}

/// <summary>赠送响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GamePresentResponse : WxaResponse
{
    /// <summary>赠送结果。</summary>
    [JsonPropertyName("present_result")]
    public string? PresentResult { get; set; }
}

/// <summary>查询订单状态请求。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GameQueryOrderRequest
{
    /// <summary>订单号。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}

/// <summary>查询订单状态响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public sealed class GameQueryOrderResponse : WxaResponse
{
    /// <summary>订单状态。</summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>金额（分）。</summary>
    [JsonPropertyName("amount")]
    public long Amount { get; set; }
}
